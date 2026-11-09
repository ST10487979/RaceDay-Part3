using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;

namespace RaceDay.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApiService _api;

        public AccountController(ApiService api)
        {
            _api = api;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // Note: No "/api/" at the start because BaseAddress has it!
            var result = await _api.PostAsync<AuthResponse>("auth/login", new { model.Email, model.Password });

            if (result == null)
            {
                ModelState.AddModelError("", "Invalid credentials");
                return View(model);
            }

            // Store in Session
            HttpContext.Session.SetString("UserID", result.UserID.ToString());
            HttpContext.Session.SetString("Role", result.Role);
            HttpContext.Session.SetString("FullName", result.FullName);

            if (result.Role == "Organiser")
            {
                return RedirectToAction("Dashboard", "Organiser");
            }

            return RedirectToAction("Index", "Events");
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _api.PostAsync<AuthResponse>("auth/register", model);

            if (result == null)
            {
                ModelState.AddModelError("", "Registration failed. Email exists or invalid data.");
                return View(model);
            }

            // Auto-login after register
            return await Login(new LoginViewModel { Email = model.Email, Password = model.Password });
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _api.PostNoReturnAsync("auth/logout", new { });
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}