CREATE TABLE Pagos (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Fecha DATETIME NOT NULL,
    Tipo NVARCHAR(50) NOT NULL, -- "Proveedor" o "Personal"
    Nombre NVARCHAR(100) NOT NULL,
    Monto DECIMAL(10, 2) NOT NULL,
    Observacion NVARCHAR(255)
);
