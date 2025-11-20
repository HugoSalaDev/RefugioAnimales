using Microsoft.EntityFrameworkCore;
using RefugioAnimales.Models;
using System.Security.Cryptography;
using System.Text;

namespace RefugioAnimales.Data
{
    // Contexto de base de datos - gestiona la conexión y las tablas
    public class RefugioContext : DbContext
    {
        public RefugioContext(DbContextOptions<RefugioContext> options) : base(options)
        {
        }

        // Tablas de la base de datos
        public DbSet<Animal> Animales { get; set; }
        public DbSet<Adoptante> Adoptantes { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar relación Animal-Adoptante
            modelBuilder.Entity<Animal>()
                .HasOne(a => a.Adoptante)
                .WithMany(ad => ad.Animales)
                .HasForeignKey(a => a.AdoptanteId)
                .OnDelete(DeleteBehavior.SetNull); // Si se borra adoptante, el animal queda sin adoptante

            // SEED DE DATOS - Se ejecuta al crear la BD
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // SEED: 2 Adoptantes de ejemplo
            modelBuilder.Entity<Adoptante>().HasData(
                new Adoptante
                {
                    Id = 1,
                    Nombre = "María García",
                    Email = "maria@example.com",
                    Telefono = "666777888",
                    FechaAlta = DateTime.Now.AddMonths(-3)
                },
                new Adoptante
                {
                    Id = 2,
                    Nombre = "Juan Pérez",
                    Email = "juan@example.com",
                    Telefono = "666111222",
                    FechaAlta = DateTime.Now.AddMonths(-1)
                }
            );

            // SEED: 3 Animales de ejemplo (sin foto por ahora, las añadiremos después)
            modelBuilder.Entity<Animal>().HasData(
                new Animal
                {
                    Id = 1,
                    Nombre = "Luna",
                    Especie = "Perro",
                    Edad = 3,
                    Estado = "Disponible",
                    Descripcion = "Energética y cariñosa."
                },
                new Animal
                {
                    Id = 2,
                    Nombre = "Milo",
                    Especie = "Gato",
                    Edad = 2,
                    Estado = "Disponible",
                    Descripcion = "Un gato cariñoso y tranquilo."
                },
                new Animal
                {
                    Id = 3,
                    Nombre = "Bella",
                    Especie = "Perro",
                    Edad = 5,
                    Estado = "Disponible",
                    Descripcion = "Una perra enérgica y leal."
                }
            );

            // SEED: 1 Usuario admin
            // Genero salt y hash para la contraseña "admin123"
            var salt = GenerateSalt();
            var hash = HashPassword("admin123", salt);

            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    NombreUsuario = "admin",
                    PasswordHash = hash,
                    Salt = salt,
                    Rol = "Admin"
                }
            );
        }

        // Métodos auxiliares para generar hash de contraseñas
        private string GenerateSalt()
        {
            byte[] saltBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        private string HashPassword(string password, string salt)
        {
            byte[] saltBytes = Convert.FromBase64String(salt);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] combinedBytes = new byte[saltBytes.Length + passwordBytes.Length];

            Buffer.BlockCopy(saltBytes, 0, combinedBytes, 0, saltBytes.Length);
            Buffer.BlockCopy(passwordBytes, 0, combinedBytes, saltBytes.Length, passwordBytes.Length);

            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(combinedBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}