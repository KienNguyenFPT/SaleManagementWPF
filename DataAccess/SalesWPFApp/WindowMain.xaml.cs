using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SalesWPFApp
{
    /// <summary>
    /// Interaction logic for WindowMain.xaml
    /// </summary>
    public partial class WindowMain : Window
    {
        WindowLogin loginView;
        ProductRepository productRespository = new ProductRepository();
        MemberRepository memberRespository = new MemberRepository();
        OrderRepository orderRespository = new OrderRepository();
        OrderDetailRepository orderDetailRespository = new OrderDetailRepository();
        private bool isInitializing;

        public WindowMain()
        {
            InitializeComponent();
        }

        public WindowMain(WindowLogin loginView)
        {
            isInitializing = true;
            InitializeComponent();
            isInitializing = false;
            txtNameUser.Content = memberRespository.GetUser().Name;
            authorize();
            this.loginView = loginView;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                TabItem selectedTab = ((sender as TabControl).SelectedItem as TabItem);

                if (selectedTab != null)
                {
                    showList(selectedTab.Name);
                }
            }
        }
        public void showList(string tab)
        {
            switch (tab)
            {
                case "product":
                    refreshListProduct(); break;
                case "order":
                    refreshListOrder(); break;
                case "member":
                    refreshListMember(); break;
                case "statistic":
                    refreshListOrderStatistic(); break;
            }
        }



        private void DecreaseButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            Product product = button.DataContext as Product;
            if (product != null && product.Quantity > 0)
            {
                product.Quantity--;
            }
        }

        private void IncreaseButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            Product product = button.DataContext as Product;
            if (product != null && product.Quantity < product.UnitsInStock)
            {
                product.Quantity++;
            }
        }

        private void txtSearchOrderDetail_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isInitializing || string.IsNullOrWhiteSpace(txtOrderId.Text))
                return;
            string str = txtSearchOrderDetail.Text;
            if (!string.IsNullOrWhiteSpace(str) && !str.Equals("Search...")) orderDetailList.ItemsSource = orderRespository.SearchOrderDetail(str, int.Parse(txtOrderId.Text));
            else
            {
                orderDetailList.ItemsSource = orderRespository.ViewOrderDetail(int.Parse(txtOrderId.Text));
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                Order o = new Order
                {
                    MemberId = isAdmin() ? (int)txtMemberOrderId.SelectedValue : memberRespository.GetUser().Id,
                    Freight = 10,
                    OrderDate = DateTime.Now,
                    RequiredDate = DateTime.Now.AddDays(5),
                    ShippedDate = DateTime.Now.AddDays(1),
                    orderDetails = new List<OrderDetail>()
                };
                foreach (Product product in productList.Items)
                {
                    if (product.Quantity > 0)
                    {
                        OrderDetail oD = new OrderDetail
                        {
                            ProductId = product.ProductId,
                            Discount = 0,
                            Quantity = product.Quantity,
                            UnitPrice = product.UnitPrice
                        };
                        o.orderDetails.Add(oD);
                    }
                }
                orderRespository.InsertOrder(o);
                refreshListProduct();
                MessageBox.Show("Order Success!!!", "Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Order failed: {ex.Message}", "Error");
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            refreshProductList();
        }

        public void refreshProductList()
        {
            foreach (Product p in productList.Items.OfType<Product>())
            {
                p.Quantity = 0;
            }
            productList.Items.Refresh();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            loginView.Show();
        }

        private void TxtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            switch (textBox.Name)
            {
                case "txtSearchProduct":
                    focus(txtSearchProduct, true); break;
                case "txtSearchOrder":
                    focus(txtSearchOrder, true); break;
                case "txtSearchOrderDetail":
                    focus(txtSearchOrderDetail, true); break;
                case "txtSearchMember":
                    focus(txtSearchMember, true); break;
            }
        }

        private void TxtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            switch (textBox.Name)
            {
                case "txtSearchProduct":
                    focus(txtSearchProduct, false); break;
                case "txtSearchOrder":
                    focus(txtSearchOrder, false); break;
                case "txtSearchOrderDetail":
                    focus(txtSearchOrderDetail, false); break;
                case "txtSearchMember":
                    focus(txtSearchMember, false); break;
            }
        }
        public void focus(TextBox txtSearch, Boolean isFocus)
        {
            if (isFocus)
            {
                if (txtSearch.Text == "Search...")
                {
                    txtSearch.Text = "";
                    txtSearch.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    txtSearch.Text = "Search...";
                    txtSearch.Foreground = new SolidColorBrush(Colors.Gray);
                }
            }
        }

        private void orderList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid listData = sender as DataGrid;
            if (listData.SelectedItem != null)
            {
                bool isOrderList = listData.Name.Equals("orderList");
                Order selectedOrder = isOrderList ? orderList.SelectedItem as Order : orderListStatistic.SelectedItem as Order;
                List<OrderDetail> orderDetails = orderRespository.ViewOrderDetail(selectedOrder.OrderId);
                double price = 0;
                foreach (OrderDetail o in orderDetails)
                {
                    price += o.Quantity * o.UnitPrice;
                }
                if (isOrderList)
                {
                    showOrderSelected(selectedOrder);
                    orderDetailList.ItemsSource = orderDetails;
                    priceTxt.Content = price.ToString("N2");
                    freightTxt.Content = selectedOrder.Freight;
                    discountTxt.Content = "0";
                    vatTxt.Content = (price / 10).ToString("N2");
                    totalTxt.Content = (price * 1.1 + selectedOrder.Freight ?? 0).ToString("N2");
                }
                else
                {
                    orderDetailStatisticList.ItemsSource = orderDetails;
                    priceStatisticTxt.Content = price.ToString("N2");
                    freightStatisticTxt.Content = selectedOrder.Freight;
                    discountStatisticTxt.Content = "0";
                    vatStatisticTxt.Content = (price / 10).ToString("N2");
                    totalStatisticTxt.Content = (price * 1.1 + selectedOrder.Freight ?? 0).ToString("N2");
                }
            }
        }

        private void btnDeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                productRespository.DeleteProduct(int.Parse(txtProductId.Text));
                refreshListProduct();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnUpdateProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                productRespository.UpdateProduct(getProduct());
                MessageBox.Show("Update Product successful!");
                clearProductForm();
                refreshListProduct();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnInsertProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                productRespository.InsertProduct(getProduct());
                MessageBox.Show("Create new Product successful!");
                clearProductForm();
                refreshListProduct();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnInsertMember_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                memberRespository.InsertMember(getMember());
                MessageBox.Show("Create new Member successful!");
                clearMemberForm();
                refreshListMember();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnUpdateMember_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                memberRespository.UpdateMember(getMember());
                MessageBox.Show("Update member successful!");
                clearMemberForm();
                refreshListMember();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnDeleteMember_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                memberRespository.DeleteMember(int.Parse(txtMemberId.Text));
                refreshListMember();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void productList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (productList.SelectedItem != null)
            {
                Product selectedProduct = productList.SelectedItem as Product;
                txtProductId.Text = selectedProduct.ProductId.ToString();
                txtProductName.Text = selectedProduct.ProductName;
                txtProductCategoryId.Text = selectedProduct.CategoryId.ToString();
                txtProductWeight.Text = selectedProduct.Weight.ToString();
                txtProductUnitPrice.Text = selectedProduct.UnitPrice.ToString();
                txtProductUnitInStock.Text = selectedProduct.UnitsInStock.ToString();
            }
        }

        private void memberList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (memberList.SelectedItem != null)
            {
                Member selectedMember = memberList.SelectedItem as Member;
                showMemberSelected(selectedMember);
            }
        }

        private void showMemberSelected(Member selectedMember)
        {
            txtMemberId.Text = selectedMember.Id.ToString();
            txtMemberName.Text = selectedMember.Name;
            txtMemberEmail.Text = selectedMember.Email;
            txtMemberCity.Text = selectedMember.City;
            txtMemberCountry.Text = selectedMember.Country;
            txtMemberPhone.Text = selectedMember.Phone;
            txtMemberBirthday.Text = selectedMember.Birthday.ToString();
            txtMemberHobby.Text = selectedMember.Hobby;
            txtMemberPw.Password = selectedMember.Password;
            txtMemberPwConfirm.Password = selectedMember.Password;
        }

        private void showOrderSelected(Order selectedOrder)
        {
            txtOrderId.Text = selectedOrder.Member.Name.ToString();
            txtOrderMemberId.Text = selectedOrder.Member.Email.ToString();
            txtOrderDate.Text = selectedOrder.OrderDate.ToString();
            txtOrderRequired.Text = selectedOrder.RequiredDate.ToString();
            txtOrderShipped.Text = selectedOrder.ShippedDate.ToString();
            txtOrderFreight.Text = selectedOrder.Freight.ToString();
        }

        private void btnClearMember_Click(object sender, RoutedEventArgs e)
        {
            clearMemberForm();
        }

        private void clearMemberForm()
        {
            txtMemberId.Text = "";
            txtMemberName.Text = "";
            txtMemberEmail.Text = "";
            txtMemberCity.Text = "";
            txtMemberCountry.Text = "";
            txtMemberPhone.Text = "";
            txtMemberBirthday.Text = "";
            txtMemberHobby.Text = "";
            txtMemberPw.Password = "";
            txtMemberPwConfirm.Password = "";
        }

        private void btnClearProduct_Click(object sender, RoutedEventArgs e)
        {
            clearProductForm();
        }

        private void clearOrderForm()
        {
            txtOrderId.Text = "";
            txtOrderMemberId.Text = "";
            txtOrderDate.Text = "";
            txtOrderRequired.Text = "";
            txtOrderShipped.Text = "";
            txtOrderFreight.Text = "";
            orderDetailList.ItemsSource = null;
            priceTxt.Content = "";
            freightTxt.Content = "";
            vatTxt.Content = "";
            discountTxt.Content = "";
            totalTxt.Content = "";
        }

        private void clearProductForm()
        {
            txtProductId.Text = "";
            txtProductName.Text = "";
            txtProductCategoryId.Text = "";
            txtProductWeight.Text = "";
            txtProductUnitPrice.Text = "";
            txtProductUnitInStock.Text = "";
        }

        public Product getProduct()
        {
            Product p = new Product();
            if (!string.IsNullOrEmpty(txtProductId.Text)) p.ProductId = int.Parse(txtProductId.Text);
            p.ProductName = txtProductName.Text;
            p.CategoryId = int.Parse(txtProductCategoryId.Text);
            p.Weight = txtProductWeight.Text;
            p.UnitPrice = int.Parse(txtProductUnitPrice.Text);
            p.UnitsInStock = int.Parse(txtProductUnitInStock.Text);
            return p;
        }

        public Order getOrder()
        {
            Order o = orderList.SelectedItem as Order;
            o.OrderDate = DateTime.Parse(txtOrderDate.Text);
            o.RequiredDate = DateTime.Parse(txtOrderRequired.Text);
            o.ShippedDate = DateTime.Parse(txtOrderShipped.Text);
            o.Freight = int.Parse(txtOrderFreight.Text);
            return o;
        }

        public Member getMember()
        {
            Member m = new Member();
            if (!string.IsNullOrEmpty(txtMemberId.Text)) m.Id = int.Parse(txtMemberId.Text);
            m.Name = txtMemberName.Text;
            m.Email = txtMemberEmail.Text;
            m.City = txtMemberCity.Text;
            m.Country = txtMemberCountry.Text;
            m.Phone = txtMemberPhone.Text;
            m.Birthday = DateTime.Parse(txtMemberBirthday.Text);
            m.Avt = "";
            m.Hobby = txtMemberHobby.Text;
            if (!string.IsNullOrWhiteSpace(txtMemberPw.Password) && txtMemberPw.Password.Equals(txtMemberPwConfirm.Password))
                m.Password = txtMemberPw.Password;
            else throw new Exception("Password must similar with password confirm and not empty");
            return m;
        }

        public void authorize()
        {
            if (!isAdmin())
            {
                btnClear.Visibility = Visibility.Collapsed;
                btnDelete.Visibility = Visibility.Collapsed;
                btnInsert.Visibility = Visibility.Collapsed;
                btnUpdate.Visibility = Visibility.Collapsed;
                btnDeleteMember.Visibility = Visibility.Collapsed;
                btnMemberInsert.Visibility = Visibility.Collapsed;
                btnClearMember.Visibility = Visibility.Collapsed;
                memberList.Visibility = Visibility.Collapsed;
                txtSearchMember.Visibility = Visibility.Collapsed;
                statistic.Visibility = Visibility.Collapsed;
                btnUpdateOrder.Visibility = Visibility.Collapsed;
                btnDeleteOrder.Visibility = Visibility.Collapsed;
                member.Header = "Profile";
                txtMemberOrderId.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtMemberOrderId.ItemsSource = memberRespository.getListUserId();
            }
        }
        public void refreshListProduct()
        {
            productList.ItemsSource = productRespository.GetProducts();
        }

        public void refreshListOrder()
        {
            if (!isAdmin())
                orderList.ItemsSource = memberRespository.GetMemberOrderHistory();
            else orderList.ItemsSource = memberRespository.GetOrderHistory();
        }

        public void refreshListOrderStatistic()
        {
            IEnumerable<Order> orders = orderRespository.getOrderListStatistic();
            orderListStatistic.ItemsSource = orders;
            StatisticOrders(orders);
        }

        public void refreshListMember()
        {
            if (!isAdmin())
            {
                showMemberSelected(memberRespository.GetMemberByID(memberRespository.GetUser().Id));
            }
            else memberList.ItemsSource = memberRespository.GetMembers();
        }

        public Boolean isAdmin()
        {
            return memberRespository.GetUser().Role.Equals("Admin");
        }

        private void btnUpdateOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                orderRespository.UpdateOrder(getOrder());
                MessageBox.Show("Update order successful!");
                clearOrderForm();
                refreshListOrder();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnDeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                orderRespository.DeleteOrder(int.Parse(txtOrderId.Text));
                refreshListOrder();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void txtSearchProduct_TextChanged(object sender, TextChangedEventArgs e)
        {
            string str = txtSearchProduct.Text;
            if (!string.IsNullOrWhiteSpace(str) && !str.Equals("Search...")) productList.ItemsSource = productRespository.SearchProduct(str);
            else refreshListProduct();
        }

        private void txtSearchOrder_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isInitializing)
                return;
            string str = txtSearchOrder.Text;
            if (!string.IsNullOrWhiteSpace(str) && !str.Equals("Search...")) orderList.ItemsSource = memberRespository.SearchOrder(str, isAdmin());
            else
            {
                refreshListOrder();
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            refreshListOrderStatistic();
            txtStartDateStatistic.Text = "";
            txtEndDateStatistic.Text = "";
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {

            if (txtStartDateStatistic.SelectedDate.HasValue && txtEndDateStatistic.SelectedDate.HasValue)
            {
                DateTime startDate = txtStartDateStatistic.SelectedDate.Value;
                DateTime endDate = txtEndDateStatistic.SelectedDate.Value;
                IEnumerable<Order> orders = orderRespository.getOrderListStatistic(startDate, endDate);
                orderListStatistic.ItemsSource = orders;
                StatisticOrders(orders);
            }
            else
            {
                MessageBox.Show("Please select valid start and end dates.", "Invalid Dates", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void StatisticOrders(IEnumerable<Order> orders)
        {
            double totalRevenue = 0;
            int orderCount = 0;
            Dictionary<int, double> products = new Dictionary<int, double>();
            Dictionary<int, double> buyers = new Dictionary<int, double>();
            int topBuyer = 0;
            int topProduct = 0;

            foreach (Order order in orders)
            {
                totalRevenue += order.Total;
                orderCount++;
                if (buyers.ContainsKey(order.MemberId))
                {
                    buyers[order.MemberId] += order.Total;
                }
                else
                {
                    buyers.Add(order.MemberId, order.Total);
                }

                if (buyers[order.MemberId] > buyers.GetValueOrDefault(topBuyer, 0))
                {
                    topBuyer = order.MemberId;
                }
                foreach (OrderDetail orderDetail in order.orderDetails)
                {
                    if (products.ContainsKey(orderDetail.ProductId))
                    {
                        products[orderDetail.ProductId] += orderDetail.Quantity;
                    }
                    else
                    {
                        products.Add(orderDetail.ProductId, orderDetail.Quantity);
                    }
                    if (products[orderDetail.ProductId] > products.GetValueOrDefault(topProduct, 0))
                    {
                        topProduct = orderDetail.ProductId;
                    }
                }
            }

            txtTotalOrders.Content = orderCount;
            txtTopBuyer.Content = "ID: " + topBuyer;
            ToolTip hoverToolTip = new ToolTip();
            if (buyers.Count() > 0) hoverToolTip.Content = "Total spending: " + buyers[topBuyer];
            txtTopBuyer.ToolTip = hoverToolTip;
            txtTopProduct.Content = "ID: " + topProduct;
            ToolTip hoverToolTip2 = new ToolTip();
            if (products.Count() > 0) hoverToolTip2.Content = "Total quantity buyed: " + products[topProduct];
            txtTopProduct.ToolTip = hoverToolTip2;
            txtTotalRevenue.Content = totalRevenue.ToString("C");
        }

        private void txtSearchMember_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isInitializing)
                return;
            string str = txtSearchMember.Text;
            if (!string.IsNullOrWhiteSpace(str) && !str.Equals("Search...")) memberList.ItemsSource = memberRespository.SearchMember(str);
            else
            {
                refreshListMember();
            }
        }
    }

}
