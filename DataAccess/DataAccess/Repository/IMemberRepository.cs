using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace SalesWPFApp
{
    public interface IMemberRepository
    {
        IEnumerable<Member> GetMembers();
        Member GetMemberByID(int id);
        void InsertMember(Member mem);
        void DeleteMember(int id);
        void UpdateMember(Member mem);
        IEnumerable<Member> SearchMember(string str);
        IEnumerable<Member> FilterMember(DateTime birthdayFrom, DateTime birthdayTo, string city, string country, string hobby);
        List<string> getListCity();
        List<string> getListCountry();
        dynamic GetAccountDefault();
        Member loginMember(string email, string pw);
        public IEnumerable<Order> GetOrderHistory();
        public IEnumerable<Order> GetMemberOrderHistory();
        public IEnumerable<Order> SearchOrder(string str, Boolean isAdmin);
    }
}
