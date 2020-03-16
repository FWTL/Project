using System.Threading.Tasks;
using FWTL.Auth.Database.Entities;
using FWTL.Common.Extensions;
using FWTL.Core.Enums;
using Microsoft.AspNetCore.Identity;

namespace FWTL.Auth.Database
{
    public class SeedData
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;

        private Role _userRole;

        public SeedData(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task UpdateAsync()
        {
            await CreateRolesAsync();
            await SeedRolesAsync();
        }

        private async Task SeedRolesAsync()
        {
            if ((await _roleManager.FindByNameAsync(Roles.User.ToString())).IsNull())
            {
                await _roleManager.CreateAsync(_userRole);
            }
        }

        private Task CreateRolesAsync()
        {
            _userRole = new Role { Id = (long)Roles.User, Name = nameof(Roles.User) };
            return Task.CompletedTask;
        }
    }
}