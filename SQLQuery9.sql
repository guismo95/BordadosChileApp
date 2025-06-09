CREATE TABLE Pedidos (
    Id INT PRIMARY KEY IDENTITY,
    Cliente NVARCHAR(100),
    Fecha DATE,
    Cajas INT,
    Bordado NVARCHAR(100),
    Logo VARBINARY(MAX),
    Estado NVARCHAR(50)
)

CREATE TABLE DetallePedidos (
    Id INT PRIMARY KEY IDENTITY,
    PedidoId INT FOREIGN KEY REFERENCES Pedidos(Id),
    TipoPrenda NVARCHAR(100),
    Color NVARCHAR(50),
    Cantidad INT,
    Bordado NVARCHAR(100)
)