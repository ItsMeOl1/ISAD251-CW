CREATE PROCEDURE DeleteProduct @ID int
AS
DELETE FROM OrderDetails WHERE ProductID = @ID
DELETE FROM Products WHERE ID = @ID