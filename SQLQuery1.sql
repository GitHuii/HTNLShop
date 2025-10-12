USE HTLNShop;
GO

CREATE TABLE Roles (
    RoleId INT IDENTITY(1,1) PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL
);
GO

CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(50),
    Email VARCHAR(100),
    PhoneNumber VARCHAR(13) NOT NULL,
    Address NVARCHAR(100) NOT NULL,
    Username VARCHAR(20) NOT NULL,
    Password VARCHAR(20) NOT NULL,
    RoleId INT NOT NULL,
    FOREIGN KEY (RoleId) REFERENCES Roles(RoleId)
);
GO

CREATE TABLE Carts (
    CartId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
GO

CREATE TABLE Categories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(200) NOT NULL
);
GO

CREATE TABLE Products (
    ProductId INT IDENTITY(1,1) PRIMARY KEY,
    ProductName NVARCHAR(500) NOT NULL,
    Price FLOAT NOT NULL,
    ProductDetail NVARCHAR(1000),
    StockQuantity INT NOT NULL DEFAULT 0,
    ImageUrl VARCHAR(200),
    CategoryId INT NOT NULL,
    FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId)
);
GO

CREATE TABLE CartItems (
    CartId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL DEFAULT 1,
    FOREIGN KEY (CartId) REFERENCES Carts(CartId),
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId),
    CONSTRAINT UQ_Cart_Product UNIQUE (CartId, ProductId)
);
GO

CREATE TABLE Orders (
    OrderId INT IDENTITY(1,1) PRIMARY KEY,
    OrderDate DATETIME DEFAULT GETDATE(),
    Status NVARCHAR(50) NOT NULL,
    TotalPrice FLOAT DEFAULT 0,
    ShippingAddress NVARCHAR(200) NOT NULL,
    UserId INT NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
GO

CREATE TABLE OrderItems (
    OrderId INT NOT NULL,
    ProductId INT NOT NULL,
    SalePrice FLOAT NOT NULL,
    Quantity INT NOT NULL DEFAULT 1,
    FOREIGN KEY (OrderId) REFERENCES Orders(OrderId),
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
);
GO
