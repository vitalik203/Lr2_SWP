using lr1_Zaluskii.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace lr1_Zaluskii.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Admin/Index — список усіх користувачів
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var model = new List<UserRoleViewModel>();

            foreach (var user in users)
            {
                model.Add(new UserRoleViewModel
                {
                    UserId = user.Id,
                    Email = user.Email ?? string.Empty,
                    Roles = await _userManager.GetRolesAsync(user)
                });
            }

            return View(model);
        }

        // GET: Admin/EditRoles/userId
        [HttpGet]
        public async Task<IActionResult> EditRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.ToList();

            var model = new ChangeRoleViewModel
            {
                UserId = user.Id,
                Email = user.Email ?? string.Empty,
                AllRoles = allRoles.Select(r => new RoleCheckbox
                {
                    RoleName = r.Name ?? string.Empty,
                    IsSelected = userRoles.Contains(r.Name ?? string.Empty)
                }).ToList()
            };

            return View(model);
        }

        // POST: Admin/EditRoles
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRoles(ChangeRoleViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            var selectedRoles = model.AllRoles
                .Where(r => r.IsSelected)
                .Select(r => r.RoleName);

            await _userManager.AddToRolesAsync(user, selectedRoles);

            TempData["Success"] = $"Ролі користувача {user.Email} оновлено.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/DeleteUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.Id == userId)
            {
                TempData["Error"] = "Ви не можете видалити власний акаунт.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            await _userManager.DeleteAsync(user);
            TempData["Success"] = $"Користувача {user.Email} видалено.";
            return RedirectToAction(nameof(Index));
        }
    }
}
