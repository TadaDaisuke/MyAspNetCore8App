USE [MyDatabase]
GO

CREATE OR ALTER FUNCTION dbo.tvf_members_filtered (
    @member_name_part NVARCHAR(128)
    ,@member_code NVARCHAR(8)
    ,@joined_date_from DATE
    ,@joined_date_to DATE
    ,@department_code NVARCHAR(6)
    ,@has_terminated_members BIT
    ,@email_domain NVARCHAR(32)
    )
RETURNS TABLE
AS
RETURN (
        SELECT *
        FROM (
            SELECT *
                ,given_name + N' ' + family_name AS full_name
                ,family_name_kana + N' ' + given_name_kana AS full_name_kana
                ,family_name_kanji + N' ' + given_name_kanji AS full_name_kanji
            FROM t_member
            ) AS main
        WHERE (
                @member_name_part IS NULL
                OR full_name LIKE N'%' + @member_name_part + N'%'
                OR full_name_kana LIKE N'%' + @member_name_part + N'%'
                OR full_name_kanji LIKE N'%' + @member_name_part + N'%'
                )
            AND (
                @member_code IS NULL
                OR member_code = @member_code
                )
            AND (
                @joined_date_from IS NULL
                OR @joined_date_from <= joined_date
                )
            AND (
                @joined_date_to IS NULL
                OR joined_date <= @joined_date_to
                )
            AND (
                @department_code IS NULL
                OR department_code = @department_code
                )
            AND (
                @has_terminated_members = 1
                OR termination_date IS NULL
                )
            AND (
                @email_domain IS NULL
                OR mail_address LIKE N'%' + @email_domain
                )
        )
GO

CREATE OR ALTER FUNCTION dbo.tvf_members_sorted (
    @member_name_part NVARCHAR(128)
    ,@member_code NVARCHAR(8)
    ,@joined_date_from DATE
    ,@joined_date_to DATE
    ,@department_code NVARCHAR(6)
    ,@has_terminated_members BIT
    ,@email_domain NVARCHAR(32)
    ,@sort_item NVARCHAR(128)
    ,@sort_type NVARCHAR(4)
    )
RETURNS TABLE
AS
RETURN (
        SELECT main.*
            ,ROW_NUMBER() OVER (
                ORDER BY IIF(@sort_item = N'joined_date' AND @sort_type = N'asc', main.joined_date, NULL) ASC
                    ,IIF(@sort_item = N'joined_date' AND @sort_type = N'desc', main.joined_date, NULL) DESC
                    ,IIF(@sort_item = N'termination_date' AND @sort_type = N'asc', ISNULL(main.termination_date, DATEFROMPARTS(9999, 12, 31)), NULL) ASC
                    ,IIF(@sort_item = N'termination_date' AND @sort_type = N'desc', ISNULL(main.termination_date, DATEFROMPARTS(1, 1, 1)), NULL) DESC
                    ,IIF(@sort_item = N'member_code' AND @sort_type = N'asc', main.member_code, NULL) ASC
                    ,IIF(@sort_item = N'member_code' AND @sort_type = N'desc', main.member_code, NULL) DESC
                    ,IIF(@sort_item = N'name_kana' AND @sort_type = N'asc', main.full_name_kana, NULL) ASC
                    ,IIF(@sort_item = N'name_kana' AND @sort_type = N'desc', main.full_name_kana, NULL) DESC
                    ,IIF(@sort_item = N'name_english' AND @sort_type = N'asc', main.full_name, NULL) ASC
                    ,IIF(@sort_item = N'name_english' AND @sort_type = N'desc', main.full_name, NULL) DESC
                    ,IIF(@sort_item = N'department_code' AND @sort_type = N'asc', main.department_code, NULL) ASC
                    ,IIF(@sort_item = N'department_code' AND @sort_type = N'desc', main.department_code, NULL) DESC
                ) AS seq
            ,total.total_records_count
        FROM tvf_members_filtered(@member_name_part, @member_code, @joined_date_from, @joined_date_to, @department_code, @has_terminated_members, @email_domain) AS main
        CROSS JOIN (
            SELECT COUNT(*) AS total_records_count
            FROM tvf_members_filtered(@member_name_part, @member_code, @joined_date_from, @joined_date_to, @department_code, @has_terminated_members, @email_domain)
            ) AS total
        )
GO

CREATE OR ALTER PROCEDURE dbo.sp_search_members (
    @member_name_part NVARCHAR(128)
    ,@member_code NVARCHAR(8)
    ,@joined_date_from DATE
    ,@joined_date_to DATE
    ,@department_code NVARCHAR(6)
    ,@has_terminated_members BIT
    ,@email_domain NVARCHAR(32)
    ,@sort_item NVARCHAR(128)
    ,@sort_type NVARCHAR(4)
    ,@offset_rows INT
    ,@fetch_rows INT
    )
AS
SELECT main.seq
    ,main.member_code
    ,main.given_name
    ,main.family_name
    ,main.given_name_kana
    ,main.family_name_kana
    ,main.given_name_kanji
    ,main.family_name_kanji
    ,main.mail_address
    ,dpt.department_name
    ,main.joined_date
    ,main.termination_date
    ,main.total_records_count
FROM tvf_members_sorted(@member_name_part, @member_code, @joined_date_from, @joined_date_to, @department_code, @has_terminated_members, @email_domain, @sort_item, @sort_type) AS main
INNER JOIN t_department AS dpt
    ON dpt.department_code = main.department_code
ORDER BY main.seq OFFSET @offset_rows ROWS
FETCH NEXT @fetch_rows ROWS ONLY
GO

CREATE OR ALTER PROCEDURE dbo.sp_download_members (
    @member_name_part NVARCHAR(128)
    ,@member_code NVARCHAR(8)
    ,@joined_date_from DATE
    ,@joined_date_to DATE
    ,@department_code NVARCHAR(6)
    ,@has_terminated_members BIT
    ,@email_domain NVARCHAR(32)
    ,@sort_item NVARCHAR(128)
    ,@sort_type NVARCHAR(4)
    )
AS
SELECT main.member_code AS [メンバーコード]
    ,main.full_name_kanji AS [漢字氏名]
    ,main.full_name_kana AS [カナ氏名]
    ,main.full_name AS [英字氏名]
    ,main.mail_address AS [メールアドレス]
    ,dpt.department_name AS [所属部署]
    ,main.joined_date AS [着任日]
    ,main.termination_date AS [離任日]
    ,main.note AS [備考]
FROM tvf_members_sorted(@member_name_part, @member_code, @joined_date_from, @joined_date_to, @department_code, @has_terminated_members, @email_domain, @sort_item, @sort_type) AS main
INNER JOIN t_department AS dpt
    ON dpt.department_code = main.department_code
ORDER BY main.seq
GO
