using Microsoft.AspNetCore.Mvc;
using PermitApplication.Models;
using PermitApplication.Services;
using System.Text.Json;

namespace PermitApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly UspsService _usps;

        public HomeController(UspsService usps)
        {
            _usps = usps;
        }

        // Show the form
        [HttpGet]
        public IActionResult Index()
        {
            return View(new PermitFormViewModel());
        }

        // User clicks "Next Step"
        [HttpPost]
        public async Task<IActionResult> Index(PermitFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            var address = new Address(model.Street, model.City, model.Zip)
            {
                State = model.State
            };

            var validated = await _usps.ValidateAddress(address);

            if (validated == null)
            {
                ModelState.AddModelError("", "USPS could not validate the address. Please check it and try again.");
                return View("Index", model);
            }

            // Apply USPS corrections
            model.Street = validated.Street;
            model.City = validated.City;
            model.State = validated.State;
            model.Zip = validated.Zip;

            // Save model for the Review page
            TempData["PermitModel"] = JsonSerializer.Serialize(model);

            return View("~/Views/Permit/Review.cshtml", model);
        }

        // shows the review page
        [HttpGet]
        public IActionResult Review()
        {
            if (TempData["PermitModel"] is not string json)
                return RedirectToAction(nameof(Index));

            var model = JsonSerializer.Deserialize<PermitFormViewModel>(json)!;

            return View(model);
        }
    }
}