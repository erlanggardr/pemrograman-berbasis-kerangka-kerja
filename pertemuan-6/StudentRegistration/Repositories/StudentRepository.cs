using System.Data;
using Microsoft.Data.SqlClient;
using StudentRegistration.Config;
using StudentRegistration.Models;

namespace StudentRegistration.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly string _connectionString;

        public StudentRepository(string? connectionString = null)
        {
            _connectionString = connectionString ?? DatabaseConfig.ConnectionString;
        }

        public List<Student> GetAll(string? keyword = null, int? programId = null, bool? isActive = null)
        {
            var students = new List<Student>();

            const string query = @"
                SELECT 
                    s.StudentId,
                    s.NIM,
                    s.Name,
                    s.ProgramId,
                    p.ProgramCode,
                    p.ProgramName,
                    s.BirthDate,
                    s.Address,
                    s.PhoneNumber,
                    s.Email,
                    s.GPA,
                    s.IsActive,
                    s.CreatedAt
                FROM dbo.Students s
                INNER JOIN dbo.Programs p ON s.ProgramId = p.ProgramId
                WHERE (@Keyword IS NULL OR @Keyword = '' OR s.NIM LIKE @SearchPattern OR s.Name LIKE @SearchPattern OR s.Email LIKE @SearchPattern)
                  AND (@ProgramId IS NULL OR @ProgramId = 0 OR s.ProgramId = @ProgramId)
                  AND (@IsActive IS NULL OR s.IsActive = @IsActive)
                ORDER BY s.StudentId DESC;";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            string searchPattern = string.IsNullOrWhiteSpace(keyword) ? string.Empty : $"%{keyword.Trim()}%";
            command.Parameters.Add("@Keyword", SqlDbType.NVarChar, 100).Value = (object?)keyword?.Trim() ?? DBNull.Value;
            command.Parameters.Add("@SearchPattern", SqlDbType.NVarChar, 100).Value = string.IsNullOrWhiteSpace(searchPattern) ? (object)DBNull.Value : searchPattern;
            command.Parameters.Add("@ProgramId", SqlDbType.Int).Value = (object?)programId ?? DBNull.Value;
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = (object?)isActive ?? DBNull.Value;

            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                students.Add(MapStudentFromReader(reader));
            }

            return students;
        }

        public Student? GetById(int studentId)
        {
            const string query = @"
                SELECT 
                    s.StudentId,
                    s.NIM,
                    s.Name,
                    s.ProgramId,
                    p.ProgramCode,
                    p.ProgramName,
                    s.BirthDate,
                    s.Address,
                    s.PhoneNumber,
                    s.Email,
                    s.GPA,
                    s.IsActive,
                    s.CreatedAt
                FROM dbo.Students s
                INNER JOIN dbo.Programs p ON s.ProgramId = p.ProgramId
                WHERE s.StudentId = @StudentId;";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.Add("@StudentId", SqlDbType.Int).Value = studentId;

            connection.Open();
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapStudentFromReader(reader);
            }

            return null;
        }

        public Student? GetByNIM(string nim)
        {
            const string query = @"
                SELECT 
                    s.StudentId,
                    s.NIM,
                    s.Name,
                    s.ProgramId,
                    p.ProgramCode,
                    p.ProgramName,
                    s.BirthDate,
                    s.Address,
                    s.PhoneNumber,
                    s.Email,
                    s.GPA,
                    s.IsActive,
                    s.CreatedAt
                FROM dbo.Students s
                INNER JOIN dbo.Programs p ON s.ProgramId = p.ProgramId
                WHERE s.NIM = @NIM;";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.Add("@NIM", SqlDbType.VarChar, 20).Value = nim.Trim();

            connection.Open();
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapStudentFromReader(reader);
            }

            return null;
        }

        public bool Insert(Student student, out string errorMessage)
        {
            errorMessage = string.Empty;

            const string query = @"
                INSERT INTO dbo.Students 
                    (NIM, Name, ProgramId, BirthDate, Address, PhoneNumber, Email, GPA, IsActive, CreatedAt)
                VALUES 
                    (@NIM, @Name, @ProgramId, @BirthDate, @Address, @PhoneNumber, @Email, @GPA, @IsActive, SYSDATETIME());
                SELECT SCOPE_IDENTITY();";

            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand(query, connection);

                AddStudentParameters(command, student);

                connection.Open();
                var insertedId = command.ExecuteScalar();
                if (insertedId != null && decimal.TryParse(insertedId.ToString(), out decimal id))
                {
                    student.StudentId = (int)id;
                    return true;
                }
                return false;
            }
            catch (SqlException ex)
            {
                errorMessage = FormatSqlException(ex);
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = $"Terjadi kesalahan umum: {ex.Message}";
                return false;
            }
        }

        public bool Update(Student student, out string errorMessage)
        {
            errorMessage = string.Empty;

            const string query = @"
                UPDATE dbo.Students
                SET 
                    Name = @Name,
                    ProgramId = @ProgramId,
                    BirthDate = @BirthDate,
                    Address = @Address,
                    PhoneNumber = @PhoneNumber,
                    Email = @Email,
                    GPA = @GPA,
                    IsActive = @IsActive
                WHERE StudentId = @StudentId;";

            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand(query, connection);

                command.Parameters.Add("@StudentId", SqlDbType.Int).Value = student.StudentId;
                AddStudentParameters(command, student);

                connection.Open();
                int rows = command.ExecuteNonQuery();
                return rows > 0;
            }
            catch (SqlException ex)
            {
                errorMessage = FormatSqlException(ex);
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = $"Terjadi kesalahan umum: {ex.Message}";
                return false;
            }
        }

        public bool Delete(int studentId, out string errorMessage)
        {
            errorMessage = string.Empty;

            const string query = "DELETE FROM dbo.Students WHERE StudentId = @StudentId;";

            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand(query, connection);
                command.Parameters.Add("@StudentId", SqlDbType.Int).Value = studentId;

                connection.Open();
                int rows = command.ExecuteNonQuery();
                return rows > 0;
            }
            catch (SqlException ex)
            {
                errorMessage = FormatSqlException(ex);
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = $"Terjadi kesalahan umum: {ex.Message}";
                return false;
            }
        }

        public List<ProgramStudy> GetAllPrograms()
        {
            var programs = new List<ProgramStudy>();
            const string query = @"
                SELECT ProgramId, ProgramCode, ProgramName, IsActive, CreatedAt
                FROM dbo.Programs
                WHERE IsActive = 1
                ORDER BY ProgramName ASC;";

            try
            {
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand(query, connection);

                connection.Open();
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    programs.Add(new ProgramStudy
                    {
                        ProgramId = reader.GetInt32(reader.GetOrdinal("ProgramId")),
                        ProgramCode = reader.GetString(reader.GetOrdinal("ProgramCode")),
                        ProgramName = reader.GetString(reader.GetOrdinal("ProgramName")),
                        IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                    });
                }
            }
            catch
            {
                // Return empty list if database not ready yet
            }

            return programs;
        }

        public bool CheckConnection(out string serverInfo, out string errorMessage)
        {
            serverInfo = string.Empty;
            errorMessage = string.Empty;

            try
            {
                using var connection = new SqlConnection(_connectionString);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = "SELECT @@VERSION AS Version, DB_NAME() AS CurrentDB;";
                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    string ver = reader.GetString(0);
                    string db = reader.GetString(1);
                    string shortVer = ver.Split('\n')[0];
                    serverInfo = $"Terkoneksi ke {DatabaseConfig.ServerName} ({db}) — {shortVer}";
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public bool ProvisionDatabase(out string statusMessage)
        {
            statusMessage = string.Empty;
            try
            {
                // 1. Buat Database via master jika belum ada
                using (var masterConn = new SqlConnection(DatabaseConfig.MasterConnectionString))
                {
                    masterConn.Open();
                    using var cmd = masterConn.CreateCommand();
                    cmd.CommandText = @"
                        IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'StudentRegistrationDB')
                        BEGIN
                            CREATE DATABASE StudentRegistrationDB;
                        END";
                    cmd.ExecuteNonQuery();
                }

                // 2. Buat tabel dan seed data via StudentRegistrationDB
                using (var dbConn = new SqlConnection(DatabaseConfig.ConnectionString))
                {
                    dbConn.Open();
                    using var cmd = dbConn.CreateCommand();
                    cmd.CommandText = @"
                        IF OBJECT_ID(N'dbo.Programs', N'U') IS NULL
                        BEGIN
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

                            INSERT INTO dbo.Programs (ProgramCode, ProgramName, IsActive)
                            VALUES
                                ('TI',  'Teknik Informatika', 1),
                                ('SI',  'Sistem Informasi', 1),
                                ('SD',  'Sains Data', 1),
                                ('TK',  'Teknik Komputer', 1),
                                ('TE',  'Teknik Elektro', 1),
                                ('DKV', 'Desain Komunikasi Visual', 1),
                                ('RPL', 'Rekayasa Perangkat Lunak', 1);
                        END

                        IF OBJECT_ID(N'dbo.Students', N'U') IS NULL
                        BEGIN
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
                                CONSTRAINT FK_Students_Programs FOREIGN KEY (ProgramId) REFERENCES dbo.Programs (ProgramId),
                                CONSTRAINT CK_Students_GPA CHECK (GPA >= 0.00 AND GPA <= 4.00)
                            );

                            INSERT INTO dbo.Students (NIM, Name, ProgramId, BirthDate, Address, PhoneNumber, Email, GPA, IsActive)
                            VALUES
                                ('5025241001', 'Muhammad Rizky Pratama', 1, '2005-04-12', 'Jl. Gebang Wetan No. 15, Surabaya',      '081234567801', 'rizky.pratama@mhs.its.ac.id', 3.88, 1),
                                ('5025241025', 'Aulia Dewi Safitri',     2, '2005-08-23', 'Jl. Keputih Perintis No. 8, Surabaya',     '081234567802', 'aulia.safitri@mhs.its.ac.id', 3.74, 1),
                                ('5025241054', 'Dimas Arya Nugroho',     3, '2006-01-15', 'Jl. Mulyosari Prima No. 42, Surabaya',    '081234567803', 'dimas.arya@mhs.its.ac.id',    3.65, 1),
                                ('5025241078', 'Vania Kusuma Wardhani',  1, '2005-11-30', 'Perumahan Galaxy Bumi Permai, Surabaya',   '081234567804', 'vania.wardhani@mhs.its.ac.id', 3.92, 1),
                                ('5025241099', 'Fajar Hidayatullah',     4, '2004-06-19', 'Jl. Kejawan Putih Tambak No. 12, Surabaya','081234567805', 'fajar.hidayat@mhs.its.ac.id', 3.25, 0),
                                ('5025241112', 'Gabriella Natasha',      5, '2003-09-05', 'Jl. Arief Rahman Hakim No. 88, Surabaya',  '081234567806', 'gabriella.n@mhs.its.ac.id',   3.80, 1),
                                ('5025241130', 'Reza Ananta Putra',      7, '2005-03-27', 'Jl. Manyar Sabrangan No. 3, Surabaya',    '081234567807', 'reza.ananta@mhs.its.ac.id',   3.68, 1),
                                ('5025241145', 'Siti Nurhaliza',         2, '2004-12-10', 'Jl. Medokan Asri Barat No. 20, Surabaya',  '081234567808', 'siti.nurhaliza@mhs.its.ac.id', 3.45, 1),
                                ('5025241160', 'Budi Santoso',           4, '2005-07-08', 'Jl. Semolowaru Elok No. 5, Surabaya',     '081234567809', 'budi.santoso@mhs.its.ac.id',  2.95, 1),
                                ('5025241179', 'Erlangga Rizqi Dwi R.',   1, '2005-02-18', 'Jl. Teknik Sipil Blok C, Sukolilo, Sby',   '081234567810', 'erlangga.rizqi@mhs.its.ac.id', 3.85, 1);
                        END";
                    cmd.ExecuteNonQuery();
                }

                statusMessage = "Database dan tabel berhasil diinisialisasi secara otomatis!";
                return true;
            }
            catch (Exception ex)
            {
                statusMessage = $"Gagal inisialisasi basis data: {ex.Message}";
                return false;
            }
        }

        private static Student MapStudentFromReader(SqlDataReader reader)
        {
            return new Student
            {
                StudentId = reader.GetInt32(reader.GetOrdinal("StudentId")),
                NIM = reader.GetString(reader.GetOrdinal("NIM")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                ProgramId = reader.GetInt32(reader.GetOrdinal("ProgramId")),
                ProgramCode = reader.GetString(reader.GetOrdinal("ProgramCode")),
                ProgramName = reader.GetString(reader.GetOrdinal("ProgramName")),
                BirthDate = reader.IsDBNull(reader.GetOrdinal("BirthDate")) ? null : reader.GetDateTime(reader.GetOrdinal("BirthDate")),
                Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? string.Empty : reader.GetString(reader.GetOrdinal("Address")),
                PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? string.Empty : reader.GetString(reader.GetOrdinal("PhoneNumber")),
                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? string.Empty : reader.GetString(reader.GetOrdinal("Email")),
                GPA = reader.IsDBNull(reader.GetOrdinal("GPA")) ? 0.00m : reader.GetDecimal(reader.GetOrdinal("GPA")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
            };
        }

        private static void AddStudentParameters(SqlCommand command, Student student)
        {
            command.Parameters.Add("@NIM", SqlDbType.VarChar, 20).Value = student.NIM.Trim();
            command.Parameters.Add("@Name", SqlDbType.VarChar, 100).Value = student.Name.Trim();
            command.Parameters.Add("@ProgramId", SqlDbType.Int).Value = student.ProgramId;
            command.Parameters.Add("@BirthDate", SqlDbType.Date).Value = (object?)student.BirthDate ?? DBNull.Value;
            command.Parameters.Add("@Address", SqlDbType.NVarChar, 255).Value = string.IsNullOrWhiteSpace(student.Address) ? (object)DBNull.Value : student.Address.Trim();
            command.Parameters.Add("@PhoneNumber", SqlDbType.VarChar, 20).Value = string.IsNullOrWhiteSpace(student.PhoneNumber) ? (object)DBNull.Value : student.PhoneNumber.Trim();
            command.Parameters.Add("@Email", SqlDbType.VarChar, 100).Value = string.IsNullOrWhiteSpace(student.Email) ? (object)DBNull.Value : student.Email.Trim();
            command.Parameters.Add("@GPA", SqlDbType.Decimal).Value = student.GPA;
            command.Parameters["@GPA"].Precision = 3;
            command.Parameters["@GPA"].Scale = 2;
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = student.IsActive;
        }

        private static string FormatSqlException(SqlException ex)
        {
            return ex.Number switch
            {
                2627 or 2601 => "Gagal: NIM tersebut sudah terdaftar dalam basis data (Pelanggaran Unique Constraint).",
                547 => "Gagal: Program Studi yang dipilih tidak valid atau data masih direferensikan tabel lain (Pelanggaran Foreign Key Constraint).",
                4060 => "Gagal membuka database: Database 'StudentRegistrationDB' belum ditemukan di SQL Server.",
                53 or 2 => "Gagal terhubung ke SQL Server: Pastikan SQL Server / LocalDB dalam kondisi berjalan.",
                _ => $"Kesalahan SQL ({ex.Number}): {ex.Message}"
            };
        }
    }
}
