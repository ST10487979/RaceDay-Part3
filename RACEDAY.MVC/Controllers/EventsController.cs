using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Attributes;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;

namespace RaceDay.MVC.Controllers
{
    public class EventsController : Controller
    {
        private readonly IApiService _api;
        public EventsController(IApiService api) => _api = api;

        public async Task<IActionResult> Index()
        {
            var events = await _api.GetAsync<List<EventDto>>("/api/events");
            return View(events ?? new List<EventDto>());
        }

        public async Task<IActionResult> Details(int id)
        {
            var evt = await _api.GetAsync<EventDto>($"/api/events/{id}");
            if (evt == null) return NotFound();

            ViewBag.Categories = await _api.GetAsync<List<CategoryDto>>($"/api/categories/event/{id}") ?? new List<CategoryDto>();
            return View(evt);
        }

        [HttpPost]
        [RequireRole("Participant")]
        public async Task<IActionResult> Enrol(int eventId, int categoryId)
        {
            var result = await _api.PostAsync<object>("/api/enrolments", new { EventID = eventId, CategoryID = categoryId });

            if (result == null) TempData["Error"] = "Failed to enrol. Already enrolled?";
            else TempData["Success"] = "Successfully enrolled!";

            return RedirectToAction("Details", new { id = eventId });
        }
    }
}