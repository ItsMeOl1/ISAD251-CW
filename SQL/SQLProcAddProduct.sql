CREATE PROCEDURE AddProduct @Name varchar(255), @Details varchar(255), @Price float, @Quantity INT
AS
INSERT INTO Products (Name, ProductDetails, Price, Quantity)
VALUES (@Name, @Details, @Price, @Quantity)