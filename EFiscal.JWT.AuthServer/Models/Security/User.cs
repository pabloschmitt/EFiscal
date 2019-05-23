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
    public class User : IBaseEntity<string>
    {

        /// <summary>
        /// ClientId es el ID de la Organizacion Principal a la que pertenece el usuario
        /// </summary>
        [Required]
        [DataType(DataType.Text)]
        [StringLength(68)]
        public string ClientId { get; set; } = new Guid().ToString();

        [Required]
        [DataType(DataType.Text)]
        [StringLength(68)]
        public string Id { get; set; } = Guid.NewGuid().ToString();


        private string name;
        [Required]
        [DataType(DataType.Text)]
        [StringLength(32)]
        public string Name
        {
            get { return name; }
            set { name = value; NormalizedName = value; }
        }

        private string normalizedName;
        [Required]
        [DataType(DataType.Text)]
        [StringLength(32)]
        public string NormalizedName
        {
            get { return normalizedName; }
            private set { normalizedName = value.ToLowerInvariant(); }
        }


        private string email;
        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(124)]
        public string Email
        {
            get { return email; }
            set { email = value; NormalizedEmail = value; }
        }
        

        private string normalizedEmail;
        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(124)]
        public string NormalizedEmail
        {
            get { return normalizedEmail; }
            private set { normalizedEmail = value.ToLowerInvariant(); }
        }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(68)]
        public string Password { get; set; }

        public bool IsLocked { get; set; } = false;

        public ICollection<UserRole> UserRoles { get; set; } = new Collection<UserRole>();
    }
}
