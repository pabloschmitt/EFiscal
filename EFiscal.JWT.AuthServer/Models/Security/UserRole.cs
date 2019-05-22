using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EFiscal.JWT.AuthServer.Models.Security
{
    [Table("UserRoles")]
    public class UserRole
    {
        [DataType(DataType.Text)]
        [StringLength(68)]
        public string UserId { get; set; }
        public User User { get; set; }

        [DataType(DataType.Text)]
        [StringLength(68)]
        public string RoleId { get; set; }

        public Role Role { get; set; }
    }
}
