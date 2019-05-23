using EFiscal.Common.Data;
using EFiscal.JWT.AuthServer.Common.Data;
using EFiscal.JWT.AuthServer.Models.Security;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFiscal.JWT.AuthServer.Data.Stores
{
    public class UserStore : BaseStore<User, string, SecurityDbContext>
    {
        public UserStore(SecurityDbContext context ) : base(context) { }


        public async Task AddUserAsync(User user, ERole[] userRoles)
        {
            var roles = await Context.Roles.Where(r => userRoles.Any(ur => ur.ToString() == r.Name))
                                            .ToListAsync();

            foreach (var role in roles)
            {
                user.UserRoles.Add(new UserRole { RoleId = role.Id });
            }

            Context.Users.Add(user);
            Context.SaveChanges();
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            return await Context.Users.Include(u => u.UserRoles)
                                       .ThenInclude(ur => ur.Role)
                                       .SingleOrDefaultAsync(u => u.NormalizedEmail == email.ToLowerInvariant());
        }

        public async Task<User> FindByNameAsync(string name)
        {
            return await Context.Users.Include(u => u.UserRoles)
                                       .ThenInclude(ur => ur.Role)
                                       .SingleOrDefaultAsync(u => u.NormalizedName == name.ToLowerInvariant());
        }

        public async Task<User> FindByIdAsync(string id)
        {
            return await Context.Users.Include(u => u.UserRoles)
                                       .ThenInclude(ur => ur.Role)
                                       .SingleOrDefaultAsync(u => u.Id == id);
        }

    }
}
