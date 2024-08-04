using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SalesWPFApp
{
    public class OrderDAO
    {
        private readonly AppDbContext dbContext = new AppDbContext();

        public List<Order> orders => GetOrders();


        public List<OrderDetail> ListOrderDetails { get; set; }

        public List<Order> GetOrders()
        {
            return dbContext.Orders.ToList();
        }

        public Order GetOrderById(int id)
        {
            Order order = orders.SingleOrDefault(o => o.OrderId == id);
            return order;
        }

        private static OrderDAO instance = null;
        public static readonly object instanceLook = new object();
        public static OrderDAO Instance
        {
            get
            {
                lock (instanceLook)
                {
                    if (instance == null)
                    {
                        instance = new OrderDAO();
                    }
                    return instance;
                }
            }
        }

        internal void DeleteItem(int id)
        {
            ListOrderDetails.Remove(ListOrderDetails.Where(x => x.ProductId == id).FirstOrDefault());
        }

        internal List<OrderDetail> ViewOrderDetail(int id)
        {
            return dbContext.Orders.Where(x => x.OrderId == id).Include(r => r.orderDetails).ThenInclude(p => p.Product).FirstOrDefault().orderDetails.ToList();
        }

        public void DeleteOrder(int id)
        {
            Order p = dbContext.Orders.Find(id);
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

        internal Order GetOrderbyId(int id)
        {
            return dbContext.Orders.FirstOrDefault(pro => pro.OrderId == id);
        }


        public void AddNew(Order order)
        {
            try
            {
                foreach(OrderDetail o in order.orderDetails)
                {
                    Product p = ProductDAO.Instance.GetProductById(o.ProductId);
                    p.UnitsInStock -= o.Quantity;
                    ProductDAO.Instance.Update(p);
                }
                dbContext.ChangeTracker.Clear();
                dbContext.Add(order);
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateOrder(Order order)
        {
            Order c = GetOrderbyId(order.OrderId);
            if (c != null)
            {
                dbContext.ChangeTracker.Clear();
                dbContext.Orders.Update(order);
                await dbContext.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Member does not already exists!");
            }
        }

        public List<Order> GetOrderListStatistic(DateTime startDate, DateTime endDate)
        {
            List<Order> orders = dbContext.Orders
                                          .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                                          .Include(o => o.orderDetails)
                                          .ToList();
            return desList(orders);
        }


        public List<Order> GetOrderListStatistic()
        {
            return desList(dbContext.Orders.Include(o => o.orderDetails).Include(m => m.Member).ToList());
        }

        public List<Order> desList(List<Order> orders)
        {
            foreach (Order order in orders)
            {
                order.Total = order.orderDetails.Sum(oD => oD.UnitPrice * oD.Quantity);
            }

            orders = orders.OrderByDescending(o => o.Total).ToList();

            return orders;
        }


        public List<OrderDetail> SearchOrderDetail(string str, int id)
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

            HashSet<OrderDetail> result = new HashSet<OrderDetail>();
            IQueryable<OrderDetail> query = dbContext.OrderDetails.AsQueryable();

            if (num2 == 0)
            {
                if (int.TryParse(str, out int pId))
                {
                    result.UnionWith(query.Where(oD => oD.OrderId == id && oD.ProductId == id));
                }
            }
            else
            {
                result.UnionWith(query.Where(oD => oD.OrderId == id && oD.UnitPrice >= num1 && oD.UnitPrice <= num2));
            }

            return result.ToList();
        }
    }
}
