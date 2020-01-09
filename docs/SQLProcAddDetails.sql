CREATE PROCEDURE AddDetails @OrderID int, @ProductID int, @Quantity int
AS
INSERT INTO OrderDetails (OrderID, ProductID, OrderQuantity)
VALUES (@OrderID, @ProductID, @Quantity);