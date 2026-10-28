using StudentRegistration.Models;

namespace StudentRegistration.Repositories
{
    public interface IStudentRepository
    {
        List<Student> GetAll(string? keyword = null, int? programId = null, bool? isActive = null);
        Student? GetById(int studentId);
        Student? GetByNIM(string nim);
        bool Insert(Student student, out string errorMessage);
        bool Update(Student student, out string errorMessage);
        bool Delete(int studentId, out string errorMessage);
        List<ProgramStudy> GetAllPrograms();
        bool CheckConnection(out string serverInfo, out string errorMessage);
        bool ProvisionDatabase(out string statusMessage);
    }
}
