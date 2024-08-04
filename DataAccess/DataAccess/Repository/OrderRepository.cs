namespace SalesWPFApp
{
    public class OrderRepository : IOrderRepository
    {
        public List<OrderDetail> GetOrder() => OrderDAO.Instance.ListOrderDetails;

        public Order GetOrderByID(int id) => OrderDAO.Instance.GetOrderById(id);

        public IEnumerable<Order> GetOrders() => OrderDAO.Instance.GetOrders();

        public void SetOrder(List<OrderDetail> o) => OrderDAO.Instance.ListOrderDetails = o;

        public void DeleteItem(int id) => OrderDAO.Instance.DeleteItem(id);

        public List<OrderDetail> ViewOrderDetail(int id) => OrderDAO.Instance.ViewOrderDetail(id);

        public void DeleteOrder(int id) => OrderDAO.Instance.DeleteOrder(id);

        public Order GetOrderbyId(int orderId) => OrderDAO.Instance.GetOrderbyId(orderId);
        public void InsertOrder(Order o) => OrderDAO.Instance.AddNew(o);
        public void UpdateOrder(Order o) => OrderDAO.Instance.UpdateOrder(o);
        public IEnumerable<Order> getOrderListStatistic(DateTime startDate, DateTime endDate) => OrderDAO.Instance.GetOrderListStatistic(startDate, endDate);
        public IEnumerable<Order> getOrderListStatistic() => OrderDAO.Instance.GetOrderListStatistic();
        public IEnumerable<OrderDetail> SearchOrderDetail(string str, int id) => OrderDAO.Instance.SearchOrderDetail(str, id);
    }
}
