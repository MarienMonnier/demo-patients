using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Windows.Forms;
using DemoPatients.Data;
using DemoPatients.Models;
using DemoPatients.WebApp.Controllers;
using DemoPatients.WebApp.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DemoPatients.Tests
{
    [TestClass]
    public class ControllerTests
    {
        private PatientController _controller;
        private Mock<IPatientRepository> _repo;
        private Patient _patient;
        private Mock<ControllerContext> _context;
        private Mock<HttpSessionStateBase> _session;

        // Initialize : Est appelé avant chaque appel à une méthode de test
        [TestInitialize]
        public void Init()
        {
            _repo = new Mock<IPatientRepository>(); // <-- Dummy
            _controller = new PatientController(_repo.Object);
            _patient = new Patient()
            {
                Civilite = Civilite.M,
                DateNaissance = DateTime.Now,
                Nom = "Test",
                Prenom = "TestP",
                Present = false
            };

            _context = new Mock<ControllerContext>();
            _session = new Mock<HttpSessionStateBase>();
            _context.Setup(c => c.HttpContext.Session).Returns(_session.Object);

            _controller.ControllerContext = _context.Object;
        }

        // CleanUp : Est appelé après chaque appel à une méthode de test
        [TestCleanup]
        public void Clean()
        {

        }

        [TestMethod, Ignore]
        public void Controller_Can_CreateOKOnRepositoryNull()
        {
            // Initialisation
            var controller = new PatientController(null);

            // Exécution
            var redirect = controller.Create(new PatientViewModel(_patient)) as RedirectToRouteResult;

            // Vérification
            if (redirect != null) Assert.AreEqual("Index", redirect.RouteValues["action"]);
            else Assert.Fail("redirect n'est pas au format RedirectToRouteResult");
        }

        [TestMethod]
        public void Controller_Can_CreateOKRedirectToIndex()
        {
            // Initialisation

            // Exécution
            var redirect = _controller.Create(new PatientViewModel(_patient)) as RedirectToRouteResult;

            // Vérification
            if (redirect != null) Assert.AreEqual("Index", redirect.RouteValues["action"]);
            else Assert.Fail("redirect n'est pas au format RedirectToRouteResult");
        }

        [TestMethod]
        public void Controller_Can_CreateKORedirectToEdit()
        {
            // Initialisation
            _repo.Setup(r => r.AddPatient(It.IsAny<Patient>())).Throws(new Exception()); // <-- Stub

            // Exécution
            var redirect = _controller.Create(new PatientViewModel(_patient)) as ViewResult;

            // Vérification
            if (redirect != null) Assert.AreEqual("Create", redirect.ViewName);
            else Assert.Fail("redirect n'est pas au format ViewResult");
        }

        [TestMethod]
        public void Controller_Can_EditOKRedirectToViewEdit()
        {
            // Initialisation
            var vm = new PatientViewModel(_patient);

            _repo.Setup(r => r.GetPatientById(It.IsAny<int>())).Returns(_patient);  // <-- Stub

            // Exécution
            var view = _controller.Edit(5) as ViewResult;

            // Vérification
            if (view != null) Assert.AreEqual(vm.Nom, ((PatientViewModel)view.Model).Nom);
            else Assert.Fail("redirect n'est pas au format ViewResult");
        }

        [TestMethod]
        public void Controller_Can_EditNotFoundRedirectToIndex()
        {
            // Initialisation
            _repo.Setup(r => r.GetPatientById(It.IsAny<int>())).Returns(null as Patient);

            // Exécution
            var route = _controller.Edit(5) as RedirectToRouteResult;

            // Vérification
            if (route != null) Assert.AreEqual("Index", route.RouteValues["action"]);
            else Assert.Fail("redirect n'est pas au format RedirectToRouteResult");
        }

        [TestMethod]
        public void Controller_Cannot_DeleteOnFail()
        {
            // Initialisation
            _repo.Setup(r => r.RemovePatient(It.IsAny<int>())).Throws(new Exception());

            // Exécution
            JsonResult result = _controller.Delete(0);

            // Vérification
            // Je n'ai pas trouvé de moyen plus direct pour lire le contenu d'un JsonResult pour l'instant.
            bool data = (bool)GetReflectedProperty(result.Data, "success");
            Assert.IsFalse(data);
        }

        [TestMethod]
        public void Controller_Can_DisplayPatientPresentOnSessionVariable()
        {
            // Initialisation
            _patient.Present = true;
            _repo.Setup(r => r.GetPatients()).Returns(new List<Patient>() { _patient }.AsQueryable());
            _session.SetupGet(s => s[It.IsAny<string>()]).Returns("true");
            _controller.ControllerContext = _context.Object;

            // Exécution
            var t = _controller.Index() as ViewResult;

            //  Vérification
            Assert.IsTrue((t.Model as List<PatientViewModel>).Any());
        }

        [TestMethod]
        public void Controller_Can_DisplayPatientNotPresentOnSessionVariable()
        {
            // Initialisation
            _patient.Present = false;
            _repo.Setup(r => r.GetPatients()).Returns(new List<Patient>() { _patient }.AsQueryable());
            _session.SetupGet(s => s[It.IsAny<string>()]).Returns("true");
            _controller.ControllerContext = _context.Object;

            // Exécution
            var t = _controller.Index() as ViewResult;

            //  Vérification
            Assert.IsFalse((t.Model as List<PatientViewModel>).Any());
        }

        [TestMethod]
        public void Controller_Can_DisplayPatientPresentWithoutSessionVariable()
        {
            // Initialisation
            _patient.Present = true;
            _repo.Setup(r => r.GetPatients()).Returns(new List<Patient>() { _patient }.AsQueryable());

            // Exécution
            var view = _controller.Index() as ViewResult;

            //  Vérification
            Assert.IsTrue((view.Model as List<PatientViewModel>).Any());
        }

        [TestMethod]
        public void Controller_Can_DisplayPatientNotPresentWithoutSessionVariable()
        {
            // Initialisation
            _patient.Present = false;
            _repo.Setup(r => r.GetPatients()).Returns(new List<Patient>() { _patient }.AsQueryable());

            // Exécution
            var t = _controller.Index() as ViewResult;

            //  Vérification
            Assert.IsTrue((t.Model as List<PatientViewModel>).Any());
        }

        [TestMethod]
        public void Controller_Can_InitSessionOnFilter()
        {
            int cc = 0;

            _session.SetupSet(s => s["cacherabsents"] = It.IsAny<object>())
                .Callback(() => cc++);

            var t = _controller.Filter();

            Assert.IsTrue(cc > 0);
        }

        private static object GetReflectedProperty(object obj, string propertyName)
        {
            PropertyInfo property = obj.GetType().GetProperty(propertyName);

            if (property == null)
            {
                return null;
            }

            return property.GetValue(obj, null);
        }
    }
}
