using EFiscal.JWT.AuthServer.Models.Security;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFiscal.JWT.AuthServer.Data
{
    public class SecurityDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        public SecurityDbContext(DbContextOptions<SecurityDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().HasKey(e => e.Id);
            builder.Entity<User>().HasIndex(e => new { e.Id } ).IsUnique(true);
            builder.Entity<User>().HasIndex(e => new { e.NormalizedName } ).IsUnique(true);
            builder.Entity<User>().HasIndex(e => new { e.NormalizedEmail } ).IsUnique(true);

            builder.Entity<Role>().HasKey(e => e.Id);
            builder.Entity<Role>().HasIndex(e => new { e.NormalizedName }).IsUnique(true);

            builder.Entity<UserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });


        }
    }
}
