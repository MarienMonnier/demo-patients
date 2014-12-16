using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using DemoPatients.WebApp.Models;

namespace DemoPatients.WebApp.Controllers
{
    using System;

    [Authorize]
    public class AccountController : Controller
    {
        public AccountController()
            : this(new StaticUserManager(new StaticUserStore()))
        {
        }

        public AccountController(StaticUserManager userManager)
        {
            UserManager = userManager;
        }

        public StaticUserManager UserManager { get; private set; }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    await SignInAsync(user, model.RememberMe);
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Patient");
        }
        
        #region Helpers

        private IAuthenticationManager AuthenticationManager
        {
            get {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, identity);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Patient");
            }
        }

        #endregion
    }

    public class StaticUserManager : UserManager<ApplicationUser>
    {
        public StaticUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public override Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            return Task.Factory.StartNew(() => password == "test");
        }
    }

    public class StaticUserStore : IUserStore<ApplicationUser>, IUserRoleStore<ApplicationUser>
    {
        private static readonly List<ApplicationRole> Roles = new List<ApplicationRole>
                                                              {
                                                                  new ApplicationRole { Id = "R1", Name = "Superviseur"},
                                                                  new ApplicationRole { Id = "R2", Name = "Utilisateur"},
                                                              };

        private static readonly List<ApplicationUser> Utilisateurs = new List<ApplicationUser>
                                                                     {
                                                                         new ApplicationUser { Id = "U1", UserName = "Admin", Roles = new List<ApplicationRole> { Roles[0] } },
                                                                         new ApplicationUser { Id = "U2", UserName = "Client", Roles = new List<ApplicationRole> { Roles[1] } },
                                                                     };

        public void Dispose()
        {
        }

        public Task CreateAsync(ApplicationUser user)
        {
            return Task.Factory.StartNew(() => Utilisateurs.Add(user));
        }

        public Task UpdateAsync(ApplicationUser user)
        {
            return Task.Factory.StartNew(() => Utilisateurs.Single(u => u.Id.Equals(user.Id, StringComparison.InvariantCultureIgnoreCase)).UserName = user.UserName);
        }

        public Task DeleteAsync(ApplicationUser user)
        {
            return Task.Factory.StartNew(() => Utilisateurs.RemoveAll(u => u.Id.Equals(user.Id, StringComparison.InvariantCultureIgnoreCase)));
        }

        public Task<ApplicationUser> FindByIdAsync(string userId)
        {
            return Task.Factory.StartNew(() => Utilisateurs.SingleOrDefault(u => u.Id.Equals(userId, StringComparison.InvariantCultureIgnoreCase)));
        }

        public Task<ApplicationUser> FindByNameAsync(string userName)
        {
            return Task.Factory.StartNew(() => Utilisateurs.SingleOrDefault(u => u.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase)));
        }

        public Task AddToRoleAsync(ApplicationUser user, string roleName)
        {
            return Task.Factory.StartNew(() => user.Roles.Add(Roles.Single(r => r.Name.Equals(roleName, StringComparison.InvariantCultureIgnoreCase))));
        }

        public Task RemoveFromRoleAsync(ApplicationUser user, string roleName)
        {
            return Task.Factory.StartNew(() => user.Roles.RemoveAll(r => r.Name.Equals(roleName, StringComparison.InvariantCultureIgnoreCase)));
        }

        public Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            return Task.Factory.StartNew(() => (IList<string>)user.Roles.Select(r => r.Name).ToList());
        }

        public Task<bool> IsInRoleAsync(ApplicationUser user, string roleName)
        {
            return Task.Factory.StartNew(() => user.Roles.Exists(r => r.Name.Equals(roleName, StringComparison.InvariantCultureIgnoreCase)));
        }
    }
}