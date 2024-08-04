using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesWPFApp
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetProducts();
        Product GetProductByID(int id);
        void InsertProduct(Product mem);
        void DeleteProduct(int id);
        void UpdateProduct(Product mem);
        IEnumerable<Product> SearchProduct(string str);
    }
}
