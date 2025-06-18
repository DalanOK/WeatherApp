using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WeatherApp.Models;
using WeatherApp.ViewModels;

namespace WeatherApp.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<Users> _userManager;
        public ProfileController(UserManager<Users> userManager)
            => _userManager = userManager;

        // GET: /Profile
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = new ProfileViewModel
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ReceiveNotifications = user.ReceiveNotifications
            };
            return View(model);
        }

        // POST: /Profile
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // Обновляем только изменившиеся поля
            if (user.Email != model.Email)
                user.Email = model.Email;
            if (user.PhoneNumber != model.PhoneNumber)
                user.PhoneNumber = model.PhoneNumber;
            user.ReceiveNotifications = model.ReceiveNotifications;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError("", e.Description);
                return View(model);
            }

            ViewBag.Message = "Профиль успешно сохранён";
            return View(model);
        }
    }
}
