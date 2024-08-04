using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Dynamic;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SalesWPFApp
{
    public class MemberDAO
    {

        private readonly IConfiguration _configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

        public dynamic GetAccountDefault()
        {
            dynamic account = new ExpandoObject();
            account.loginId = _configuration["login:id"];
            account.loginPassword = _configuration["login:password"];
            return account;
        }
        private readonly AppDbContext dbContext = new AppDbContext();
        public List<Member> GetMembers()
        {
            return dbContext.Members.ToList();
        }

        public dynamic getUser() => dbContext.account;

        public void setUser(dynamic user) => dbContext.account = user;

        public Member getMember() => dbContext.member;

        public void setMember(Member mem) => dbContext.member = mem;

        private static MemberDAO instance = null;
        public static readonly object instanceLook = new object();
        public static MemberDAO Instance
        {
            get
            {
                lock (instanceLook)
                {
                    if (instance == null)
                    {
                        instance = new MemberDAO();
                    }
                    return instance;
                }
            }
        }

        public List<string> getListCity()
        {
            List<string> list = new List<string>();
            list.Add("");
            foreach (var mem in MemberList)
            {
                if (!list.Contains(mem.City)) list.Add(mem.City);
            }
            return list;
        }

        public List<string> getListCountry()
        {
            List<string> list = new List<string>();
            list.Add("");
            foreach (var mem in MemberList)
            {
                if (!list.Contains(mem.Country)) list.Add(mem.Country);
            }
            return list;
        }

        public List<int> getListUserId()
        {
            List<int> list = new List<int>();
            foreach (var mem in MemberList)
            {
                list.Add(mem.Id);
            }
            return list;
        }

        public List<Member> MemberList => GetMembers();
        public Member GetMemberById(int MemberID)
        {
            Member mem = MemberList.FirstOrDefault(pro => pro.Id == MemberID);
            return mem;
        }

        public List<Member> SearchMember(string str)
        {
            HashSet<Member> result = new HashSet<Member>();
            IQueryable<Member> query = MemberList.AsQueryable();
            try
            {
                if (int.TryParse(str, out int id)) { 
                    result.UnionWith(query.Where(x => x.Id == id));
                    result.UnionWith(query.Where(x => x.Phone.Equals(str)));
                }
                else
                {
                    result.UnionWith(query.Where(x => x.Name.Contains(str, StringComparison.CurrentCultureIgnoreCase)));
                    result.UnionWith(query.Where(x => x.Email.Contains(str, StringComparison.CurrentCultureIgnoreCase)));
                    result.UnionWith(query.Where(x => x.City.Contains(str, StringComparison.CurrentCultureIgnoreCase)));
                    result.UnionWith(query.Where(x => x.Country.Contains(str, StringComparison.CurrentCultureIgnoreCase)));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result.ToList<Member>();
        }

        public List<Member> FilterMember(DateTime birthdayFrom, DateTime birthdayTo, string city, string country, string hobby)
        {
            HashSet<Member> result = new HashSet<Member>();
            IQueryable<Member> query = MemberList.AsQueryable();
            try
            {

                if (!string.IsNullOrEmpty(city))
                {
                    result.UnionWith(query.Where(x => x.City.Contains(city, StringComparison.CurrentCultureIgnoreCase)));
                }
                if (!string.IsNullOrEmpty(country))
                {
                    result.UnionWith(query.Where(x => x.Country.Contains(country, StringComparison.CurrentCultureIgnoreCase)));
                }
                if ((DateTime.Compare(birthdayFrom, DateTime.MinValue) > 0) && (DateTime.Compare(birthdayTo, DateTime.MinValue) > 0) && (DateTime.Compare(birthdayFrom, birthdayTo) < 0))
                {
                    result.UnionWith(query.Where(x => (DateTime.Compare(birthdayFrom, x.Birthday) >= 0 && DateTime.Compare(birthdayTo, x.Birthday) <= 0)));
                }
                if (!string.IsNullOrEmpty(hobby))
                {
                    result.UnionWith(query.Where(x => x.Hobby.Contains(hobby, StringComparison.CurrentCultureIgnoreCase)));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result.ToList();
        }

        public void AddNew(Member mem)
        {
            checkValidAccount(mem);
            Member pro = GetMemberById(mem.Id);
            if (pro == null)
            {
                dbContext.Add(mem);
                dbContext.SaveChanges();
            }
            else
            {
                throw new Exception("Member is already exists!");
            }
        }

        public bool IsValidPassword(string password)
        {
            return (password.Length > 6 && Regex.IsMatch(password, @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d\W]+$"));
        }
        public bool IsValidEmail(string email)
        {
            string emailPattern = @"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$";
            return Regex.IsMatch(email, emailPattern);
        }

        public bool IsValidPhone(string phone)
        {
            string phonePattern = @"^\d{10}$";
            return Regex.IsMatch(phone, phonePattern);
        }

        private Boolean checkValidAccount(Member mem)
        {
            if (mem.Name == null) throw new Exception("Input invalid");
            if (!IsValidEmail(mem.Email)) throw new Exception("Email invalid!!!");
            if (!IsValidPassword(mem.Password)) throw new Exception("Password invalid!!!");
            if (!IsValidPhone(mem.Phone)) throw new Exception("Phone invalid!!!");
            return true;
        }

        public async Task Update(Member mem)
        {
            checkValidAccount(mem);
            Member c = GetMemberById(mem.Id);
            if (c != null)
            {
                dbContext.ChangeTracker.Clear();
                dbContext.Members.Update(mem); 
                await dbContext.SaveChangesAsync();
                if (getMember() != null && getMember().Orders != null) mem.Orders = getMember().Orders;
                setMember(mem);
            }
            else
            {
                throw new Exception("Member does not already exists!");
            }
        }

        public void Remove(int id)
        {
            Member p = GetMemberById(id);
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

        public Member loginMember(string email, string pw)
        {
            foreach (var mem in MemberList)
            {
                if (mem.Email.Equals(email) && mem.Password.Equals(pw))
                {
                    dbContext.Members.Include(r => r.Orders).First().Id = mem.Id;
                    setMember(dbContext.Members.Include(r => r.Orders).FirstOrDefault(pro => pro.Id == mem.Id));
                    return mem;
                }
            }
            return null;
        }

        public List<Order> GetOrderHistory()
        {
            return dbContext.Orders.Include(o => o.Member).AsNoTracking().ToList();
        }

        public List<Order> SearchOrder(string str, Boolean isAdmin)
        {

            HashSet<Order> result = new HashSet<Order>();
            if (int.TryParse(str, out int id))
                {
                    if (isAdmin)
                    {

                        IQueryable<Order> query = dbContext.Orders.AsQueryable();
                        result.UnionWith(query.Where(o => o.OrderId == id));
                        result.UnionWith(query.Where(o => o.MemberId == id));
                    }
                    else
                    {
                        IQueryable<Order> query = dbContext.member.Orders.AsQueryable();
                        result.UnionWith(query.Where(o => o.OrderId == id));
                    }
                }
            return result.ToList();
        }


        internal List<Order> GetMemberOrderHistory()
        {
            return dbContext.member.Orders.ToList();
        }

        internal Member GetMemberByEmail(string? v)
        {
            return dbContext.Members.FirstOrDefault(pro => pro.Email == v);
        }
    }
}