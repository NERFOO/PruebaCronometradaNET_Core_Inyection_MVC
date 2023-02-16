using PruebaCronometrada.Models;
using System.Data.SqlClient;
using System.Data;
using Oracle.ManagedDataAccess.Client;

#region
/*
 select * from doctor;
 select * from hospital;

 create or replace procedure SP_CREATE_DOCTOR
(p_hospitalCod doctor.hospital_cod%type
, p_doctorNum doctor.doctor_no%type
, p_apellido doctor.apellido%type
, p_especialidad doctor.especialidad%type
, p_salario doctor.salario%type)
as
begin
  insert into doctor values 
  (p_hospitalCod, p_doctorNum, p_apellido, p_especialidad, p_salario);
commit;
end;

create or replace procedure SP_UPDATE_DOCTOR
(p_hospitalCod doctor.hospital_cod%type
, p_doctorNum doctor.doctor_no%type
, p_apellido doctor.apellido%type
, p_especialidad doctor.especialidad%type
, p_salario doctor.salario%type)
as
begin
  update doctor set hospital_cod=p_hospitalCod, apellido=p_apellido, especialidad=p_especialidad, salario=p_salario
  where doctor_no = p_doctorNum;
commit;
end;

create or replace procedure SP_DELETE_DOCTOR
(p_doctorNum doctor.doctor_no%type)
as
begin
  delete from doctor where doctor_no = p_doctorNum;
commit;
end;
 */
#endregion

namespace PruebaCronometrada.Repositories
{
    public class RepositoryDoctorOracle : IRepositoryDoctor
    {
        OracleConnection connection;
        OracleCommand command;

        OracleDataAdapter adapterDoctor;
        DataTable tablaDoctor;

        OracleDataAdapter adapterHospital;
        DataTable tablaHospital;

        public RepositoryDoctorOracle()
        {
            string connectionString = @"Data Source=LOCALHOST:1521/XE; Persist Security Info=True;User Id=system;Password=ORACLE";

            //Sql
            this.connection = new OracleConnection(connectionString);
            this.command = new OracleCommand();
            this.command.Connection = this.connection;

            //Linq
            string consultaDoctor = "select * from doctor";
            this.adapterDoctor = new OracleDataAdapter(consultaDoctor, connectionString);
            this.tablaDoctor = new DataTable();
            this.adapterDoctor.Fill(this.tablaDoctor);


            string consultaHospital = "select * from hospital";
            this.adapterHospital = new OracleDataAdapter(consultaHospital, connectionString);
            this.tablaHospital = new DataTable();
            this.adapterHospital.Fill(this.tablaHospital);
        }

        public List<Doctor> GetDoctores()
        {
            var consulta = from datos in this.tablaDoctor.AsEnumerable()
                           select datos;
            List<Doctor> doctores = new List<Doctor>();

            foreach (var row in consulta)
            {
                Doctor doctor = new Doctor
                {
                    HospitalCod = row.Field<int>("HOSPITAL_COD"),
                    DoctorNum = row.Field<int>("DOCTOR_NO"),
                    Apellido = row.Field<string>("APELLIDO"),
                    Especialidad = row.Field<string>("ESPECIALIDAD"),
                    Salario = row.Field<int>("SALARIO")
                };
                doctores.Add(doctor);
            }
            return doctores;
        }

        public Doctor FindDoctor(int doctorNum)
        {
            var consulta = from datos in this.tablaDoctor.AsEnumerable()
                           where datos.Field<int>("DOCTOR_NO") == doctorNum
                           select new Doctor
                           {
                               HospitalCod = datos.Field<int>("HOSPITAL_COD"),
                               DoctorNum = datos.Field<int>("DOCTOR_NO"),
                               Apellido = datos.Field<string>("APELLIDO"),
                               Especialidad = datos.Field<string>("ESPECIALIDAD"),
                               Salario = datos.Field<int>("SALARIO")
                           };

            return consulta.FirstOrDefault();
        }

        private int GetMaximo()
        {
            var consulta = (from datos in this.tablaDoctor.AsEnumerable()
                            select datos).Max(x => x.Field<int>("DOCTOR_NO") + 1);

            return consulta;
        }

        public List<Hospital> GetHospitales()
        {
            var consulta = from datos in this.tablaHospital.AsEnumerable()
                           select new Hospital
                           {
                               HospitalCod = datos.Field<int>("HOSPITAL_COD"),
                               Nombre = datos.Field<string>("NOMBRE"),
                               Direccion = datos.Field<string>("DIRECCION"),
                               Telefono = datos.Field<string>("TELEFONO"),
                               NumCama = datos.Field<int>("NUM_CAMA")
                           };

            return consulta.ToList();
        }

        public List<string> GetEspecialidades()
        {
            var consulta = (from datos in this.tablaDoctor.AsEnumerable()
                            select datos.Field<string>("ESPECIALIDAD")).Distinct();

            return consulta.ToList();
        }

        public void CreateDoctor(int hospitalCod, string apellido, string especialidad, int salario)
        {
            int maximo = this.GetMaximo();

            OracleParameter paramHospitalCod = new OracleParameter("p_hospitalCod", hospitalCod);
            OracleParameter paramDoctorNum = new OracleParameter("p_doctorNum", maximo);
            OracleParameter paramApellido = new OracleParameter("p_apellido", apellido);
            OracleParameter paramEspecialidad = new OracleParameter("p_especialidad", especialidad);
            OracleParameter paramSalario = new OracleParameter("p_salario", salario);

            this.command.Parameters.Add(paramHospitalCod);
            this.command.Parameters.Add(paramDoctorNum);
            this.command.Parameters.Add(paramApellido);
            this.command.Parameters.Add(paramEspecialidad);
            this.command.Parameters.Add(paramSalario);

            this.command.CommandType = CommandType.StoredProcedure;
            this.command.CommandText = "SP_CREATE_DOCTOR";

            this.connection.Open();
            this.command.ExecuteNonQuery();

            this.connection.Close();
            this.command.Parameters.Clear();
        }

        public void DeleteDoctor(int doctorNum)
        {
            OracleParameter paramDoctorNum = new OracleParameter("@DOCTORNUM", doctorNum);
            this.command.Parameters.Add(paramDoctorNum);

            this.command.CommandType = CommandType.StoredProcedure;
            this.command.CommandText = "SP_DELETE_DOCTOR";

            this.connection.Open();
            this.command.ExecuteNonQuery();

            this.connection.Close();
            this.command.Parameters.Clear();
        }

        public void UpdateDoctor(int hospitalCod, int doctorNum, string apellido, string especialidad, int salario)
        {
            OracleParameter paramHospitalCod = new OracleParameter("@HOSPITALCOD", hospitalCod);
            OracleParameter paramDoctorNum = new OracleParameter("@DOCTORNUM", doctorNum);
            OracleParameter paramApellido = new OracleParameter("@APELLIDO", apellido);
            OracleParameter paramEspecialidad = new OracleParameter("@ESPECIALIDAD", especialidad);
            OracleParameter paramSalario = new OracleParameter("@SALARIO", salario);

            this.command.Parameters.Add(paramHospitalCod);
            this.command.Parameters.Add(paramDoctorNum);
            this.command.Parameters.Add(paramApellido);
            this.command.Parameters.Add(paramEspecialidad);
            this.command.Parameters.Add(paramSalario);

            this.command.CommandType = CommandType.StoredProcedure;
            this.command.CommandText = "SP_UPDATE_DOCTOR";

            this.connection.Open();
            this.command.ExecuteNonQuery();

            this.connection.Close();
            this.command.Parameters.Clear();
        }
    }
}
