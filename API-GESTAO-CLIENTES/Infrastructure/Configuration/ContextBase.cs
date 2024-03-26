using Entities.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Configuration
{
    public class ContextBase : IdentityDbContext<ApplicationUser>
    {
        public ContextBase(DbContextOptions<ContextBase> options) : base(options)
        {
        }

        public DbSet<Cliente> Cliente { get; set; }
        public DbSet<Endereco> Endereco { get; set; }
        public DbSet<Contato> Contato { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(ObterStringConexao());
                base.OnConfiguring(optionsBuilder);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Cliente
            modelBuilder.Entity<Cliente>().ToTable("Cliente").HasKey(m => m.Id);
            modelBuilder.Entity<Cliente>()
                .HasOne(m => m.Endereco)
                .WithOne(m => m.Cliente)
                .HasForeignKey<Endereco>(m => m.ClienteId)
                .IsRequired();
            modelBuilder.Entity<Cliente>()
                .HasMany(m => m.Contatos)
                .WithOne(m => m.Cliente)
                .HasForeignKey(m => m.ClienteId);
            #endregion

            #region Endereco
            modelBuilder.Entity<Endereco>().ToTable("Endereco").HasKey(m => m.Id);
            modelBuilder.Entity<Endereco>()
                .HasOne(m => m.Cliente)
                .WithOne(m => m.Endereco)
                .HasForeignKey<Endereco>(m => m.ClienteId)
                .IsRequired();

            #endregion

            #region Contato
            modelBuilder.Entity<Contato>().ToTable("Contato").HasKey(m => m.Id);
            modelBuilder.Entity<Contato>()
                .HasOne(m => m.Cliente)
                .WithMany(m => m.Contatos)
                .HasForeignKey(m => m.ClienteId);
            #endregion          

            base.OnModelCreating(modelBuilder);
        }

        public string ObterStringConexao()
        {
            return "Data Source=DESKTOP-27P7RL9\\SQLEXPRESS;Initial Catalog=GESTAO-CLIENTES;Integrated Security=False;User ID=sa;Password=admin123;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False";
        }
    }
}
