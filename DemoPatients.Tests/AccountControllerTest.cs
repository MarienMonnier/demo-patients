using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using DemoPatients.WebApp.Controllers;
using DemoPatients.WebApp.Models;
using Microsoft.Owin.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DemoPatients.Tests
{
    [TestClass]
    public class AccountControllerTest
    {
        private StaticUserStore _su;
        private StaticUserManager _am;
        private AccountController _ac;
        private LoginViewModel _loginModel;
        private Mock<ControllerContext> _context;
        private Mock<HttpContextBase> _contextBase;

        [TestInitialize]
        public void Init()
        {
            _su = new StaticUserStore();
            _am = new StaticUserManager(_su);
            _ac = new AccountController(_am);
            _context = new Mock<ControllerContext>();
            _contextBase = new Mock<HttpContextBase>();

            _context.SetupGet(c => c.HttpContext).Returns(_contextBase.Object);

            _ac.ControllerContext = _context.Object;

            _loginModel = new LoginViewModel()
            {
                UserName = "test",
                Password = "test",
                RememberMe = false
            };
        }

        [TestMethod]
        public void AccountController_Can_RedirectOnModelStateInvalid()
        {
            _ac.ModelState.AddModelError("testError", new Exception());

            var view = _ac.Login(_loginModel, "").Result as ViewResult;

            Assert.AreEqual(_loginModel, view.Model);
        }

        [TestMethod]
        public void AccountController_Can_RedirectLoginInvalid()
        {
            var view = _ac.Login(_loginModel, "").Result as ViewResult;

            Assert.AreEqual(_loginModel, view.Model);
        }

        [TestMethod]
        public void AccountController_Can_RedirectLoginValid()
        {
            var aman = new Mock<IAuthenticationManager>();

            var contr = new AccountController(_am, aman.Object);
            contr.Url = new UrlHelper(new RequestContext(_contextBase.Object, new RouteData()),
                new RouteCollection());

            _loginModel.UserName = "Admin";

            var redirect = contr.Login(_loginModel, "Patient/Index").Result as RedirectToRouteResult;

            var t = redirect.RouteValues["action"];
            var y = redirect.RouteValues["controller"];

            Assert.AreEqual(t, "Index");
            Assert.AreEqual(y, "Patient");
            //Assert.AreEqual(_loginModel, view.Model);
        }
    }
}
