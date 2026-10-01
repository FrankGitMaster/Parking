# 🚗 Parking Management System

![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/postgresql-4169e1?style=for-the-badge&logo=postgresql&logoColor=white)
![Bootstrap](https://img.shields.io/badge/bootstrap-%238511FA.svg?style=for-the-badge&logo=bootstrap&logoColor=white)
![JavaScript](https://img.shields.io/badge/javascript-%23323330.svg?style=for-the-badge&logo=javascript&logoColor=%23F7DF1E)

Un sistema integral para la gestión de parqueaderos desarrollado con **ASP.NET Core MVC** y **PostgreSQL**. 

Este proyecto fue construido aplicando principios de desarrollo backend robusto, incluyendo seguridad de identidades, manejo de transacciones en base de datos, y renderizado dinámico en el cliente usando Vanilla JavaScript (Fetch API).

## ✨ Características Principales

- **Gestión de Espacios y Vehículos:** Control en tiempo real de la disponibilidad de espacios según el tipo de vehículo.
- **Motor de Tarifas Dinámico:** Cálculo automático de cobros por minuto o tarifa plena basado en la hora de ingreso y salida.
- **Autenticación y Autorización:** Sistema de roles (Administrador, Operador) implementado con **ASP.NET Core Identity**, incluyendo políticas de bloqueo por intentos fallidos y validación estricta de contraseñas mediante *Data Annotations* personalizadas.
- **Transacciones Seguras:** Uso de `BeginTransactionAsync` de Entity Framework Core para garantizar la atomicidad (ACID) al finalizar servicios y liberar espacios simultáneamente.
- **Reportes en Excel:** Generación y descarga de reportes de servicios filtrados por espacio utilizando la librería **EPPlus**.

## 🏗️ Arquitectura y Tecnologías

- **Backend:** C# con ASP.NET Core MVC.
- **ORM:** Entity Framework Core (Code-First) con `UseSnakeCaseNamingConvention` para estandarización en PostgreSQL.
- **Frontend:** Razor Views (`.cshtml`), Bootstrap 5, y llamadas asíncronas al servidor usando `fetch` (Vanilla JS) para actualizar la UI sin recargar la página.
- **Seguridad:** Cookies configuradas con `HttpOnly`, `SecurePolicy.Always` y `SameSiteMode.Strict`.

## 🚀 Instalación y Configuración Local

1. Clona el repositorio:
   ```bash
   git clone git@github.com:FrankGitMaster/Parking.git
