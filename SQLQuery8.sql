CREATE TABLE Facturas (
  Id INT PRIMARY KEY IDENTITY,
  Fecha DATE,
  Cliente NVARCHAR(100),
  Monto DECIMAL(18,2),
  Observaciones NVARCHAR(255)
);

CREATE TABLE Guias (
  Id INT PRIMARY KEY IDENTITY,
  Fecha DATE,
  Cliente NVARCHAR(100),
  Detalle NVARCHAR(255),
  Cantidad INT,
  Observaciones NVARCHAR(255)
);
