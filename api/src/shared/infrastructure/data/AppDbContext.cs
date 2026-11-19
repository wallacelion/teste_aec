using Microsoft.EntityFrameworkCore;

using api.src.modules.user.domain.entities;
using api.src.modules.address.domain.entities;

namespace api.src.shared.infrastructure.data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            //modelBuilder.Entity<User>(entity =>
            //{
            //    entity.HasKey(e => e.Id);
            //    entity.Property(e => e.Uid).IsRequired();
            //    entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            //    entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            //    entity.Property(e => e.Password).IsRequired().HasMaxLength(255);
            //    entity.Property(e => e.Role).IsRequired();
            //});

            //modelBuilder.Entity<Address>(entity =>
            //{
            //    entity.HasKey(e => e.Id);
            //    entity.Property(e => e.UserId).IsRequired();
            //    entity.Property(e => e.Cep).IsRequired().HasMaxLength(10);
            //    entity.Property(e => e.Logradouro).HasMaxLength(200);
            //    entity.Property(e => e.Number).HasMaxLength(200);
            //    entity.Property(e => e.Complemento).HasMaxLength(200);
            //    entity.Property(e => e.Bairro).HasMaxLength(100);
            //    entity.Property(e => e.Localidade).IsRequired().HasMaxLength(100);
            //    entity.Property(e => e.Uf).IsRequired().HasMaxLength(2);

            //    entity.HasOne<User>()
            //          .WithMany()
            //          .HasForeignKey(e => e.UserId)
            //          .OnDelete(DeleteBehavior.Cascade);
            //});
        }
    }
}
