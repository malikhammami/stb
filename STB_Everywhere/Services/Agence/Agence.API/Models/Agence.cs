namespace Agence.API.Models
{
    public class Agence
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Governance { get; set; }
        public string Address { get; set; }
        public int CodeBCT { get; set; }
        public int PhoneNumber { get; set; }
        public string Email {  get; set; }
        public string ResponsibleName { get; set; }
    }
}
