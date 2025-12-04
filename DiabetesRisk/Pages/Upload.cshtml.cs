using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IO;
using DiabetesRisk.Models;
using DiabetesRisk.Database;

namespace DiabetesRisk.Pages
{
    public class UploadModel(PatientDataService pds) : PageModel
    {
        private readonly PatientDataService _patientDataService = pds;
        public required IFormFile Upload { get; set; }
        public bool UploadSuccessful { get; set; } = false;
        public bool UploadFailed { get; set; } = false;
        public List<Patient> PatientRecords { get; set; } = [];

        public async Task OnPostAsync()
        {
            if (Upload == null || Upload.Length == 0)
            {
                return;
            }

            PatientRecords.Clear();

            /////////////////////////////////////////
            // ADD FILE VALIDATION IF THERE'S TIME //
            /////////////////////////////////////////

            using (StreamReader reader = new(Upload.OpenReadStream()))
            {
                await reader.ReadLineAsync();

                string? record;
                try
                {
                    while ((record = await reader.ReadLineAsync()) != null)
                    {
                        var fields = record.Split(',');
                        var patientRecord = CreatePatient(fields);
                        if (patientRecord != null)
                        {
                            PatientRecords.Add(patientRecord);
                        }
                    }
                    await _patientDataService.InsertPatientsAsync(PatientRecords);
                    UploadSuccessful = true;
                }
                catch (Exception ex)
                {
                    UploadFailed = true;
                    Console.WriteLine($"ex.Message: {ex.Message}");
                }
            }
        }        

        public Patient? CreatePatient(string[] fields)
        {
            Console.WriteLine($"CreatePatient start...");
            try
            {
                var patient = new Patient
                {
                    Id = int.Parse(fields[0].Trim()),
                    Name = fields[1].Trim(),
                    Age = int.Parse(fields[2].Trim()),
                    Gender = fields[3].Trim(),
                    BMI = float.Parse(fields[4].Trim()),
                    Glucose = int.Parse(fields[5].Trim()),
                    Insulin = int.Parse(fields[6].Trim()),
                    BloodPressure = int.Parse(fields[7].Trim()),
                    DiabetesPedigree = float.Parse(fields[8].Trim()),
                    PhysicalActivityHours = int.Parse(fields[9].Trim()),
                    RiskScore = CalculateRiskScore(fields),
                };
                Console.WriteLine($"CreatePatient end, patient.RiskScore: {patient.Name} - {patient.RiskScore}");
                return patient;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ex.Message: {ex.Message}");
                return null;
            }
        }

        private int CalculateRiskScore(string[] fields)
        {
            int score = 0;

            var bmi = float.Parse(fields[4].Trim());
            var glucose = int.Parse(fields[5].Trim());
            var diabetesPedigree = float.Parse(fields[8].Trim());
            var physicalActivityHours = int.Parse(fields[9].Trim());
            Console.WriteLine($"bmi: {bmi}, glucose: {glucose}, diabetes: {diabetesPedigree}, phys: {physicalActivityHours}");

            if (bmi > 30)
            {
                score++;
            }
            if (glucose > 140)
            {
                score++;
            }
            if (physicalActivityHours < 2)
            {
                score++;
            }
            if (diabetesPedigree > 0.8)
            {
                score++;
            }

            return score;
        }
    }
}
