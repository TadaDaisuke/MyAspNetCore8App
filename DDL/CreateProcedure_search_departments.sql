USE [MyDatabase]
GO

CREATE OR ALTER FUNCTION dbo.tvf_departments_filtered (@department_name_part NVARCHAR(128))
RETURNS TABLE
AS
RETURN (
        SELECT *
        FROM department
        WHERE @department_name_part IS NULL
            OR department_name LIKE N'%' + @department_name_part + N'%'
        )
GO

CREATE OR ALTER FUNCTION dbo.tvf_departments_sorted (
    @department_name_part NVARCHAR(128)
    ,@sort_item NVARCHAR(128)
    ,@sort_type NVARCHAR(4)
    )
RETURNS TABLE
AS
RETURN (
        SELECT ROW_NUMBER() OVER (
                ORDER BY IIF(@sort_item = N'department_name' AND @sort_type = N'asc', main.department_name, NULL) ASC
                    ,IIF(@sort_item = N'department_name' AND @sort_type = N'desc', main.department_name, NULL) DESC
                    ,IIF(@sort_item = N'department_code' AND @sort_type = N'asc', main.department_code, NULL) ASC
                    ,IIF(@sort_item = N'department_code' AND @sort_type = N'desc', main.department_code, NULL) DESC
                ) AS seq
            ,main.department_code
            ,main.department_name
            ,total.total_records_count
        FROM tvf_departments_filtered(@department_name_part) AS main
        CROSS JOIN (
            SELECT COUNT(*) AS total_records_count
            FROM tvf_departments_filtered(@department_name_part)
            ) AS total
        )
GO

CREATE OR ALTER PROCEDURE dbo.sp_search_departments (
    @department_name_part NVARCHAR(128)
    ,@sort_item NVARCHAR(128)
    ,@sort_type NVARCHAR(4)
    ,@offset_rows INT
    ,@fetch_rows INT
    )
AS
SELECT main.seq
    ,main.department_code
    ,main.department_name
    ,main.total_records_count
FROM tvf_departments_sorted(@department_name_part, @sort_item, @sort_type) AS main
ORDER BY main.seq OFFSET @offset_rows ROWS
FETCH NEXT @fetch_rows ROWS ONLY
GO

CREATE OR ALTER PROCEDURE dbo.sp_download_departments (
    @department_name_part NVARCHAR(128)
    ,@sort_item NVARCHAR(128)
    ,@sort_type NVARCHAR(4)
    )
AS
SELECT main.department_code AS [部署コード]
    ,main.department_name AS [部署名]
FROM tvf_departments_sorted(@department_name_part, @sort_item, @sort_type) AS main
ORDER BY main.seq
GO
