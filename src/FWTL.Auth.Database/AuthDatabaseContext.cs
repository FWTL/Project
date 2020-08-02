using System;
using FWTL.Auth.Database.Configuration;
using FWTL.Domain.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FWTL.Auth.Database
{
    public class AuthDatabaseContext : IdentityDbContext<User, Role, Guid>
    {
        private readonly AuthDatabaseCredentials _credentials;

        public AuthDatabaseContext(AuthDatabaseCredentials credentials)
        {
            _credentials = credentials;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_credentials.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new RoleConfiguration());
            base.OnModelCreating(builder);
        }
    }
}