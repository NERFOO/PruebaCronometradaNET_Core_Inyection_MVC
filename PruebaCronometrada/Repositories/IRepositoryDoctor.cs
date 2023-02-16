using PruebaCronometrada.Models;

namespace PruebaCronometrada.Repositories
{
    public interface IRepositoryDoctor
    {
        List<Doctor> GetDoctores();
        List<Hospital> GetHospitales();
        List<string> GetEspecialidades();
        Doctor FindDoctor(int doctorNum);
        List<Doctor> FindDoctorEspecialidad(string especialidad);
        void CreateDoctor(int hospitalCod, string apellido, string especialidad, int salario);
        void DeleteDoctor(int doctorNum);
        void UpdateDoctor(int hospitalCod, int doctorNum, string apellido, string especialidad, int salario);
    }
}
