using BusinessLogic.Interface;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using Microsoft.AspNetCore.Authorization;
using static Web.Utility.Constant;
using Shared;
using Microsoft.AspNetCore.Mvc.Rendering;
using DocumentFormat.OpenXml.InkML;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using OfficeOpenXml;

namespace Web.Controllers
{
   [Authorize(Roles = $"{Roles.Admin},{Roles.General}")]
    public class ItemController : Controller
    {
        private readonly IItemRepository _itemRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMemoryCache _memoryCache;
        [Obsolete]
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;

        [Obsolete]
        public ItemController(IItemRepository itemRepository, ICategoryRepository categoryRepository,IMemoryCache memoryCache, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment)
        {
            _itemRepository = itemRepository;
            _categoryRepository = categoryRepository;
            _memoryCache = memoryCache;
            _environment = environment;
        }

        // GET: Item

        [HttpGet]
        public async Task<ActionResult> Index([FromQuery] PaginationFilters filters)
        {
            var result = await _itemRepository.GetDataAsync(filters);

            return View(result);
        }


        // GET: Item/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return View();
            }

            var item = await _itemRepository.GetDataByIdAsync(id.Value);
            
            return View(item);
        }

        // GET: Item/Create
        public async Task<IActionResult> Create()
        {
            var categories = await GetAllCategories(); //await _categoryRepository.GetAllCategoriesAsync();
            ViewData["CategoryId"] = new SelectList(categories, "Id", "Name");
            return View(new ItemViewModel());
        }

        // POST: Item/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Unit,Quantity,CategoryId")] ItemViewModel item)
        {
            var categories = await GetAllCategories(); //await _categoryRepository.GetAllCategoriesAsync();
            ViewData["CategoryId"] = new SelectList(categories, "Id", "Name");

            if (ModelState.IsValid)
            {
                var duplicateName = await _itemRepository.CheckDuplicateName(item.Name, item.Id, item.CategoryId);

                if (duplicateName > 0)
                {
                    ViewBag.Error = $"{item.Name} Name is Already Exist";
                    return View(item);
                }


                var result = await _itemRepository.CreateDataAsync(item);
                return RedirectToAction(nameof(Index));
            }

            

            return View(item);
        }


        // GET: Item/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                View();
            }

            var item = await _itemRepository.GetDataByIdAsync(id.Value);

            var categories = await GetAllCategories(); //await _categoryRepository.GetAllCategoriesAsync();
            ViewData["CategoryId"] = new SelectList(categories, "Id", "Name");

            return View(item);
        }

        // POST: Item/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Unit,Quantity,CategoryId")] ItemViewModel item)
        {
            if (id != item.Id)
            {
                return View();
            }

            if (await _itemRepository.GetDataByIdAsync(id) is null)
            {
                return View();
            }

            var categories = await GetAllCategories(); // await _categoryRepository.GetAllCategoriesAsync();
            ViewData["CategoryId"] = new SelectList(categories, "Id", "Name");

            if (ModelState.IsValid)
            {
                var duplicateName = await _itemRepository.CheckDuplicateName(item.Name, item.Id, item.CategoryId);

                if (duplicateName > 0)
                {
                    ViewBag.Error = $"{item.Name} Name is Already Exist";
                    return View(item);
                }

                var result = await _itemRepository.UpdateDataAsync(item);
                
               return RedirectToAction(nameof(Index));
            }

            return View(item);
        }

        // POST: Item/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _itemRepository.GetDataByIdAsync(id);

            if (item is null)
            {
                return View();
            }

            var result = await _itemRepository.DeleteDataAsync(id);

            return RedirectToAction(nameof(Index));
        }


        private async Task<List<CategoryViewModel>> GetAllCategories() {
            string key = "_CategoryList";

            try
            {
               var cachedData = GetValueFromCache(key);
                
                if (cachedData != null) {
                    return cachedData;
                }
            }
            catch (Exception)
            {
               
            }

            var result = await _categoryRepository.GetAllCategoriesAsync();
            SetValueToCache(key, result);
            return result;

        }


        private void SetValueToCache(string key, List<CategoryViewModel> data)
        {

            var expiration = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(1),
                SlidingExpiration = TimeSpan.FromMinutes(1),
                Priority = CacheItemPriority.High
            };

            _memoryCache.Set(key, data, expiration);
        }


        private List<CategoryViewModel> GetValueFromCache(string key)
        {
            var value = new List<CategoryViewModel>();
            _memoryCache.TryGetValue(key, out value);
            return value;
        }


        [Obsolete]
        public FileResult GetUplodFormat()
        {
            var fileName = "Item-Upload-Formate.xlsx";

            string path = Path.Combine(_environment.WebRootPath, "Files/") + fileName;

            byte[] bytes = System.IO.File.ReadAllBytes(path);

            return File(bytes, "application/octet-stream", fileName);
        }

        [HttpGet]
        public async Task<ActionResult> Upload()
        {
            var categories = await GetAllCategories(); //await _categoryRepository.GetAllCategoriesAsync();
            ViewData["CategoryId"] = new SelectList(categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        [Obsolete]
        public async Task<ActionResult> Upload(ItemUploadModel model)
        {
            var categories = await GetAllCategories(); 

            ViewData["CategoryId"] = new SelectList(categories, "Id", "Name");
            
            try
            {
                var stream = model.File!.OpenReadStream();
                List<ItemViewModel> itemViewList = new List<ItemViewModel>();
                var errors = new List<string>();

                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    var rowCount = worksheet!.Dimension.Rows;
                    for (var row = 2; row <= rowCount; row++)
                    {

                        var name = worksheet.Cells[row, 1].Value?.ToString();
                        var unit = Convert.ToInt32(worksheet.Cells[row, 2].Value != null ? worksheet.Cells[row, 2].Value.ToString() : 0);
                        var qantity = Convert.ToInt32(worksheet.Cells[row, 3].Value != null ? worksheet.Cells[row, 3].Value.ToString() : 0);

                        if (name != null)
                        {
                            var categoryView = new ItemViewModel
                            {
                                Name = name.Trim(),
                                Unit = unit,
                                Quantity = qantity,
                                CategoryId = model.CategoryId

                            };

                            itemViewList.Add(categoryView);
                        }
                    }
                }

                if (!itemViewList.Any())
                {
                    ViewBag.Error = $"No Data Found";
                    return View();
                }

                var result = await _itemRepository.BalkUpload(itemViewList);

                if (result)
                {
                    ViewBag.Error = $"Successfully File Uploaded.";

                    try
                    {
                        string path = Path.Combine(_environment.WebRootPath, "Files/UploadedFiles/") + model.File.FileName;
                        using var a = System.IO.File.Create(path);
                        model.File.CopyTo(a);
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

        [HttpGet]
        public async Task<ActionResult> ImageUpload(int? id)
        {
            if (id == null)
            {
                View();
            }

            return View();
        }

        [HttpPost]
        [Obsolete]
        public async Task<ActionResult> ImageUpload(int? id, ItemImagesUploadModel model) //List<IFormFile> Files
        {
            
            try
            {
                if (model.Files.Count() > 0) {

                    foreach (var file in model.Files)
                    {
                        try
                        {
                            string path = Path.Combine(_environment.WebRootPath, "Files/UploadedFiles/") + file.FileName;
                            using var stream = System.IO.File.Create(path);
                            file.CopyTo(stream);
                        }
                        catch (Exception)
                        {
                            
                        }
                       
                    }

                    ViewBag.Error = $"Successfully File Uploaded.";
                    return View();
                }

                else
                {
                    ViewBag.Error = $"No File Found";
                    return View();

                }

            }

            catch (Exception ex)
            {
                ViewBag.Error = $"Something Went Wrong.";
                return View();
            }
        }

    }
}
