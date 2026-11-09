using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Attributes;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;

namespace RaceDay.MVC.Controllers
{
    [RequireRole("Organiser")]
    public class OrganiserController : Controller
    {
        private readonly IApiService _api;
        public OrganiserController(IApiService api) => _api = api;

        public async Task<IActionResult> Dashboard() => View(await _api.GetAsync<List<EventDto>>("/api/events") ?? new List<EventDto>());
        public async Task<IActionResult> ManageEvents() => View(await _api.GetAsync<List<EventDto>>("/api/events") ?? new List<EventDto>());

        [HttpGet] public IActionResult CreateEvent() => View();

        [HttpPost]
        public async Task<IActionResult> CreateEvent(CreateEventDto model, IFormFile banner)
        {
            if (!ModelState.IsValid) return View(model);

            if (banner != null && banner.Length > 0)
            {
                var url = await _api.UploadFileAsync("/api/blob/upload?container=event-banners", banner);
                if (url != null) model.BannerImageUrl = url;
            }

            var result = await _api.PostAsync<EventDto>("/api/events", model);
            if (result == null) { ModelState.AddModelError("", "Failed to create event."); return View(model); }

            TempData["Success"] = "Event created!";
            return RedirectToAction(nameof(ManageEvents));
        }

        public async Task<IActionResult> ViewEnrolments(int eventId)
        {
            ViewBag.EventId = eventId;
            return View(await _api.GetAsync<List<EnrolmentDto>>($"/api/enrolments/event/{eventId}") ?? new List<EnrolmentDto>());
        }
    }
}
