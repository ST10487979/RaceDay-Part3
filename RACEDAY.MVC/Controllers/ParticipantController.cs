using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Attributes;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;

namespace RaceDay.MVC.Controllers
{
    [RequireRole("Participant")]
    public class ParticipantController : Controller
    {
        private readonly IApiService _api;
        public ParticipantController(IApiService api) => _api = api;

        public async Task<IActionResult> MyEvents() => View(await _api.GetAsync<List<EnrolmentDto>>("/api/enrolments/my") ?? new List<EnrolmentDto>());
        public async Task<IActionResult> MyResults() => View(await _api.GetAsync<List<ResultDto>>("/api/results/my") ?? new List<ResultDto>());
    }
}