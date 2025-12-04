using DiabetesRisk.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace DiabetesRisk.Database
{
    public class PatientDataService(IConfiguration config)
    {
        private readonly string _connectionString = config.GetConnectionString("DefaultConnection");

        public async Task InsertPatientsAsync(List<Patient> patients)
        {
            if (patients == null || patients.Count == 0)
            {
                return;
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                SqlTransaction transaction = connection.BeginTransaction();
                SqlCommand command = new("InsertPatient", connection, transaction)
                {
                    Connection = connection,
                    Transaction = transaction
                };

                try
                {
                    foreach (var patient in patients)
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Id", patient.Id);
                        command.Parameters.AddWithValue("@Name", patient.Name);
                        command.Parameters.AddWithValue("@Age", patient.Age);
                        command.Parameters.AddWithValue("@Gender", patient.Gender);
                        command.Parameters.AddWithValue("@BMI", patient.BMI);
                        command.Parameters.AddWithValue("@Glucose", patient.Glucose);
                        command.Parameters.AddWithValue("@Insulin", patient.Insulin);
                        command.Parameters.AddWithValue("@BloodPressure", patient.BloodPressure);
                        command.Parameters.AddWithValue("@DiabetesPedigree", patient.DiabetesPedigree);
                        command.Parameters.AddWithValue("@PhysicalActivityHours", patient.PhysicalActivityHours);
                        command.Parameters.AddWithValue("@RiskScore", patient.RiskScore);

                        var inserted = await command.ExecuteNonQueryAsync(); // if 1, display success?
                        command.Parameters.Clear();
                        Console.WriteLine($"inserted patient: {inserted}");
                    }
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Something went wrong: {ex.Message}");
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        Console.WriteLine($"Rollback failed: {ex2.Message}");
                    }
                }
                finally
                {
                    transaction.Dispose();
                    command.Dispose();
                }
            }
        }

        public async Task<List<Patient>> GetTopPatientsAsync()
        {
            Patient patient;
            List<Patient> patients = [];

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            SqlCommand command = new("GetTopPatients", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            try
            {
                SqlDataReader reader = command.ExecuteReader();
                Console.WriteLine($"reader.HasRows: {reader.HasRows}");
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        patient = GeneratePatient(reader);
                        Console.WriteLine($"patient.Name: {patient.Name}");
                        patients.Add(patient);
                    }
                }
                reader.Close();
                return patients;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ex.Message: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return patients;
            }
            finally
            {
                command.Dispose();
                connection.Close();
            }
        }

        private static Patient GeneratePatient(SqlDataReader reader)
        {
            Console.WriteLine($"reader.FieldCount: {reader.FieldCount}");
            Console.WriteLine($"reader.Depth: {reader.Depth}");

            var generatedPatient = new Patient()
            {
                Name = reader.GetString(0),
                RiskScore = reader.GetInt32(1),
                Glucose = reader.GetInt32(2),
            };

            return generatedPatient;
        }
    }
}
