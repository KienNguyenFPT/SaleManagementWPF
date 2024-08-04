using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesWPFApp
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public int MemberId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? RequiredDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public int? Freight { get; set; }
        [NotMapped]
        public int Total { get; set; }
        public Member Member { get; set; }
        public ICollection<OrderDetail> orderDetails { get; set; }
    }
}
