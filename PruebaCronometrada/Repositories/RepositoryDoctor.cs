using PruebaCronometrada.Models;
using System.Data;
using System.Data.SqlClient;


#region
/*
 CREATE PROCEDURE SP_CREATE_DOCTOR
(@HOSPITALCOD INT, @DOCTORNUM INT, @APELLIDO NVARCHAR(50), @ESPECIALIDAD NVARCHAR(50), @SALARIO INT)
AS
	INSERT INTO DOCTOR VALUES(@HOSPITALCOD, @DOCTORNUM, @APELLIDO, @ESPECIALIDAD, @SALARIO) 
GO

CREATE PROCEDURE SP_DELETE_DOCTOR
(@DOCTORNUM INT)
AS
	DELETE FROM DOCTOR WHERE DOCTOR_NO = @DOCTORNUM 
GO

CREATE PROCEDURE SP_UPDATE_DOCTOR
(@HOSPITALCOD INT, @DOCTORNUM INT, @APELLIDO NVARCHAR(50), @ESPECIALIDAD NVARCHAR(50), @SALARIO INT)
AS
	UPDATE DOCTOR SET HOSPITAL_COD = @HOSPITALCOD, APELLIDO = @APELLIDO, ESPECIALIDAD = @ESPECIALIDAD, SALARIO = @SALARIO WHERE DOCTOR_NO = @DOCTORNUM
GO
 */
#endregion

namespace PruebaCronometrada.Repositories
{
    public class RepositoryDoctor : IRepositoryDoctor
    {
        SqlConnection connection;
        SqlCommand command;

        SqlDataAdapter adapterDoctor;
        DataTable tablaDoctor;

        SqlDataAdapter adapterHospital;
        DataTable tablaHospital;

        public RepositoryDoctor()
        {
            string connectionString = @"Data Source=LOCALHOST\SQLEXPRESS;Initial Catalog=HOSPITAL;Persist Security Info=True;User ID=sa;Password=MCSD2022";

            //Sql
            this.connection = new SqlConnection(connectionString);
            this.command = new SqlCommand();
            this.command.Connection = this.connection;

            //Linq
            string consultaDoctor = "SELECT * FROM DOCTOR";
            this.adapterDoctor = new SqlDataAdapter(consultaDoctor, connectionString);
            this.tablaDoctor = new DataTable();
            this.adapterDoctor.Fill(this.tablaDoctor);


            string consultaHospital = "SELECT * FROM HOSPITAL";
            this.adapterHospital = new SqlDataAdapter(consultaHospital, connectionString);
            this.tablaHospital = new DataTable();
            this.adapterHospital.Fill(this.tablaHospital);
        }

        public List<Doctor> GetDoctores()
        {
            var consulta = from datos in this.tablaDoctor.AsEnumerable()
                           select datos;
            List<Doctor> doctores = new List<Doctor>();

            foreach(var row in consulta)
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

            SqlParameter paramHospitalCod = new SqlParameter("@HOSPITALCOD", hospitalCod);
            SqlParameter paramDoctorNum = new SqlParameter("@DOCTORNUM", maximo);
            SqlParameter paramApellido = new SqlParameter("@APELLIDO", apellido);
            SqlParameter paramEspecialidad = new SqlParameter("@ESPECIALIDAD", especialidad);
            SqlParameter paramSalario = new SqlParameter("@SALARIO", salario);

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
            SqlParameter paramDoctorNum = new SqlParameter("@DOCTORNUM", doctorNum);
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
            SqlParameter paramHospitalCod = new SqlParameter("@HOSPITALCOD", hospitalCod);
            SqlParameter paramDoctorNum = new SqlParameter("@DOCTORNUM", doctorNum);
            SqlParameter paramApellido = new SqlParameter("@APELLIDO", apellido);
            SqlParameter paramEspecialidad = new SqlParameter("@ESPECIALIDAD", especialidad);
            SqlParameter paramSalario = new SqlParameter("@SALARIO", salario);

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

        public List<Doctor> FindDoctorEspecialidad(string especialidad)
        {
            var consulta = (from datos in this.tablaDoctor.AsEnumerable()
                            where datos.Field<string>("ESPECIALIDAD") == especialidad
                            select new Doctor
                            {
                                HospitalCod = datos.Field<int>("HOSPITAL_COD"),
                                DoctorNum = datos.Field<int>("DOCTOR_NO"),
                                Apellido = datos.Field<string>("APELLIDO"),
                                Especialidad = datos.Field<string>("ESPECIALIDAD"),
                                Salario = datos.Field<int>("SALARIO")
                            }).Distinct();

            return consulta.ToList();
        }
    }
}
