using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFiscal.JWT.AuthServer.Common.Data
{
    public enum ServiceFindOrderBy
    {
        None = 0,
        Asc = 0 << 1,
        Desc = 1 << 1,
    };

}
