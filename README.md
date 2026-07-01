# TRABAJO TESTING - APLICACIONES SERVIDOR WEB

# API REST - Arquitectura Limpia Unificada & Vertical Slices (C# .NET 10)

Proyecto desarrollado para la asignatura de **Aplicaciones Servidor Web (Quinto Semestre)** en la **Universidad Laica Eloy Alfaro de Manabí (ULEAM)**.

Esta API implementa un diseño de Arquitectura Limpia consolidada en un único proyecto, priorizando la alta cohesión a través del patrón **Vertical Slice Architecture**. Incluye un CRUD completo de usuarios con borrado lógico (Soft Delete), validaciones, auditoría automática (Pipeline Behaviors) y una suite de pruebas de integración orquestada con **Testcontainers**.

---

## 📂 1. Estructura del Proyecto

El código está organizado para evitar la burocracia de capas innecesarias (como repositorios genéricos) e inyecta directamente el `DbContext` donde se necesita. La estructura es la siguiente:

```text
TrabajoServer/
├── Api/                       # Proyecto Web API (Producción)
│   ├── Domain/                # Reglas de negocio e interfaces puras
│   │   └── Entities/          # Entidades del Dominio (Usuario.cs)
│   ├── Application/           # Configuración de nivel de aplicación
│   │   └── Behaviors/         # Interceptores MediatR (LoggingBehavior, PerformanceBehavior)
│   ├── Features/              # Slices Verticales (Agrupados por entidad)
│   │   └── Usuarios/          # CRUD de Usuarios (Crear, Obtener, Listar, Actualizar, Borrar)
│   ├── Infrastructure/        # Persistencia de Datos
│   │   └── AppDbContext.cs    # Contexto de EF Core con Global Query Filters
│   └── Program.cs             # Registro de servicios y Minimal APIs
│
├── Api.AppHost/               # Proyecto .NET Aspire 
│   └── Program.cs             # Orquestador local para levantar contenedores y la API
│
└── Api.Tests/                 # Suite de Pruebas de Integración (xUnit)
    ├── IntegrationTestBase.cs # Configuración base: Testcontainers, Respawn y WebApplicationFactory
    └── *Tests.cs              # Pruebas automatizadas por cada endpoint (Crear, Listar, Delete, etc.)


---

## 🏗️ 2. Reglas de Diseño Implementadas

A. Dominio e Infraestructura

* Entidades Puras: La clase Usuario vive aislada en el dominio y posee una bandera IsDeleted para habilitar el Soft Delete.

* Filtros Globales (Global Query Filters): El AppDbContext incluye una regla nativa (HasQueryFilter(u => !u.IsDeleted)) que garantiza que los usuarios eliminados lógicamente nunca sean retornados por accidente en consultas posteriores.


B. Aplicación (Vertical Slices & MediatR)

* Feature en Archivo Único: Cada operación (ej. DeleteUser.cs) contiene en un solo archivo C#:

* El Comando/Consulta (CQRS).

* El Handler (Lógica de negocio interactuando con EF Core).

* El Endpoint de Minimal API (Mapeo de la ruta HTTP).

* Pipeline Behaviors: Se intercepta el flujo de MediatR para inyectar automáticamente Logging (auditoría de parámetros de entrada/salida) y Performance (alertas si una consulta tarda más de 500ms) sin ensuciar el código de los Handlers.


C. Pruebas de Integración (Testing)

* Aislamiento Total: Uso de Testcontainers (SQL Server) para levantar una base de datos real y efímera en Docker por cada ejecución de pruebas, evitando depender de bases de datos locales.

* Manejo de Concurrencia: Refactorización de la inicialización paralela de xUnit para que cada contenedor aplique EnsureCreatedAsync de manera aislada, evitando colisiones de Invalid Object Name.

* Limpieza de Estado: Uso de la librería Respawn para resetear el esquema de la base de datos de manera ultra-rápida entre pruebas.


## 🤖 3. Justificación Tecnológica: IA Externa vs. Agentes Integrados en el IDE
Para el desarrollo y depuración de esta arquitectura, se tomó la decisión técnica de utilizar un modelo de IA externo y conversacional (Gemini/Prompt Engineering) en lugar de depender exclusivamente de los agentes integrados en el IDE (como Antigravity, GitHub Copilot, Cursor, etc.) recomendados inicialmente.


Las razones de esta decisión metodológica son:

*Manejo de Contexto Arquitectónico Global: Los agentes integrados (como Antigravity o Copilot) son excepcionales para el autocompletado en línea y tareas focalizadas. Sin embargo, en arquitecturas complejas como Vertical Slices, los agentes internos a menudo pierden la "visión de helicóptero". Un modelo externo permitió mantener un hilo conductor estricto entre las reglas de dominio, la persistencia y la configuración del host.

*Depuración de Infraestructura y Docker: El proyecto presentó desafíos significativos fuera del código fuente de C#, específicamente con el Daemon de Docker, protocolos de red (npipe), y colisiones de hilos en Testcontainers. Los agentes del IDE están diseñados para razonar sobre el código de la ventana activa, mientras que un modelo externo permitió pegar logs completos del sistema y razonar sobre la infraestructura y el sistema operativo.

*Proceso de Aprendizaje Activo (Evitar la "Caja Negra"): Delegar toda la refactorización a un agente autónomo dentro del editor convierte el desarrollo en una caja negra donde el código aparece mágicamente. Interactuar con una IA mediante prompts detallados obligó a analizar el porqué de los fallos (ej. el problema de inicialización paralela de xUnit que rompía la tabla Usuarios) e implementar las correcciones de forma manual y consciente, asegurando un aprendizaje real de los patrones modernos de .NET 10.

## 🛠️ 4. Instrucciones de Ejecución

- Requisito previo: Tener Docker Desktop ejecutándose en segundo plano.

* Restaurar el proyecto:

Bash
dotnet clean
dotnet build


* Ejecutar las Pruebas de Integración:

Bash
# xUnit levantará los contenedores de SQL Server automáticamente y ejecutará el CRUD completo.
dotnet test