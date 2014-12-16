using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DemoPatients.Data;
using DemoPatients.Models;
using DemoPatients.Services;
using DemoPatients.WebApp.Models;

namespace DemoPatients.WebApp.Controllers
{
    //[Authorize(Roles = "Administrator")]
    public class PatientController : Controller
    {
        // GET: /Patient/
        public ActionResult Index()
        {
            List<PatientViewModel> model = GetPatients();

            return View(model);
        }

        private List<PatientViewModel> GetPatients()
        {
            PatientService service = new PatientService(new PatientRepository());

            bool? optimize = (bool?)HttpContext.Session["cacherabsents"];

            List<PatientViewModel> model = service.GetPatients().Select(p => new PatientViewModel(p)).ToList();

            if (optimize.GetValueOrDefault(false))
            {
                model = model.Where(m => m.Present).ToList();
            }
            return model;
        }

        // GET: /Patient/Create
        [Authorize(Roles = "Superviseur")]
        public ActionResult Create()
        {
            return View("Create", new PatientViewModel());
        }

        // POST: /Patient/Create
        [HttpPost]
        [Authorize(Roles = "Superviseur")]
        public ActionResult Create(PatientViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    PatientService service = new PatientService(new PatientRepository());
                    Patient patient = model.ToPatient();
                    service.AddPatient(patient);
                    TempData["SuccessMessage"] = string.Format("Le patient {0} ({1}) a été créé avec succès.", model.NomComplet, patient.Id);
                    return RedirectToAction("Index");
                }
                
                TempData["ErrorMessage"] = "Veuillez remplir les champs obligatoires.";
                return View("Create", model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = string.Format("Une erreur est survenue lors de la création du patient {0} : {1}.", model.NomComplet, ex.Message);
                return View("Create", model);
            }
        }

        // GET: /Patient/Edit/5
        [Authorize(Roles = "Superviseur")]
        public ActionResult Edit(int id)
        {
            PatientService service = new PatientService(new PatientRepository());
            Patient patient = service.GetPatientById(id);
            if (patient != null)
                return View(new PatientViewModel(patient));

            return RedirectToAction("Index");
        }

        // POST: /Patient/Edit/5
        [HttpPost]
        [Authorize(Roles = "Superviseur")]
        public ActionResult Edit(int id, PatientViewModel patient)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    PatientService service = new PatientService(new PatientRepository());
                    service.UpdatePatient(id, patient.ToPatient());
                    TempData["SuccessMessage"] = string.Format("Le patient {0} ({1}) a été mis à jour avec succès.", patient.NomComplet, id);
                    return RedirectToAction("Index");
                }

                TempData["ErrorMessage"] = "Veuillez remplir les champs obligatoires.";
                return View(patient);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = string.Format("Une erreur est survenue lors de la modification du patient {0} ({1}) : {2}", patient.NomComplet, id, ex.Message);
                return View(patient);
            }
        }

        // POST: /Patient/Delete/5
        [HttpPost]
        [Authorize(Roles = "Superviseur")]
        public JsonResult Delete(int id)
        {
            try
            {
                PatientService service = new PatientService(new PatientRepository());
                service.RemovePatient(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        public ContentResult Filter()
        {
            if (Session["cacherabsents"] == null)
                Session["cacherabsents"] = true;
            else
                Session["cacherabsents"] = !(bool)Session["cacherabsents"];

            return Content("OK");
        }

        public PartialViewResult List()
        {
            List<PatientViewModel> patients = GetPatients();
            return PartialView("_PatientList", patients);
        }
    }
}
