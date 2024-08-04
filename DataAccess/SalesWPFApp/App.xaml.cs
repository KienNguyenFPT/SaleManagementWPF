using System.Configuration;
using System.Data;
using System.Windows;

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace SalesWPFApp
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();

            var loginWindow = ServiceProvider.GetRequiredService<WindowLogin>();
            loginWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<IMemberRepository, MemberRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddSingleton<OrderDAO>();
            services.AddSingleton<OrderDetailDAO>();
            services.AddSingleton<ProductDAO>();
            services.AddSingleton<MemberDAO>();
            services.AddSingleton<AppDbContext>();
            services.AddTransient<WindowMain>();
            services.AddTransient<WindowLogin>();
        }
    }
}

