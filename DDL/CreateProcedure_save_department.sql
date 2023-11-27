USE [MyDatabase]
GO

CREATE OR ALTER PROCEDURE dbo.sp_save_department (
    @department_code NVARCHAR(6)
    ,@department_name NVARCHAR(128)
    ,@error_message NVARCHAR(4000) OUTPUT
    )
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION

        MERGE department
        USING (
            SELECT @department_code
                ,@department_name
            ) AS src(department_code, department_name)
            ON (department.department_code = src.department_code)
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
                        FROM department
                        WHERE department_code LIKE N'Z%'
                        )
                    ,src.department_name
                    );

        COMMIT TRANSACTION
    END TRY

    BEGIN CATCH
        ROLLBACK TRANSACTION

        SET @error_message = @error_message + ERROR_MESSAGE() + CHAR(13) + CHAR(10)
        SET @error_message = @error_message + N'エラーが発生したためロールバックしました。'
    END CATCH
END
GO
