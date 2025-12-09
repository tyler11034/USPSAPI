using Microsoft.AspNetCore.Mvc;
using PermitApplication.Models;
using PermitApplication.Services;

namespace PermitApplication.Controllers
{
    public class PermitController : Controller
    {
        private readonly UspsService _uspsService;

        public PermitController(UspsService uspsService)
        {
            _uspsService = uspsService;
        }

        [HttpGet]
        public IActionResult Review(PermitFormViewModel model)
        {
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Submit(PermitFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Review", model);
            }

            // Map submitted fields to Address model for USPS validation
            var addr = new Address(model.Street, model.City, model.Zip) { State = model.State };

            // Validate address via USPS API
            Address? validated = null;
            try
            {
                validated = await _uspsService.ValidateAddress(addr);
            }
            catch
            {
                validated = null;
            }

            if (validated == null)
            {
                ModelState.AddModelError("Street", "Address could not be validated by USPS.");
                return View("Review", model);
            }

            // save to database
            //var permit = new PermitEntity
            //{
            //    Name = model.Name,
            //    Street = validated.Street,
            //    City = validated.City,
            //    State = validated.State,
            //    Zip = validated.Zip,
            //    County = model.County,
            //    PermitType = model.PermitType,
            //    SubmittedAt = DateTime.UtcNow,
            //    Status = "Submitted"
            //};

            //_db.Permits.Add(permit);
            //await _db.SaveChangesAsync();

            return RedirectToAction("Success");
        }

        public IActionResult Success()
        {
            return View("~/Views/Permit/Success.cshtml");
        }
    }
}
