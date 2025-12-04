using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using DiabetesRisk.Models;

namespace DiabetesRisk.Pages
{
    public class UploadModel : PageModel
    {
        public List<Patient> PatientRecords { get; set; } = [];
        public async Task OnPostAsync(IFormFile Upload)
        {
            if (Upload == null || Upload.Length == 0)
            {
                return;
            }

            using (StreamReader reader = new(Upload.OpenReadStream()))
            {
                await reader.ReadLineAsync();

                string? record;
                while ((record = await reader.ReadLineAsync()) != null) 
                {
                    var fields = record.Split(',');
                    try
                    {
                        var patientRecord = CreatePatient(fields);
                        if (patientRecord != null)
                        {
                            PatientRecords.Add(patientRecord);
                            // add patientRecord to the DB using ADO stored procedures
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"ex.Message: {ex.Message}");
                    }
                }
            }
        }

        public Patient? CreatePatient(string[] fields)
        {
            try
            {
                var patient = new Patient
                {
                    Id = int.Parse(fields[0].Trim()),
                    Name = fields[1].Trim(),
                    Age = int.Parse(fields[2].Trim()),
                    Gender = fields[3].Trim(),
                    BMI = double.Parse(fields[4].Trim()),
                    Glucose = int.Parse(fields[5].Trim()),
                    Insulin = int.Parse(fields[6].Trim()),
                    BloodPressure = int.Parse(fields[7].Trim()),
                    DiabetesPedigree = double.Parse(fields[8].Trim()),
                    PhysicalActivityHours = int.Parse(fields[9].Trim()),
                };

                return patient;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ex.Message: {ex.Message}");
                return null;
            }
        }
    }
}
