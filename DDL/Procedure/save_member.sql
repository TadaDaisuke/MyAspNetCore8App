USE [MyDatabase]
GO

CREATE OR ALTER PROCEDURE dbo.sp_save_member (
    @member_code NVARCHAR(8)
    ,@given_name NVARCHAR(128)
    ,@family_name NVARCHAR(128)
    ,@given_name_kana NVARCHAR(128)
    ,@family_name_kana NVARCHAR(128)
    ,@given_name_kanji NVARCHAR(128)
    ,@family_name_kanji NVARCHAR(128)
    ,@mail_address NVARCHAR(256)
    ,@department_code NVARCHAR(6)
    ,@joined_date DATE
    ,@termination_date DATE
    ,@note NVARCHAR(4000)
    )
AS
BEGIN
    MERGE t_member
    USING (
        SELECT @member_code
            ,@given_name
            ,@family_name
            ,@given_name_kana
            ,@family_name_kana
            ,@given_name_kanji
            ,@family_name_kanji
            ,@mail_address
            ,@department_code
            ,@joined_date
            ,@termination_date
            ,@note
        ) AS src(member_code, given_name, family_name, given_name_kana, family_name_kana, given_name_kanji, family_name_kanji, mail_address, department_code, joined_date, termination_date, note)
        ON (t_member.member_code = src.member_code)
    WHEN MATCHED
        THEN
            UPDATE
            SET given_name = src.given_name
                ,family_name = src.family_name
                ,given_name_kana = src.given_name_kana
                ,family_name_kana = src.family_name_kana
                ,given_name_kanji = src.given_name_kanji
                ,family_name_kanji = src.family_name_kanji
                ,mail_address = src.mail_address
                ,department_code = src.department_code
                ,joined_date = src.joined_date
                ,termination_date = src.termination_date
                ,note = src.note
    WHEN NOT MATCHED
        THEN
            INSERT (
                member_code
                ,given_name
                ,family_name
                ,given_name_kana
                ,family_name_kana
                ,given_name_kanji
                ,family_name_kanji
                ,mail_address
                ,department_code
                ,joined_date
                ,termination_date
                ,note
                )
            VALUES (
                (
                    SELECT N'A' + FORMAT(ISNULL(MAX(CONVERT(INT, SUBSTRING(member_code, 2, 7)) + 1), 1), N'0000000')
                    FROM t_member
                    )
                ,src.given_name
                ,src.family_name
                ,src.given_name_kana
                ,src.family_name_kana
                ,src.given_name_kanji
                ,src.family_name_kanji
                ,src.mail_address
                ,src.department_code
                ,src.joined_date
                ,src.termination_date
                ,src.note
                );
END
GO
