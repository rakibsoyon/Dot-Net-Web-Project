using BusinessLogic.Interface;

using JqueryDataTables.ServerSide.AspNetCoreWeb.ActionResults;
using JqueryDataTables.ServerSide.AspNetCoreWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Shared.Models;
using System.Diagnostics;
using System.Text.Json;
using static Web.Utility.Constant;


namespace Web.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;

        [Obsolete]
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;

        [Obsolete]
        public CategoryController(ICategoryRepository categoryRepository, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment)
        {
            _categoryRepository = categoryRepository;
            _environment = environment;
        }

        // GET: Category

        [HttpGet]
        public IActionResult Index()
        {
            return View(new CategoryViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> LoadTable([FromBody] JqueryDataTablesParameters param)
        {
            try
            {
                HttpContext.Session.SetString(nameof(JqueryDataTablesParameters), JsonSerializer.Serialize(param));
                var results = await _categoryRepository.GetDataAsync(param);

                return new JsonResult(new JqueryDataTablesResult<CategoryViewModel>
                {
                    Draw = param.Draw,
                    Data = results.Items,
                    RecordsFiltered = results.TotalSize,
                    RecordsTotal = results.TotalSize
                });
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return new JsonResult(new { error = "Internal Server Error" });
            }
        }

        public async Task<IActionResult> GetExcel()
        {
            var param = HttpContext.Session.GetString(nameof(JqueryDataTablesParameters));

            var results = await _categoryRepository.GetDataAsync(JsonSerializer.Deserialize<JqueryDataTablesParameters>(param));
            return new JqueryDataTablesExcelResult<CategoryViewModel>(results.Items, "Category", "Category Report");
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryViewModel item)
        {
            var duplicateName = await _categoryRepository.CheckDuplicateName(item.Name, item.Id);

            if (duplicateName > 0) {

                Response.StatusCode = 400;
                return Json(new { error = $"Name is Already Exist" });
            }

            await _categoryRepository.CreateDataAsync(item);
            return NoContent();

        }

        public async Task<ActionResult> Edit(int id)
        {
            var item = await _categoryRepository.GetDataByIdAsync(id);

            return PartialView("_Edit", item);
        }

        [HttpPut]
        public async Task<ActionResult> Edit(CategoryViewModel item)
        {
            var duplicateName = await _categoryRepository.CheckDuplicateName(item.Name, item.Id);

            if (duplicateName > 0)
            {
                Response.StatusCode = 400;
                return Json(new { error = $"Name is Already Exist" });
            }

            await _categoryRepository.UpdateDataAsync(item);

            return NoContent();
        }

        [HttpDelete]
        public async Task<NoContentResult> Delete(int id)
        {
            await _categoryRepository.DeleteDataAsync(id);

            return NoContent();
        }

        [Obsolete]
        public FileResult GetUplodFormat()
        {
            var fileName = "Category-Upload-Formate.xlsx";

            string path = Path.Combine(_environment.WebRootPath, "Files/") + fileName;

            byte[] bytes = System.IO.File.ReadAllBytes(path);

            return File(bytes, "application/octet-stream", fileName);
        }

        [HttpGet]
        public  ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        [Obsolete]
        public async Task<ActionResult> Upload(IFormFile file)
        {
            try
            {
                var stream = file!.OpenReadStream();
                List<CategoryViewModel> categoryViewList = new List<CategoryViewModel>();
                var errors = new List<string>();

                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    var rowCount = worksheet!.Dimension.Rows;
                    for (var row = 2; row <= rowCount; row++)
                    {

                        var name = worksheet.Cells[row, 1].Value?.ToString();

                        if (name != null)
                        {
                            var categoryView = new CategoryViewModel
                            {
                                Name = name.Trim()
                            };

                            categoryViewList.Add(categoryView);
                        }
                    }
                }

                if (!categoryViewList.Any())
                {
                    ViewBag.Error = $"No Data Found";
                    return View();
                }

                var result = await _categoryRepository.BalkUpload(categoryViewList);

                if (result)
                {
                    ViewBag.Error = $"Successfully File Uploaded.";

                    try
                    {
                        string path = Path.Combine(_environment.WebRootPath, "Files/UploadedFiles/") + file.FileName;
                        using var a = System.IO.File.Create(path);
                        file.CopyTo(a);
                    }
                    catch (Exception)
                    {

                    }

                    return View();
                }

                ViewBag.Error = $"Something Went Wrong.";
                return View();
            }

            catch (Exception ex)
            {
                ViewBag.Error = $"Something Went Wrong.";
                return View();
            }
        }
    }
}
