using EFiscal.Common.Data;
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

        private string name;
        [Required]
        [DataType(DataType.Text)]
        [StringLength(50)]
        public string Name
        {
            get { return name; }
            set { name = value; NormalizedName = value.ToLowerInvariant(); }
        }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(32)]
        private string normalizedName;

        public string NormalizedName
        {
            get { return normalizedName; }
            private set { normalizedName = value; }
        }

        public ICollection<UserRole> UsersRole { get; set; } = new Collection<UserRole>();
    }

}
