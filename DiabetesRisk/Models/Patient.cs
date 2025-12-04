namespace DiabetesRisk.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int Age { get; set; }
        public required string Gender { get; set; }
        public double BMI { get; set; }
        public int Glucose { get; set; }
        public int Insulin { get; set; }
        public int BloodPressure { get; set; }
        public double DiabetesPedigree { get; set; }
        public int PhysicalActivityHours { get; set; }
    }
}
