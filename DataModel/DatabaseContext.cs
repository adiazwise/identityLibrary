using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace IdentityLibrary.DataModel
{
    public partial class DatabaseContext : DbContext
    {
        public DatabaseContext()
            : base("name=DefaultConnection")
        {
        }

        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<UserClaims> UserClaims { get; set; }
        public virtual DbSet<UserLogins> UserLogins { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Roles>()
                .HasMany(e => e.Users)
                .WithMany(e => e.Roles)
                .Map(m => m.ToTable("UserRoles").MapLeftKey("RoleId").MapRightKey("UserId"));

            modelBuilder.Entity<Users>()
                .HasMany(e => e.UserClaims)
                .WithRequired(e => e.Users)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.UserLogins)
                .WithRequired(e => e.Users)
                .HasForeignKey(e => e.UserId);
        }
    }
}
