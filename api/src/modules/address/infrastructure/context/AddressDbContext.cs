using Microsoft.EntityFrameworkCore;

using api.src.modules.address.domain.entities;

namespace api.src.modules.address.infrastructure.context
{
    public class AddressDbContext : DbContext
    {
        public AddressDbContext(DbContextOptions<AddressDbContext> options) : base(options) { }

        public DbSet<Address> Addresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.Cep).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Logradouro).HasMaxLength(200);
                entity.Property(e => e.Number).HasMaxLength(200);
                entity.Property(e => e.Complemento).HasMaxLength(200);
                entity.Property(e => e.Bairro).HasMaxLength(100);
                entity.Property(e => e.Localidade).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Uf).IsRequired().HasMaxLength(2);
            });
        }
    }
}
