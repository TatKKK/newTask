namespace newTask.Models
{
    public class Company
    {
        public int? CompanyId { get; set; }
        public string Name { get; set; }
        public string TaxCode { get; set; }
        public string Address { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();

        public int? UserCount { get; set; }
    }

   
}
