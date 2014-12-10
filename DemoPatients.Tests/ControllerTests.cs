using System;
using System.Web.Mvc;
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

        [TestInitialize]
        public void Init()
        {
            _repo = new Mock<IPatientRepository>();
            _controller = new PatientController(_repo.Object);
        }

        [TestMethod]
        public void Controller_Can_CreateOKRedirectToIndex()
        {
            var patient = GetPatient();

            var redirect = _controller.Create(new PatientViewModel(patient)) as RedirectToRouteResult;

            if (redirect != null) Assert.AreEqual("Index", redirect.RouteValues["action"]);
            else Assert.Fail("redirect n'est pas au format RedirectToRouteResult");
        }

        [TestMethod]
        public void Controller_Can_CreateKORedirectToEdit()
        {
            _repo.Setup(r => r.AddPatient(It.IsAny<Patient>())).Throws(new Exception());

            var patient = GetPatient();

            var redirect = _controller.Create(new PatientViewModel(patient)) as ViewResult;

            if (redirect != null) Assert.AreEqual("Create", redirect.ViewName);
            else Assert.Fail("redirect n'est pas au format ViewResult");
        }

        [TestMethod]
        public void Controller_Can_EditOKRedirectToViewEdit()
        {
            var p = GetPatient();
            var vm = new PatientViewModel(p);

            _repo.Setup(r => r.GetPatientById(It.IsAny<int>())).Returns(p);

            var view = _controller.Edit(5) as ViewResult;

            if (view != null) Assert.AreEqual(vm.Nom, ((PatientViewModel)view.Model).Nom);
            else Assert.Fail("redirect n'est pas au format ViewResult");
        }

        [TestMethod]
        public void Controller_Can_EditNotFoundRedirectToIndex()
        {
            _repo.Setup(r => r.GetPatientById(It.IsAny<int>())).Returns(null as Patient);

            var route = _controller.Edit(5) as RedirectToRouteResult;

            if (route != null) Assert.AreEqual("Index", route.RouteValues["action"]);
            else Assert.Fail("redirect n'est pas au format RedirectToRouteResult");
        }

        private static Patient GetPatient()
        {
            var patient = new Patient()
            {
                Civilite = Civilite.M,
                DateNaissance = DateTime.Now,
                Nom = "Test",
                Prenom = "TestP",
                Present = false
            };
            return patient;
        }
    }
}
