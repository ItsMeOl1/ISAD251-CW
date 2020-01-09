CREATE PROCEDURE EditProduct @ID int, @Name varchar(255), @Details varchar(255), @Price float, @Quantity INT
AS
UPDATE Products
SET Name = @Name, ProductDetails = @Details, Price = @Price, Quantity = @Quantity
WHERE ID = @ID;