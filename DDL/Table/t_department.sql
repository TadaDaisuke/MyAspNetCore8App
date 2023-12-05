USE [MyDatabase]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID(N't_department') IS NOT NULL
    DROP TABLE t_department
GO

CREATE TABLE t_department (
    [department_code] NVARCHAR(6) NOT NULL PRIMARY KEY CLUSTERED
    ,[department_name] NVARCHAR(128) NOT NULL
    );
GO

INSERT INTO t_department
VALUES (N'X00001', N'営業部')
    ,(N'X00002', N'生産部')
    ,(N'X00003', N'物流部')
    ,(N'X00004', N'購買部')
    ,(N'X00005', N'品質管理部')
    ,(N'X00006', N'研究開発部')
    ,(N'X00007', N'総務部')
    ,(N'X00008', N'人事部')
    ,(N'X00009', N'経理部')
    ,(N'X00010', N'財務部')
    ,(N'X00011', N'法務部')
    ,(N'X00012', N'広報部')
    ,(N'X00013', N'マーケティング部')
    ,(N'X00014', N'情報システム部')
    ,(N'X00015', N'営業企画部')
    ,(N'Y00001', N'札幌事業所')
    ,(N'Y00002', N'仙台事業所')
    ,(N'Y00003', N'横浜事業所')
    ,(N'Y00004', N'名古屋事業所')
    ,(N'Y00005', N'大阪事業所')
    ,(N'Y00006', N'福岡事業所')
GO
