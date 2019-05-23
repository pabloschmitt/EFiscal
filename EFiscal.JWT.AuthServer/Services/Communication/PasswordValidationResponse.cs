using EFiscal.JWT.AuthServer.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFiscal.JWT.AuthServer.Services.Communication
{
    public class PasswordValidationResponse : BaseResponse
    {
        public User User { get; private set; }

        public PasswordValidationResponse(bool success, string message, User user) : base(success, message)
        {
            User = user;
        }
    }
}
