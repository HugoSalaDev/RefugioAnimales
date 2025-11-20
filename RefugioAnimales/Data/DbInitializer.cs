using RefugioAnimales.Models;
using Microsoft.EntityFrameworkCore;

namespace RefugioAnimales.Data
{
    // Clase para inicializar datos de prueba en la BD
    public static class DbInitializer
    {
        public static void Initialize(RefugioContext context)
        {
            // Asegurar que la BD está creada
            context.Database.EnsureCreated();

            // Log para ver si entra aquí
            Console.WriteLine("=== INICIANDO CARGA DE IMÁGENES ===");

            // Cargar imágenes desde SeedImages
            CargarImagenesAnimales(context);
        }

        private static void CargarImagenesAnimales(RefugioContext context)
        {
            // Mostrar la ruta base
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            Console.WriteLine($"BaseDirectory: {baseDir}");

            string seedDir = Path.Combine(baseDir, "SeedImages");
            Console.WriteLine($"SeedImages path: {seedDir}");
            Console.WriteLine($"¿Existe carpeta SeedImages? {Directory.Exists(seedDir)}");

            if (!Directory.Exists(seedDir))
            {
                Console.WriteLine("ERROR: No existe la carpeta SeedImages");
                return;
            }

            // Listar archivos en la carpeta
            var archivos = Directory.GetFiles(seedDir);
            Console.WriteLine($"Archivos encontrados: {archivos.Length}");
            foreach (var archivo in archivos)
            {
                Console.WriteLine($"  - {Path.GetFileName(archivo)}");
            }

            // Diccionario: Id del animal -> nombre del archivo
            var imagenesAnimales = new Dictionary<int, string>
            {
                { 1, "luna.jpg" },
                { 2, "milo.jpg" },
                { 3, "bella.jpg" }
            };

            foreach (var item in imagenesAnimales)
            {
                int animalId = item.Key;
                string nombreArchivo = item.Value;
                string rutaCompleta = Path.Combine(seedDir, nombreArchivo);

                Console.WriteLine($"\nProcesando animal ID {animalId} - archivo: {nombreArchivo}");
                Console.WriteLine($"Ruta completa: {rutaCompleta}");
                Console.WriteLine($"¿Existe archivo? {File.Exists(rutaCompleta)}");

                if (File.Exists(rutaCompleta))
                {
                    try
                    {
                        // Leer la imagen como bytes
                        byte[] imagenBytes = File.ReadAllBytes(rutaCompleta);
                        string mimeType = nombreArchivo.EndsWith(".png") ? "image/png" : "image/jpeg";

                        Console.WriteLine($"Imagen leída: {imagenBytes.Length} bytes");

                        // Buscar el animal en la BD y actualizarlo
                        var animal = context.Animales.Find(animalId);
                        if (animal != null)
                        {
                            Console.WriteLine($"Animal encontrado: {animal.Nombre}");
                            animal.FotoContenido = imagenBytes;
                            animal.FotoMimeType = mimeType;
                            Console.WriteLine("Imagen asignada al animal");
                        }
                        else
                        {
                            Console.WriteLine($"ERROR: No se encontró animal con ID {animalId}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"ERROR al procesar imagen: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"ERROR: No existe el archivo {rutaCompleta}");
                }
            }

            // Guardar cambios
            Console.WriteLine("\n=== GUARDANDO CAMBIOS EN LA BD ===");
            int cambios = context.SaveChanges();
            Console.WriteLine($"Cambios guardados: {cambios}");
        }
    }
}