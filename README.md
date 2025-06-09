# 🧵 BordadosChileApp

**Sistema de gestión para talleres de bordados y estampados** desarrollado en **C# WinForms (.NET Framework)** y **SQL Server Express**, con funcionalidades completas para el manejo de clientes, empleados, proveedores, pedidos, inventario, facturación y guías de despacho.

![Logo PromoBordados](https://github.com/guismo95/BordadosChileApp/assets/your-image.png) <!-- reemplazar si usas GitHub Pages o imagen externa -->

---

## ✨ Características principales

- ✅ Inicio de sesión seguro con roles (admin / empleado)
- 👥 Gestión de Clientes, Proveedores y Empleados
- 📦 Registro completo de Pedidos con detalle por prenda
- 🧾 Generación de Facturas y Guías de Despacho en PDF (con IVA 19%)
- 📊 Exportación de reportes a Excel y PDF
- 🔒 Control de acceso por rol para módulos restringidos
- 📁 Instalador `.msi` para despliegue fácil
- 🎨 Diseño moderno con logo, colores personalizados y DataGridView estilizados

---

## 📷 Capturas de pantalla

### Pantalla Principal
![Main Menu](https://github.com/guismo95/BordadosChileApp/assets/your-image.png)

### Módulo Pedidos
![Pedidos](https://github.com/guismo95/BordadosChileApp/assets/your-image.png)

### Factura PDF generada
![Factura PDF](https://github.com/guismo95/BordadosChileApp/assets/your-image.png)

> 📌 Reemplaza los links si alojas imágenes en GitHub Pages, Imgur o GitHub Assets.

---

## ⚙️ Requisitos

- Windows 10/11
- .NET Framework 4.7.2 o superior
- SQL Server Express
- Visual Studio 2019/2022 (para desarrollo)

---

## 🚀 Instalación

1. Descargar e instalar el archivo `.msi` desde la carpeta `/Installer`.
2. Ejecutar el sistema y conectar con la base de datos `BordadosChileDB`.
3. Usuario por defecto:


---

## 📌 Notas

- Los módulos de **Facturas**, **Guías** y **Empleados** están restringidos solo al rol `admin`.
- Se utiliza `ClosedXML` para exportar Excel y `iTextSharp` para generar PDFs.

---

## 👨‍💻 Autor

**Jorge Humberto Hernández (Guismo95)**  
📫 [Perfil de GitHub](https://github.com/guismo95)  
🛠️ Desarrollador de software - BordadosChile

---

## 📝 Licencia

Este proyecto está protegido. No redistribuir sin permiso del autor.

