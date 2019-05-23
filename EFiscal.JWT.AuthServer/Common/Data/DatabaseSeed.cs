using EFiscal.JWT.AuthServer.Common.Security;
using EFiscal.JWT.AuthServer.Data;
using EFiscal.JWT.AuthServer.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFiscal.JWT.AuthServer.Common.Data
{
    public class DatabaseSeed
    {
        public static void Seed(SecurityDbContext context, IPasswordHasher passwordHasher)
        {
            context.Database.EnsureCreated();

            if (context.Roles.Count() == 0)
            {

                var roles = new List<Role>
                {
                new Role { Name = ERole.Fiscal.ToString() },
                new Role { Name = ERole.SimpleAdmin.ToString() },
                new Role { Name = ERole.SiteAdmin.ToString() },
                new Role { Name = ERole.Administrator.ToString() }
                };

                context.Roles.AddRange(roles);
                context.SaveChanges();
            }

            if (context.Users.Count() == 0)
            {
                var users = new List<User>
                {
                    new User { Name="usuario_1", Email = "usuario_1@algo.com", Password = passwordHasher.HashPassword("12345678") },
                    new User { Name="usuario_2", Email = "usuario_2@algo.com", Password = passwordHasher.HashPassword("12345678") },
                    new User { Name="usuario_3", Email = "usuario_3@algo.com", Password = passwordHasher.HashPassword("12345678") },
                };

                users[0].UserRoles.Add(new UserRole
                {
                    RoleId = context.Roles.SingleOrDefault(r => r.Name == ERole.Administrator.ToString()).Id
                });

                users[1].UserRoles.Add(new UserRole
                {
                    RoleId = context.Roles.SingleOrDefault(r => r.Name == ERole.Fiscal.ToString()).Id
                });

                context.Users.AddRange(users);
                context.SaveChanges();
            }
        }

    }
}
