using System.Collections.Generic;
using Microsoft.AspNet.Identity;

namespace DemoPatients.WebApp.Models
{
    public class ApplicationUser : IUser<string>
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public List<ApplicationRole> Roles { get; set; }
    }

    public class ApplicationRole : IRole<string>
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}