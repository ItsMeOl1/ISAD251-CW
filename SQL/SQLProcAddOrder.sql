CREATE PROCEDURE AddOrder @Name nvarchar(255), @TableNumber int, @Price float
AS
INSERT INTO Orders (Name, TableNumber, TotalPrice)
VALUES (@Name, @TableNumber, @Price);
SELECT SCOPE_IDENTITY()
RETURN ;