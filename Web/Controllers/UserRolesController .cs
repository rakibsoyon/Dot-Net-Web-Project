using DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Web.Utility.Constant;

namespace Web.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class UserRolesController : Controller
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserRolesController(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        //list of roles
        public IActionResult Index()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ApplicationRole model)
        {
            
            if (!_roleManager.RoleExistsAsync(model.Name!).GetAwaiter().GetResult())
            {
                 _roleManager.CreateAsync(new ApplicationRole(model.Name!,model.Description)).GetAwaiter().GetResult();
            }
            return RedirectToAction("Index");
        }
    }
}
