using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesWPFApp
{
    public interface IOrderRepository
    {
        IEnumerable<Order> GetOrders();
        Order GetOrderByID(int id);

        List<OrderDetail> GetOrder();
        void SetOrder(List<OrderDetail> o);
        void InsertOrder(Order o);
        void UpdateOrder(Order o);
        IEnumerable<Order> getOrderListStatistic(DateTime startDate, DateTime endDate);
        IEnumerable<Order> getOrderListStatistic();
        public IEnumerable<OrderDetail> SearchOrderDetail(string str, int id);
    }
}
