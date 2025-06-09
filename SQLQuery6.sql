CREATE TABLE PagosPersonal (
    Id INT PRIMARY KEY IDENTITY,
    Fecha DATE,
    Nombre NVARCHAR(100),
    Monto DECIMAL(18, 2),
    Observacion NVARCHAR(255)
);

CREATE TABLE PagosProveedores (
    Id INT PRIMARY KEY IDENTITY,
    Fecha DATE,
    Nombre NVARCHAR(100),
    Monto DECIMAL(18, 2),
    Observacion NVARCHAR(255)
);
