using EFiscal.Common.Data;
using EFiscal.JWT.AuthServer.Common.Data;
using EFiscal.JWT.AuthServer.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFiscal.JWT.AuthServer.Data.Stores
{
    public class RoleStore : BaseStore<Role, string, SecurityDbContext>
    {
        public RoleStore(SecurityDbContext context) : base(context) { }
    }
}
