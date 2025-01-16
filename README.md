# 🏠 Sistema de Gestión de Alquileres de Inmuebles

[![.Net Core](https://img.shields.io/badge/Framework-.Net%20Core-blue?style=flat-square)](https://dotnet.microsoft.com/)
[![Database](https://img.shields.io/badge/Database-MSSqlLocalDB-brightgreen?style=flat-square)](https://docs.microsoft.com/en-us/sql/sql-server/sql-server-editions-express)
[![Visual Studio Code](https://img.shields.io/badge/IDE-VS%20Code-orange?style=flat-square)](https://code.visualstudio.com/)


## 📄 Descripción del Proyecto

Este sistema informatiza la gestión de alquileres de propiedades para una agencia inmobiliaria. Permite administrar eficientemente propietarios, inquilinos, inmuebles, contratos de alquiler y pagos. Además, incluye funcionalidades avanzadas como auditoría, informes detallados y roles de usuario con permisos diferenciados.

## 🕹️ Funcionalidades Principales

### **Gestión de Entidades**
- **Propietarios:**
  - Alta, baja y modificación de propietarios con datos como DNI, nombre, apellido y contacto.
  - Relación uno a varios con inmuebles.
- **Inmuebles:**
  - Administración de propiedades (alta, baja, modificación).
  - Registro de características: dirección, uso (residencial/comercial), tipo (casa, departamento, etc.), cantidad de ambientes, precio y coordenadas.
  - Suspensión temporal de la oferta de un inmueble.
- **Inquilinos:**
  - Alta, baja y modificación de inquilinos con datos personales y de contacto.
  - Relación con contratos de alquiler.
- **Contratos:**
  - Creación de contratos con fechas de inicio y fin, monto mensual, e identificación del inquilino y el inmueble.
  - Gestión de finalizaciones anticipadas, incluyendo cálculo y registro de multas.
  - Renovación automática de contratos.
- **Pagos:**
  - Registro de pagos asociados a contratos: número, fecha, concepto e importe.
  - Edición limitada al concepto del pago.
  - Anulación de pagos mediante un cambio de estado.

### **Roles y Seguridad**
- **Administrador:**
  - Acceso completo al sistema, incluida la gestión de usuarios y eliminación de entidades.
  - Visualización de auditorías (quién creó o anuló contratos/pagos).
- **Empleado:**
  - Modificación de su perfil personal (datos, contraseña y avatar).
  - Sin permisos para gestionar usuarios ni eliminar entidades.

### **Informes y Consultas**
- Listar inmuebles por disponibilidad, propietario o filtros específicos.
- Consultar contratos vigentes, próximos a finalizar (30, 60 o 90 días), o asociados a un inmueble o inquilino específico.
- Consultar pagos realizados y permitir la carga de nuevos pagos desde el listado.
- Búsqueda de inmuebles disponibles en un rango de fechas.

## 🚀 Tecnologías Utilizadas

### **Backend**
- **Framework:** .Net Core
- **Base de Datos:** MSSqlLocalDB (configurable para otros motores).
- **ORM:** EntityFrameworkCore y extensiones para MySQL.

### **Frontend**
- **Herramientas de diseño:** Figma para pantallas y diagramas.

### **Extensiones**
- C# para VS Code: Soporte de desarrollo en C#.
- Conveyor by Keyoti: Conexión remota al IIS Express generado por Visual Studio.

### **Paquetes**
- **Autenticación:** `JwtBearer` para autenticación JWT.
- **Funcionalidades adicionales:**
  - `MailKit`: Envío de correos electrónicos.


## 🛠️ Herramientas Requeridas
- **IDE:** Visual Studio Code.
- **Generador de Código:** `aspnet-codegenerator`.
- **Depuración:** Postman para pruebas de API.

