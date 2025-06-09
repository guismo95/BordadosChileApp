CREATE TABLE Proveedores (
    Id INT PRIMARY KEY IDENTITY(1,1),
    NombreProveedor NVARCHAR(100),
    Direccion NVARCHAR(150),
    Telefono NVARCHAR(20),
    Correo NVARCHAR(100),
    NombreContacto NVARCHAR(100)
);
CREATE TABLE Empleados (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nombres NVARCHAR(100),
    Telefono NVARCHAR(20),
    Direccion NVARCHAR(150),
    Correo NVARCHAR(100),
    SueldoBase DECIMAL(18, 2),
    Cargo NVARCHAR(50)
);
