USE [MyDatabase]
GO

CREATE OR ALTER PROCEDURE dbo.sp_get_department (@department_code NVARCHAR(6))
AS
SELECT TOP 1 department_code
    ,department_name
FROM department
WHERE department_code = @department_code
GO
