using System.Data;
using System.Data.Entity;
using Stackoverflow.SqlContext.Entities;

namespace Stackoverflow.SqlContext.Concrete.TemplateMessage
{
    public class TemplateMessageRepository
    {
        private readonly StackoverflowEntities _dataContext = new StackoverflowEntities();

        public DbSet<TemplateMessageEmail> Entity
        {
            get { return _dataContext.TemplateMessageEmail; }
        } 

        private TemplateMessageEmail Find(long id)
        {
            return Entity.Find(id);
        }

        public void Add(string nameTemplate, string subject, string body)
        {
            var item = new TemplateMessageEmail();
            item.Name = nameTemplate;
            item.Subject = subject;
            item.Body = body;

            _dataContext.TemplateMessageEmail.Add(item);
            _dataContext.SaveChanges();
        }

        public void Edit(long id, string nameTemplate, string subject, string body)
        {
            var item = Find(id);
            item.Name = nameTemplate;
            item.Subject = subject;
            item.Body = body;

            _dataContext.Entry(item).State = EntityState.Modified;
            _dataContext.SaveChanges();
        }

        public void Delete(long id)
        {
            var item = Find(id);
            Entity.Remove(item);
            _dataContext.SaveChanges();
        }
    
    }
}
