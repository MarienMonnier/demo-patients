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
    public class PatientController : Controller
    {
        private readonly IPatientRepository _patientRepository;

        public PatientController(IPatientRepository repo)
        {
            _patientRepository = repo;
        }

        public PatientController()
        {
            _patientRepository = new PatientRepository();
        }
        // GET: /Patient/
        public ActionResult Index()
        {
            PatientService service = new PatientService(_patientRepository);
            List<PatientViewModel> model = service.GetPatients().Select(p => new PatientViewModel(p)).ToList();
            return View(model);
        }

        // GET: /Patient/Create
        public ActionResult Create()
        {
            return View("Create", new PatientViewModel());
        }

        // POST: /Patient/Create
        [HttpPost]
        public ActionResult Create(PatientViewModel model)
        {
            try
            {
                PatientService service = new PatientService(_patientRepository);
                Patient patient = model.ToPatient();
                service.AddPatient(patient);
                TempData["SuccessMessage"] = string.Format("Le patient {0} ({1}) a été créé avec succès.", model.NomComplet, patient.Id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = string.Format("Une erreur est survenue lors de la création du patient {0} : {1}.", model.NomComplet, ex.Message);
                return View("Create", model);
            }
        }

        // GET: /Patient/Edit/5
        public ActionResult Edit(int id)
        {
            PatientService service = new PatientService(_patientRepository);
            Patient patient = service.GetPatientById(id);
            if (patient != null)
                return View(new PatientViewModel(patient));

            return RedirectToAction("Index");
        }

        // POST: /Patient/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, PatientViewModel patient)
        {
            try
            {
                PatientService service = new PatientService(_patientRepository);
                service.UpdatePatient(id, patient.ToPatient());
                TempData["SuccessMessage"] = string.Format("Le patient {0} ({1}) a été mis à jour avec succès.", patient.NomComplet, id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = string.Format("Une erreur est survenue lors de la modification du patient {0} ({1}) : {2}", patient.NomComplet, id, ex.Message);
                return View(patient);
            }
        }

        // POST: /Patient/Delete/5
        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                PatientService service = new PatientService(_patientRepository);
                service.RemovePatient(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
    }
}
