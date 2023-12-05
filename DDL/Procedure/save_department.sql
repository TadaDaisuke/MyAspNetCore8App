USE [MyDatabase]
GO

CREATE OR ALTER PROCEDURE dbo.sp_save_department (
    @department_code NVARCHAR(6)
    ,@department_name NVARCHAR(128)
    )
AS
BEGIN
    MERGE t_department
    USING (
        SELECT @department_code
            ,@department_name
        ) AS src(department_code, department_name)
        ON (t_department.department_code = src.department_code)
    WHEN MATCHED
        THEN
            UPDATE
            SET department_name = src.department_name
    WHEN NOT MATCHED
        THEN
            INSERT (
                department_code
                ,department_name
                )
            VALUES (
                (
                    SELECT N'Z' + FORMAT(ISNULL(MAX(CONVERT(INT, SUBSTRING(department_code, 2, 5)) + 1), 1), N'00000')
                    FROM t_department
                    WHERE department_code LIKE N'Z%'
                    )
                ,src.department_name
                );
END
GO
