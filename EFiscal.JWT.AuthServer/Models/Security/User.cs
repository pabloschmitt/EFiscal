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
        /// OwnerId es el ID de la Organizacion Principal a la que pertenece el usuario
        /// </summary>
        [Required]
        [DataType(DataType.Text)]
        [StringLength(68)]
        public string OwnerId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(68)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [DataType(DataType.Text)]
        [StringLength(32)]
        public string Name { get => Name; set { Name = value; NormalizedName = value; } }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(32)]
        public string NormalizedName { get => NormalizedName; private set => NormalizedName = value.ToLowerInvariant(); }

        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(124)]
        public string Email { get => Email; set { Email = value; NormalizedEmail = value; } }

        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(124)]
        public string NormalizedEmail { get => NormalizedEmail; private set => NormalizedEmail = value.ToLowerInvariant(); }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(68)]
        public string Password { get; set; }

        public bool IsLocked { get; set; } = false;

        public ICollection<UserRole> UserRoles { get; set; } = new Collection<UserRole>();
    }
}
