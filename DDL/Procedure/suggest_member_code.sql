USE [MyDatabase]
GO

CREATE OR ALTER PROCEDURE dbo.sp_suggest_member_code (@member_code_part NVARCHAR(8))
AS
SELECT TOP 20 member_code
FROM t_member
WHERE member_code LIKE N'%' + @member_code_part + N'%'
ORDER BY member_code
GO
