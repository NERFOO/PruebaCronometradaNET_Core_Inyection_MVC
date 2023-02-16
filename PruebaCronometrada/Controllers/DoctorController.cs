using Microsoft.AspNetCore.Mvc;
using PruebaCronometrada.Models;
using PruebaCronometrada.Repositories;

namespace PruebaCronometrada.Controllers
{
    public class DoctorController : Controller
    {

        IRepositoryDoctor irepo;

        public DoctorController(IRepositoryDoctor doctor)
        {
            this.irepo = doctor;
        }

        public IActionResult Index()
        {
            List<Doctor> doctores = this.irepo.GetDoctores();

            List<string> especialidades = this.irepo.GetEspecialidades();
            ViewData["ESPECIALIDADES"] = especialidades;

            return View(doctores);
        }

        [HttpPost]
        public IActionResult Index(string especialidad)
        {
            List<Doctor> doctores = this.irepo.FindDoctorEspecialidad(especialidad);

            List<string> especialidades = this.irepo.GetEspecialidades();
            ViewData["ESPECIALIDADES"] = especialidades;

            return View(doctores);
        }

        public IActionResult Details(int doctorNum)
        {
            Doctor doctor = this.irepo.FindDoctor(doctorNum);
            return View(doctor);
        }

        public IActionResult Delete(int doctorNum)
        {
            this.irepo.DeleteDoctor(doctorNum);
            return RedirectToAction("Index");
        }

        public IActionResult Create()
        {
            List<Hospital> hospitales = this.irepo.GetHospitales();
            ViewData["HOSPITALES"] = hospitales;

            List<string> especialidades = this.irepo.GetEspecialidades();
            ViewData["ESPECIALIDADES"] = especialidades;

            return View();
        }

        [HttpPost]
        public IActionResult Create(int hospitalCod, string apellido, string especialidad, int salario)
        {
            this.irepo.CreateDoctor(hospitalCod, apellido, especialidad, salario);
            return RedirectToAction("Index");
        }

        public IActionResult Update(int doctorNum)
        {
            Doctor doctor = this.irepo.FindDoctor(doctorNum);

            List<Hospital> hospitales = this.irepo.GetHospitales();
            ViewData["HOSPITALES"] = hospitales;

            List<string> especialidades = this.irepo.GetEspecialidades();
            ViewData["ESPECIALIDADES"] = especialidades;

            return View(doctor);
        }

        [HttpPost]
        public IActionResult Update(int hospitalCod, int doctorNum, string apellido, string especialidad, int salario)
        {
            this.irepo.UpdateDoctor(hospitalCod, doctorNum, apellido, especialidad, salario);
            return RedirectToAction("Index");
        }
    }
}
