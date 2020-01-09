DROP TABLE OrderDetails;
DROP TABLE Products;
DROP TABLE Orders;

CREATE TABLE Products (
    ID int IDENTITY NOT NULL PRIMARY KEY,
    Name varchar(255) NOT NULL,
    ProductDetails varchar(255) NOT NULL,
    Price float,
    Quantity int
);

CREATE TABLE Orders (
    ID int IDENTITY NOT NULL PRIMARY KEY,
    Name varchar(255) NOT NULL,
    TableNumber int,
    TotalPrice float
    );

CREATE TABLE OrderDetails (
    ProductID int FOREIGN KEY REFERENCES Products(ID),
    OrderID int FOREIGN KEY REFERENCES Orders(ID),
    OrderQuantity int,
    PRIMARY KEY (ProductID, OrderID)
    );





