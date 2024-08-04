using System.ComponentModel.DataAnnotations;

namespace SalesWPFApp
{
    public class Member
    {
        [Key]
        public int Id { get; set; }
        public string Avt { get; set; }
        public string Name { get; set; }

        [StringLength(100)]
        public string Email { get; set; }
        [StringLength(15)]
        public string City { get; set; }
        [StringLength(15)]
        public string Country { get; set; }
        [StringLength(30)]
        public string Password { get; set; }
        public DateTime Birthday { get; set; }
        public string Phone { get; set; }
        public string Hobby { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}