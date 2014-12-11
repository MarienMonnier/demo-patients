using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Helpers;
using System.Web.Mvc;
using DemoPatients.Data;
using DemoPatients.Models;
using DemoPatients.WebApp.Controllers;
using DemoPatients.WebApp.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DemoPatients.Tests
{
    [TestClass]
    public class ControllerTests
    {
        private PatientController _controller;
        private Mock<IPatientRepository> _repo;
        private Patient _patient;

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
            bool data = (bool) GetReflectedProperty(result.Data, "success");
            Assert.IsFalse(data);
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
