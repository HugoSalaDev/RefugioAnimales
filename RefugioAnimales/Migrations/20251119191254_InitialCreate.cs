using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RefugioAnimales.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Adoptantes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    FechaAlta = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adoptantes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreUsuario = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Salt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Animales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Especie = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Edad = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FotoContenido = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    FotoMimeType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdoptanteId = table.Column<int>(type: "int", nullable: true),
                    FechaAdopcion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Animales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Animales_Adoptantes_AdoptanteId",
                        column: x => x.AdoptanteId,
                        principalTable: "Adoptantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "Adoptantes",
                columns: new[] { "Id", "Email", "FechaAlta", "Nombre", "Telefono" },
                values: new object[,]
                {
                    { 1, "maria@example.com", new DateTime(2025, 8, 19, 20, 12, 50, 891, DateTimeKind.Local).AddTicks(7574), "María García", "666777888" },
                    { 2, "juan@example.com", new DateTime(2025, 10, 19, 20, 12, 50, 891, DateTimeKind.Local).AddTicks(7581), "Juan Pérez", "666111222" }
                });

            migrationBuilder.InsertData(
                table: "Animales",
                columns: new[] { "Id", "AdoptanteId", "Descripcion", "Edad", "Especie", "Estado", "FechaAdopcion", "FotoContenido", "FotoMimeType", "Nombre" },
                values: new object[,]
                {
                    { 1, null, "Energética y cariñosa.", 3, "Perro", "Disponible", null, null, null, "Luna" },
                    { 2, null, "Un gato cariñoso y tranquilo.", 2, "Gato", "Disponible", null, null, null, "Milo" },
                    { 3, null, "Una perra enérgica y leal.", 5, "Perro", "Disponible", null, null, null, "Bella" }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "NombreUsuario", "PasswordHash", "Rol", "Salt" },
                values: new object[] { 1, "admin", "Z2YeM5fJHwon8tdLGVTVwzJFH2Wv9jJ2vVWPu+0VVdY=", "Admin", "u1snxLW/luPKmyOTCfzw3/RY7zwN/IDaAnDCIZ9r9nA=" });

            migrationBuilder.CreateIndex(
                name: "IX_Animales_AdoptanteId",
                table: "Animales",
                column: "AdoptanteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Animales");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Adoptantes");
        }
    }
}
