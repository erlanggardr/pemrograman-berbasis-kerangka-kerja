-- ============================================================================
-- PRAKTIKUM PEMROGRAMAN BERBASIS KERANGKA KERJA (PBKK) - PERTEMUAN 6
-- HANDS-ON LAB 1: MEMBANGUN DATABASE STUDENT REGISTRATION DENGAN SQL SERVER
-- ============================================================================
-- Deskripsi: Skrip DDL dan DML lengkap untuk membuat basis data StudentRegistrationDB,
--            tabel relasional Programs dan Students (1:N), integritas referensial,
--            serta data awal (seed data) untuk aplikasi registrasi mahasiswa.
-- ============================================================================

-- ----------------------------------------------------------------------------
-- 1. PEMBUATAN DATABASE
-- ----------------------------------------------------------------------------
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'StudentRegistrationDB')
BEGIN
    CREATE DATABASE StudentRegistrationDB;
    PRINT 'Database StudentRegistrationDB berhasil dibuat.';
END
ELSE
BEGIN
    PRINT 'Database StudentRegistrationDB sudah ada.';
END
GO

USE StudentRegistrationDB;
GO

-- ----------------------------------------------------------------------------
-- 2. PEMBERSIHAN TABEL JIKA SUDAH ADA (IDEMPOTEN)
-- ----------------------------------------------------------------------------
IF OBJECT_ID(N'dbo.Students', N'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.Students;
    PRINT 'Tabel Students lama berhasil dihapus.';
END
GO

IF OBJECT_ID(N'dbo.Programs', N'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.Programs;
    PRINT 'Tabel Programs lama berhasil dihapus.';
END
GO

-- ----------------------------------------------------------------------------
-- 3. PEMBUATAN TABEL PROGRAMS (PARENT TABLE)
-- ----------------------------------------------------------------------------
CREATE TABLE dbo.Programs
(
    ProgramId   INT IDENTITY(1,1) NOT NULL,
    ProgramCode VARCHAR(20)       NOT NULL,
    ProgramName VARCHAR(100)      NOT NULL,
    IsActive    BIT               NOT NULL CONSTRAINT DF_Programs_IsActive DEFAULT (1),
    CreatedAt   DATETIME2         NOT NULL CONSTRAINT DF_Programs_CreatedAt DEFAULT (SYSDATETIME()),

    CONSTRAINT PK_Programs PRIMARY KEY CLUSTERED (ProgramId),
    CONSTRAINT UQ_Programs_ProgramCode UNIQUE NONCLUSTERED (ProgramCode)
);
PRINT 'Tabel Programs berhasil dibuat dengan Primary Key dan Unique Constraint.';
GO

-- ----------------------------------------------------------------------------
-- 4. PEMBUATAN TABEL STUDENTS (CHILD TABLE DENGAN FOREIGN KEY 1:N)
-- ----------------------------------------------------------------------------
CREATE TABLE dbo.Students
(
    StudentId   INT IDENTITY(1,1) NOT NULL,
    NIM         VARCHAR(20)       NOT NULL,
    Name        VARCHAR(100)      NOT NULL,
    ProgramId   INT               NOT NULL,
    BirthDate   DATE              NULL,
    Address     NVARCHAR(255)     NULL,
    PhoneNumber VARCHAR(20)       NULL,
    Email       VARCHAR(100)      NULL,
    GPA         DECIMAL(3,2)      NULL,
    IsActive    BIT               NOT NULL CONSTRAINT DF_Students_IsActive DEFAULT (1),
    CreatedAt   DATETIME2         NOT NULL CONSTRAINT DF_Students_CreatedAt DEFAULT (SYSDATETIME()),

    CONSTRAINT PK_Students PRIMARY KEY CLUSTERED (StudentId),
    CONSTRAINT UQ_Students_NIM UNIQUE NONCLUSTERED (NIM),
    CONSTRAINT FK_Students_Programs FOREIGN KEY (ProgramId)
        REFERENCES dbo.Programs (ProgramId)
        ON UPDATE CASCADE
        ON DELETE NO ACTION,
    CONSTRAINT CK_Students_GPA CHECK (GPA >= 0.00 AND GPA <= 4.00)
);
PRINT 'Tabel Students berhasil dibuat dengan Foreign Key ke tabel Programs dan validasi GPA.';
GO

-- ----------------------------------------------------------------------------
-- 5. INSERT SEED DATA: TABEL PROGRAMS
-- ----------------------------------------------------------------------------
SET IDENTITY_INSERT dbo.Programs ON;

INSERT INTO dbo.Programs (ProgramId, ProgramCode, ProgramName, IsActive, CreatedAt)
VALUES
(1, 'TI',  'Teknik Informatika',        1, SYSDATETIME()),
(2, 'SI',  'Sistem Informasi',           1, SYSDATETIME()),
(3, 'SD',  'Sains Data',                 1, SYSDATETIME()),
(4, 'TK',  'Teknik Komputer',            1, SYSDATETIME()),
(5, 'TE',  'Teknik Elektro',             1, SYSDATETIME()),
(6, 'DKV', 'Desain Komunikasi Visual',   1, SYSDATETIME()),
(7, 'RPL', 'Rekayasa Perangkat Lunak',   1, SYSDATETIME());

SET IDENTITY_INSERT dbo.Programs OFF;
PRINT 'Seed data tabel Programs berhasil dimasukkan (7 program studi).';
GO

-- ----------------------------------------------------------------------------
-- 6. INSERT SEED DATA: TABEL STUDENTS
-- ----------------------------------------------------------------------------
INSERT INTO dbo.Students 
    (NIM, Name, ProgramId, BirthDate, Address, PhoneNumber, Email, GPA, IsActive, CreatedAt)
VALUES
('5025241001', 'Muhammad Rizky Pratama', 1, '2005-04-12', 'Jl. Gebang Wetan No. 15, Surabaya',      '081234567801', 'rizky.pratama@mhs.its.ac.id', 3.88, 1, SYSDATETIME()),
('5025241025', 'Aulia Dewi Safitri',     2, '2005-08-23', 'Jl. Keputih Perintis No. 8, Surabaya',     '081234567802', 'aulia.safitri@mhs.its.ac.id', 3.74, 1, SYSDATETIME()),
('5025241054', 'Dimas Arya Nugroho',     3, '2006-01-15', 'Jl. Mulyosari Prima No. 42, Surabaya',    '081234567803', 'dimas.arya@mhs.its.ac.id',    3.65, 1, SYSDATETIME()),
('5025241078', 'Vania Kusuma Wardhani',  1, '2005-11-30', 'Perumahan Galaxy Bumi Permai, Surabaya',   '081234567804', 'vania.wardhani@mhs.its.ac.id', 3.92, 1, SYSDATETIME()),
('5025241099', 'Fajar Hidayatullah',     4, '2004-06-19', 'Jl. Kejawan Putih Tambak No. 12, Surabaya','081234567805', 'fajar.hidayat@mhs.its.ac.id', 3.25, 0, SYSDATETIME()),
('5025241112', 'Gabriella Natasha',      5, '2003-09-05', 'Jl. Arief Rahman Hakim No. 88, Surabaya',  '081234567806', 'gabriella.n@mhs.its.ac.id',   3.80, 1, SYSDATETIME()),
('5025241130', 'Reza Ananta Putra',      7, '2005-03-27', 'Jl. Manyar Sabrangan No. 3, Surabaya',    '081234567807', 'reza.ananta@mhs.its.ac.id',   3.68, 1, SYSDATETIME()),
('5025241145', 'Siti Nurhaliza',         2, '2004-12-10', 'Jl. Medokan Asri Barat No. 20, Surabaya',  '081234567808', 'siti.nurhaliza@mhs.its.ac.id', 3.45, 1, SYSDATETIME()),
('5025241160', 'Budi Santoso',           4, '2005-07-08', 'Jl. Semolowaru Elok No. 5, Surabaya',     '081234567809', 'budi.santoso@mhs.its.ac.id',  2.95, 1, SYSDATETIME()),
('5025241179', 'Erlangga Rizqi Dwi R.',   1, '2005-02-18', 'Jl. Teknik Sipil Blok C, Sukolilo, Sby',   '081234567810', 'erlangga.rizqi@mhs.its.ac.id', 3.85, 1, SYSDATETIME()),
('5025241188', 'Clarissa Putri Maharani', 6, '2005-10-14', 'Jl. Dharmawangsa Barat No. 11, Surabaya', '081234567811', 'clarissa.putri@mhs.its.ac.id', 3.52, 1, SYSDATETIME());

PRINT 'Seed data tabel Students berhasil dimasukkan (11 mahasiswa).';
GO

-- ----------------------------------------------------------------------------
-- 7. PENGUJIAN INTEGRITAS BASIS DATA
-- ----------------------------------------------------------------------------

-- A. Uji Relasi One-to-Many (INNER JOIN)
PRINT '--- HASIL PENGUJIAN RELASI (INNER JOIN) ---';
SELECT 
    s.StudentId,
    s.NIM,
    s.Name AS StudentName,
    p.ProgramCode,
    p.ProgramName,
    s.GPA,
    CASE 
        WHEN s.GPA >= 3.51 THEN 'Cumlaude'
        WHEN s.GPA >= 3.00 THEN 'Sangat Memuaskan'
        WHEN s.GPA >= 2.50 THEN 'Memuaskan'
        ELSE 'Cukup'
    END AS Predikat,
    CASE WHEN s.IsActive = 1 THEN 'Aktif' ELSE 'Non-Aktif' END AS StatusAkademik,
    s.Email,
    s.PhoneNumber
FROM dbo.Students s
INNER JOIN dbo.Programs p ON s.ProgramId = p.ProgramId
ORDER BY s.StudentId ASC;
GO

-- B. Uji Integritas Foreign Key (Simulasi Error - Foreign Key Violation)
-- Query ini diharapkan menghasilkan error FK jika dijalankan karena ProgramId = 999 tidak ada di Programs.
/*
BEGIN TRY
    INSERT INTO dbo.Students (NIM, Name, ProgramId, GPA)
    VALUES ('5025249999', 'Mahasiswa Uji FK Gagal', 999, 3.50);
END TRY
BEGIN CATCH
    PRINT 'PENGUJIAN FK BERHASIL: Error dicegat -> ' + ERROR_MESSAGE();
END CATCH
*/

-- C. Uji Integritas Unique Constraint (Simulasi Error - Duplicate Key Violation)
-- Query ini diharapkan menghasilkan error UNIQUE jika dijalankan karena NIM 5025241001 sudah terdaftar.
/*
BEGIN TRY
    INSERT INTO dbo.Students (NIM, Name, ProgramId, GPA)
    VALUES ('5025241001', 'Mahasiswa Uji NIM Duplikat', 1, 3.50);
END TRY
BEGIN CATCH
    PRINT 'PENGUJIAN UNIQUE BERHASIL: Error dicegat -> ' + ERROR_MESSAGE();
END CATCH
*/
GO
