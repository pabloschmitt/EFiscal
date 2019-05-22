using EFiscal.JWT.AuthServer.Common.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EFiscal.JWT.AuthServer.Models.Security
{
    public class Role : IBaseEntity<string>
    {
        [Required]
        [DataType(DataType.Text)]
        [StringLength(68)]
        public string Id { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50)]
        public string Name { get => Name; set { Name = value; NormalizedName = value; } }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(32)]
        public string NormalizedName { get => NormalizedName; private set => NormalizedName = value.ToLowerInvariant(); }

        public ICollection<UserRole> UsersRole { get; set; } = new Collection<UserRole>();
    }

}
