using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SalesWPFApp
{
    public class ProductDAO
    {
        private readonly AppDbContext dbContext = new AppDbContext();
        public List<Product> GetProducts()
        {
            return dbContext.Products.ToList();
        }

        public List<Product> ProductList => GetProducts();



        private static ProductDAO instance = null;
        public static readonly object instanceLook = new object();
        public static ProductDAO Instance
        {
            get
            {
                lock (instanceLook)
                {
                    if (instance == null)
                    {
                        instance = new ProductDAO();
                    }
                    return instance;
                }
            }
        }

        public void AddNew(Product pro)
        {
            try
            {
                dbContext.Add(pro);
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Product is null!");
            }
        }

        public Product GetProductById(int id)
        {
            Product pro = ProductList.SingleOrDefault(pro => pro.ProductId == id);
            return pro;
        }

        public void Update(Product pro)
        {
            Product c = GetProductById(pro.ProductId);
            if (c != null)
            {
                var ind = ProductList.IndexOf(c);
                dbContext.ChangeTracker.Clear();
                dbContext.Update(pro);
                dbContext.SaveChanges();
            }
            else
            {
                throw new Exception("Member does not already exists!");
            }
        }

        public void Remove(int id)
        {
            Product p = GetProductById(id);
            if (p != null)
            {
                dbContext.Remove(p);
                dbContext.SaveChanges();
            }
            else
            {
                throw new Exception("Member does not already exists!");
            }
        }

        public List<Product> Search(string str)
        {
            double num1 = 0;
            double num2 = 0;

            try
            {
                if (str.Contains("-"))
                {
                    string[] period = str.Split('-');
                    if (period.Length == 2)
                    {
                        string[] part1 = System.Text.RegularExpressions.Regex.Split(period[0], @"\s*(?<=\d)\s*(?=\D)");
                        string[] part2 = System.Text.RegularExpressions.Regex.Split(period[1], @"\s*(?<=\d)\s*(?=\D)");

                        try
                        {
                            num1 = double.Parse(part1[0]);
                            num2 = double.Parse(part2[0]);
                        }
                        catch (FormatException)
                        {
                            throw new Exception("Invalid format!");
                        }

                        if (part1.Length == 1 && part2.Length == 2)
                        {
                            switch (part2[1].ToLower())
                            {
                                case "k":
                                case "thousand":
                                    if (num1 < num2) num1 *= 1000;
                                    num2 *= 1000;
                                    break;
                                case "mil":
                                case "milion":
                                    if (num1 < num2) num1 *= 1000000;
                                    num2 *= 1000000;
                                    break;
                            }
                        }
                        else if (part1.Length == 2 && part2.Length == 2)
                        {
                            switch (part1[1].ToLower())
                            {
                                case "k":
                                case "thousand":
                                    num1 *= 1000;
                                    break;
                                case "mil":
                                case "milion":
                                    num1 *= 1000000;
                                    break;
                            }
                            switch (part2[1].ToLower())
                            {
                                case "k":
                                case "thousand":
                                    num2 *= 1000;
                                    break;
                                case "mil":
                                case "milion":
                                    num2 *= 1000000;
                                    break;
                            }
                        }

                        if (num1 > num2 || num1 < 0 || num2 <= 0)
                        {
                            throw new Exception("Invalid range!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                num1 = 0;
                num2 = 0;
            }

            HashSet<Product> result = new HashSet<Product>();
            IQueryable<Product> query = ProductList.AsQueryable();

            if (num2 == 0)
            {
                if (int.TryParse(str, out int productId))
                {
                    result.UnionWith(query.Where(p => p.ProductId == productId));
                }
                else
                {
                    result.UnionWith(query.Where(p => p.ProductName.Contains(str, StringComparison.CurrentCultureIgnoreCase)));
                }
            }
            else
            {
                result.UnionWith(query.Where(p => p.UnitPrice >= num1 && p.UnitPrice <= num2));
                result.UnionWith(query.Where(p => p.UnitsInStock >= num1 && p.UnitsInStock <= num2));
            }

            return result.ToList();
        }

    }
}
