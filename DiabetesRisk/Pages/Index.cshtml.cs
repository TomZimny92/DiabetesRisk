using DiabetesRisk.Database;
using DiabetesRisk.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DiabetesRisk.Pages
{
    public class IndexModel(PatientDataService pds) : PageModel
    {
        private readonly PatientDataService _patientDataService = pds;
        public List<Patient> TopFive = [];
        public async Task OnGet()
        {
            TopFive = await _patientDataService.GetTopPatientsAsync();
        }
    }
}
