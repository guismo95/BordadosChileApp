CREATE TABLE Pedidos (
    Id INT PRIMARY KEY IDENTITY,
    IdCliente INT,
    Fecha DATETIME,
    Estado NVARCHAR(50),
    Prioridad NVARCHAR(20),
    Costo DECIMAL(18,2),
    FOREIGN KEY (IdCliente) REFERENCES Clientes(Id)
);
