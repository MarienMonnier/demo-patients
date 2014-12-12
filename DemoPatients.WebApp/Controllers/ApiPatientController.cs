using System.Web.Mvc;
using DemoPatients.Data;
using DemoPatients.Services;

namespace DemoPatients.WebApp.Controllers
{
    [BasicAuthorize]
    public class ApiPatientController : Controller
    {
        private readonly IPatientRepository _patientRepository;

        public ApiPatientController(IPatientRepository repo)
        {
            _patientRepository = repo;
        }

        public ApiPatientController()
        {
            _patientRepository = new PatientRepository();
        }

        // GET: /api/patients
        [HttpGet]
        public JsonResult Patients()
        {
            PatientService service = new PatientService(_patientRepository);
            return Json(service.GetPatients(), JsonRequestBehavior.AllowGet);
        }

        // GET : /api/patient/id
        [HttpGet]
        public JsonResult Patient(int id)
        {
            PatientService service = new PatientService(_patientRepository);
            return Json(service.GetPatientById(id), JsonRequestBehavior.AllowGet);
        }
    }
}