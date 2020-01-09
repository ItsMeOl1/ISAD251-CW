CREATE PROCEDURE DeleteOrder @OrderID int
AS
DELETE FROM OrderDetails WHERE OrderID = @OrderID
DELETE FROM Orders WHERE ID = @OrderID