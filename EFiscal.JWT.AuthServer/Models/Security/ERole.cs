using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFiscal.JWT.AuthServer.Models.Security
{
    //TODO Clasificar los roles necesarioa aca
    public enum ERole
    {
        Fiscal = 0b0000_0000_0001,
        SimpleAdmin = 0b0000_0000_0010,
        SiteAdmin = 0b0000_0000_1000,
        Administrator = 0b0000_1000_0000
    }

}
