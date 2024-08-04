using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;



namespace SalesWPFApp
{
    public class ProductRepository : IProductRepository
    {
        public IEnumerable<Product> GetProducts() => ProductDAO.Instance.GetProducts();
        public Product GetProductByID(int id) => ProductDAO.Instance.GetProductById(id);
        public void InsertProduct(Product pro) => ProductDAO.Instance.AddNew(pro);
        public void DeleteProduct(int id) => ProductDAO.Instance.Remove(id);
        public void UpdateProduct(Product pro) => ProductDAO.Instance.Update(pro);
        public IEnumerable<Product> SearchProduct(string str) => ProductDAO.Instance.Search(str);
    }
}
