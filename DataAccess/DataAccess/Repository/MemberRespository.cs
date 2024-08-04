using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SalesWPFApp
{
    public class MemberRepository : IMemberRepository
    {

        public void DeleteMember(int id) => MemberDAO.Instance.Remove(id);

        public Member  GetMemberByID(int id) => MemberDAO.Instance.GetMemberById(id);

        public IEnumerable<Member> GetMembers() => MemberDAO.Instance.MemberList;
            
        public void InsertMember(Member mem) => MemberDAO.Instance.AddNew(mem);

        public void UpdateMember(Member mem) => MemberDAO.Instance.Update(mem);

        public IEnumerable<Member> SearchMember(string str) => MemberDAO.Instance.SearchMember(str);
        public IEnumerable<Member> FilterMember(DateTime birthdayFrom, DateTime birthdayTo, string city, string country, string hobby) => MemberDAO.Instance.FilterMember(birthdayFrom, birthdayTo, city, country, hobby);
        public dynamic GetAccountDefault() => MemberDAO.Instance.GetAccountDefault();

        public Member loginMember(string email, string pw) => MemberDAO.Instance.loginMember(email, pw);
        public dynamic GetUser() => MemberDAO.Instance.getUser();
        public void setUser(dynamic user) => MemberDAO.Instance.setUser(user);
        public Member GetMember() => MemberDAO.Instance.getMember();

        public List<string> getListCity() => MemberDAO.Instance.getListCity();

        public List<string> getListCountry() => MemberDAO.Instance.getListCountry();
        public List<int> getListUserId() => MemberDAO.Instance.getListUserId();

        public IEnumerable<Order> GetOrderHistory() => MemberDAO.Instance.GetOrderHistory();
        public IEnumerable<Order> GetMemberOrderHistory() => MemberDAO.Instance.GetMemberOrderHistory();

        public Member GetMemberByEmail(string? v) => MemberDAO.Instance.GetMemberByEmail(v);

        public IEnumerable<Order> SearchOrder(string str, Boolean isAdmin) => MemberDAO.Instance.SearchOrder(str, isAdmin);
    }
}
