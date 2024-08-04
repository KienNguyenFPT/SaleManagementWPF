using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesWPFApp
{
    public class Product : INotifyPropertyChanged
    {
        [Key]
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        [StringLength(40)]
        public string ProductName { get; set; }
        [StringLength(20)]
        public string Weight { get; set; }
        public int UnitPrice { get; set; }
        public int UnitsInStock { get; set; }
        private int _quantity = 0;
        [NotMapped]
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
