using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using DemoPatients.Data;
using DemoPatients.Models;
using DemoPatients.WebApp.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DemoPatients.Tests
{
    [TestClass]
    public class ControllerAuthorizeTest
    {
        private Mock<IPatientRepository> _repo;
        private PatientController _controller;
        private Patient _patient;
        private Mock<ControllerContext> _context;
        private Mock<HttpSessionStateBase> _session;

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

        [TestMethod]
        public void Controller_DoesntNeedAuthorize()
        {
            Type controllerType = _controller.GetType();

            Assert.IsFalse(TypeContainsAuthorizeAttribute(controllerType));
        }

        [TestMethod]
        public void Controller_CreateNeedsAuthorize()
        {
            Type controllerType = _controller.GetType();

            Assert.IsTrue(MethodContainsAuthorizeAttribute(controllerType, "Create", Type.EmptyTypes));
        }
        

        [TestMethod]
        public void Controller_Can_BlockCreationIfNotAdmin()
        {
            Type controllerType = _controller.GetType();

            Assert.IsTrue(MethodOrTypeContainsAuthorizeRoles(controllerType, "Create", Type.EmptyTypes, "Superviseur"));
        }

        private static bool MethodOrTypeContainsAuthorizeRoles(Type controllerType, string methodName, Type[] methodSignature, params string[] roles)
        {
            if (controllerType.GetCustomAttributes(typeof(AuthorizeAttribute))
                .Any(a => roles.All(r => (a as AuthorizeAttribute).Roles.Contains(r))))
                return true;

            var methodInfo = controllerType.GetMethod(methodName, methodSignature);

            return methodInfo.GetCustomAttributes(typeof(AuthorizeAttribute))
                .Any(a => roles.All(r => (a as AuthorizeAttribute).Roles.Contains(r)));
        }

        private static bool TypeContainsAuthorizeAttribute(Type controllerType)
        {
            return controllerType.GetCustomAttributes(typeof(AuthorizeAttribute))
                .Any();
        }

        private bool MethodContainsAuthorizeAttribute(Type controllerType, string methodName, Type[] methodSignature)
        {
            if (TypeContainsAuthorizeAttribute(controllerType)) return true;

            var methodInfo = controllerType.GetMethod(methodName, methodSignature);

            return methodInfo.GetCustomAttributes(typeof(AuthorizeAttribute))
                .Any();
        }
    }
}
