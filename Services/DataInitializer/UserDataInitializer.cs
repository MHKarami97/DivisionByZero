using System;
using System.Linq;
using Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Services.DataInitializer
{
    public class UserDataInitializer : IDataInitializer
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public UserDataInitializer(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void InitializeData()
        {
            if (_userManager.Users.AsNoTracking().Any(p => p.UserName == "mhkarami97")) return;

            var user = new User
            {
                Birthday = DateTime.Now,
                FullName = "محمد حسین کرمی",
                Gender = GenderType.Male,
                UserName = "mhkarami97",
                Email = "mhkarami1997@gmail.com"
            };

            var role = new Role
            {
                Name = "Admin",
                Description = "Admin Role"
            };

            var roleUser = new Role
            {
                Name = "User",
                Description = "User Role"
            };

            _userManager.CreateAsync(user, "123456").GetAwaiter().GetResult();
            _roleManager.CreateAsync(role).GetAwaiter().GetResult();
            _roleManager.CreateAsync(roleUser).GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(user, "Admin").GetAwaiter().GetResult();
        }
    }
}