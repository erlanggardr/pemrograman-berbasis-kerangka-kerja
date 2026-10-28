-- ============================================================================
-- PRAKTIKUM PEMROGRAMAN BERBASIS KERANGKA KERJA (PBKK) - PERTEMUAN 6
-- QUERY CHALLENGES & ANALISIS DATA STUDENT REGISTRATION
-- ============================================================================

USE StudentRegistrationDB;
GO

-- ----------------------------------------------------------------------------
-- CHALLENGE 1: DAFTAR MAHASISWA DENGAN NAMA PROGRAM STUDI (INNER JOIN)
-- Menampilkan NIM, Nama Mahasiswa, Kode Prodi, Nama Prodi, dan IPK terformat.
-- ----------------------------------------------------------------------------
SELECT 
    s.StudentId,
    s.NIM,
    s.Name AS NamaMahasiswa,
    p.ProgramCode AS KodeProdi,
    p.ProgramName AS ProgramStudi,
    FORMAT(s.GPA, 'N2') AS IPK,
    s.Email,
    s.PhoneNumber AS Telepon,
    CONVERT(VARCHAR(10), s.BirthDate, 120) AS TanggalLahir,
    s.Address AS Alamat
FROM dbo.Students s
INNER JOIN dbo.Programs p ON s.ProgramId = p.ProgramId
ORDER BY p.ProgramName ASC, s.Name ASC;
GO

-- ----------------------------------------------------------------------------
-- CHALLENGE 2: REKAPITULASI PROGRAM STUDI (LEFT JOIN & AGREGASI GROUP BY)
-- Menampilkan semua program studi, total mahasiswa terdaftar, rata-rata IPK,
-- dan jumlah mahasiswa aktif (termasuk prodi yang belum memiliki mahasiswa).
-- ----------------------------------------------------------------------------
SELECT 
    p.ProgramId,
    p.ProgramCode AS KodeProdi,
    p.ProgramName AS ProgramStudi,
    COUNT(s.StudentId) AS TotalMahasiswa,
    ISNULL(FORMAT(AVG(s.GPA), 'N2'), '0.00') AS RataRataIPK,
    SUM(CASE WHEN s.IsActive = 1 THEN 1 ELSE 0 END) AS MahasiswaAktif,
    SUM(CASE WHEN s.GPA >= 3.51 THEN 1 ELSE 0 END) AS MahasiswaCumlaude
FROM dbo.Programs p
LEFT JOIN dbo.Students s ON p.ProgramId = s.ProgramId
GROUP BY p.ProgramId, p.ProgramCode, p.ProgramName
ORDER BY TotalMahasiswa DESC, p.ProgramName ASC;
GO

-- ----------------------------------------------------------------------------
-- CHALLENGE 3: FILTER MAHASISWA BERPRESTASI (PREDIKAT CUMLAUDE IPK >= 3.51)
-- Menampilkan mahasiswa dengan IPK dengan pujian diurutkan dari IPK tertinggi.
-- ----------------------------------------------------------------------------
SELECT 
    s.NIM,
    s.Name AS NamaLengkap,
    p.ProgramName AS ProgramStudi,
    s.GPA AS IPK,
    DATEDIFF(YEAR, s.BirthDate, GETDATE()) AS PerkiraanUsia,
    s.Email
FROM dbo.Students s
INNER JOIN dbo.Programs p ON s.ProgramId = p.ProgramId
WHERE s.GPA >= 3.51
ORDER BY s.GPA DESC, s.Name ASC;
GO

-- ----------------------------------------------------------------------------
-- CHALLENGE 4: DISTRIBUSI PREDIKAT KELULUSAN KESELURUHAN (AGGREGATION)
-- Mengelompokkan seluruh mahasiswa ke dalam kategori predikat akademik ITS.
-- ----------------------------------------------------------------------------
SELECT 
    CASE 
        WHEN s.GPA >= 3.51 THEN 'Cumlaude (Dengan Pujian)'
        WHEN s.GPA >= 3.00 THEN 'Sangat Memuaskan'
        WHEN s.GPA >= 2.50 THEN 'Memuaskan'
        ELSE 'Cukup'
    END AS PredikatAkademik,
    COUNT(*) AS JumlahMahasiswa,
    FORMAT(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM dbo.Students), 'N1') + '%' AS Persentase
FROM dbo.Students s
GROUP BY 
    CASE 
        WHEN s.GPA >= 3.51 THEN 'Cumlaude (Dengan Pujian)'
        WHEN s.GPA >= 3.00 THEN 'Sangat Memuaskan'
        WHEN s.GPA >= 2.50 THEN 'Memuaskan'
        ELSE 'Cukup'
    END
ORDER BY MIN(s.GPA) DESC;
GO

-- ----------------------------------------------------------------------------
-- CHALLENGE 5: SIMULASI PARAMETERIZED SEARCH QUERY (ADO.NET EQUIVALENT)
-- Mensimulasikan query yang dijalankan oleh ADO.NET dengan filter kata kunci.
-- ----------------------------------------------------------------------------
DECLARE @Keyword NVARCHAR(100) = N'Pratama';
DECLARE @FilterProgramId INT = 0; -- 0 berarti semua prodi

SELECT 
    s.StudentId,
    s.NIM,
    s.Name,
    p.ProgramName,
    s.GPA,
    s.IsActive
FROM dbo.Students s
INNER JOIN dbo.Programs p ON s.ProgramId = p.ProgramId
WHERE (@Keyword IS NULL OR @Keyword = '' OR s.NIM LIKE '%' + @Keyword + '%' OR s.Name LIKE '%' + @Keyword + '%')
  AND (@FilterProgramId = 0 OR s.ProgramId = @FilterProgramId)
ORDER BY s.StudentId ASC;
GO
