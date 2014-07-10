using System;
using System.Data;
using System.Data.Entity;
using Stackoverflow.SqlContext.Entities;

namespace Stackoverflow.SqlContext.Concrete.Users
{
    public class UserRepository
    {
        private readonly StackoverflowEntities _dataContext = new StackoverflowEntities();

        public DbSet<User> Entity
        {
            get { return _dataContext.User; }
        }

        private User Find(long id)
        {
            return _dataContext.User.Find(id);
        }

        public void Add(string nickname, string name, string surname, string sex, DateTime birthDate, string mobile, string email)
        {
            var item = new User();
            item.Nickname = nickname;
            item.Name = name;
            item.Surname = surname;
            item.Sex = sex;
            item.BirthDate = birthDate;
            item.Mobile = mobile;
            item.Email = email;

            Entity.Add(item);
            _dataContext.SaveChanges();
        }

        public void Edit(long id, string nickname, string name, string surname, string sex, DateTime birthDate, string mobile, string email)
        {
            var item = Find(id);
            item.Nickname = nickname;
            item.Name = name;
            item.Surname = surname;
            item.Sex = sex;
            item.BirthDate = birthDate;
            item.Mobile = mobile;
            item.Email = email;

            _dataContext.Entry(item).State = EntityState.Modified;
            _dataContext.SaveChanges();
        }

        public void Delete(long id)
        {
            var item = Find(id);
            _dataContext.User.Remove(item);
            _dataContext.SaveChanges();
        }

    }
}
