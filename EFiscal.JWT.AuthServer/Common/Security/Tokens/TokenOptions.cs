﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFiscal.JWT.AuthServer.Common.Security.Tokens
{
    public class TokenOptions
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public long AccessTokenExpiration { get; set; }
        public long RefreshTokenExpiration { get; set; }
    }
}
