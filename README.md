# 🐾 Refugio de Animales - Tarea 3

Proyecto de gestión para un refugio de animales desarrollado en **ASP.NET Core MVC**, siguiendo el patrón **Modelo-Vista-Controlador**.

---

## 📋 Descripción

Esta aplicación permite la gestión integral de un refugio, incluyendo:

- **Gestión de Animales:** CRUD completo con subida de imágenes y almacenamiento en base de datos.
- **Gestión de Adoptantes:** Registro y administración de personas interesadas.
- **Sistema de Adopciones:** Flujo completo para tramitar adopciones, bloqueando animales ya adoptados y permitiendo cancelaciones.
- **Seguridad:** Sistema de Login propio con contraseñas cifradas (SHA256 + Salt) y protección de rutas críticas.

> **Nota de Diseño:** La interfaz ha sido diseñada utilizando **CSS personalizado** (sin frameworks como Bootstrap), para garantizar un diseño único, ligero y adaptado a los requisitos.

---

## 🚀 Instrucciones de Ejecución

### 🔧 Requisitos Previos

- .NET SDK 6.0 o superior
- SQL Server LocalDB (o instancia compatible de SQL Server)

---

### 🗄️ Configuración de la Base de Datos

El proyecto utiliza **Entity Framework Core (Code First)**.  
Para inicializar la base de datos y cargar los datos de prueba (Seed):

1. Abre la terminal o consola en la carpeta del proyecto (donde está el archivo `.csproj`).
2. Ejecuta el comando para aplicar las migraciones:

```bash
dotnet ef database update
```

### 🗄️ Base de Datos y Datos de Prueba (Seeding)

Para poner en marcha el entorno de desarrollo, es necesario poblar la base de datos con información inicial.

> **ℹ️ Nota:** Al ejecutar el comando de inicialización, el sistema creará la base de datos localmente e insertará automáticamente los siguientes registros:
>
> - **3 Animales de ejemplo** (incluyendo sus fotografías):
>   - Luna
>   - Milo
>   - Bella
> - **2 Adoptantes de prueba** (para simular solicitudes).
> - **1 Usuario Administrador** (para gestión del panel).

## 🔑 Usuarios y Acceso

La aplicación distingue entre zonas públicas (accesibles para cualquier visitante) y zonas privadas de gestión.

Para acceder a las funciones de administrador (**Crear, Editar, Eliminar, Adoptar**), utiliza las siguientes credenciales preconfiguradas:

| Rol               | Usuario | Contraseña |
| :---------------- | :------ | :--------- |
| **Administrador** | `admin` | `admin123` |

---

## 🛠️ Funcionalidades Implementadas

### 🛡️ Control de Acceso

- **Usuarios anónimos:** Solo pueden ver el listado general y la vista en detalle de los animales.
- **Administrador:** Al iniciar sesión, se habilitan los botones de gestión y el acceso al listado de adoptantes.
- **Protección en el servidor:** Las rutas críticas (`/Crear`, `/Editar`, `/Eliminar`) redirigen automáticamente al _Login_ si no se detecta una sesión activa.

### 🖼️ Gestión de Imágenes

- Las fotografías se almacenan directamente en la base de datos como `byte[]`.
- El sistema incluye validaciones tanto para el **tipo de archivo** permitido como para el **tamaño máximo**.

### 🐾 Flujo de Adopción

- **Seguridad:** Implementación de _ViewModels_ para gestionar los datos del formulario de adopción de forma segura.
- **Lógica de Estado:**
  - **Bloqueo visual:** Si un animal ya está adoptado, el botón de "Adoptar" desaparece.
  - **Reversión:** Se incluye la opción de **"Desadoptar"** para liberar al animal y revertir su estado.

---

## 👤 Autor

- **Nombre:** Hugo Pexegueiro Salazar
- **Ciclo:** Desarrollo de Aplicaciones Web
- **Asignatura:** Desarrollo Web en Entorno Servidor
