using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using Stackoverflow.SqlContext.Concrete.Answers;
using Stackoverflow.SqlContext.Entities;

namespace Stackoverflow.SqlContext.Concrete.Questions
{
    public class QuestionRepository
    {
        private readonly StackoverflowEntities _dataContext = new StackoverflowEntities();
        private readonly AnswerRepository _answerRepository = new AnswerRepository();

        public DbSet<Question> Entity
        {
            get { return _dataContext.Question; }
        }

        private Question Find(long id)
        {
            return Entity.Find(id);
        }

        public void Add(long userId, string title, string body, int usefulness, DateTime publicationDate)
        {
            var item = new Question();
            item.UserId = userId;
            item.Title = title;
            item.Body = body;
            item.Usefulness = usefulness;
            item.PublicationDate = publicationDate;

            Entity.Add(item);
            _dataContext.SaveChanges();
        }

        public void Edit(long id, string title, string body)
        {
            var item = Find(id);

            item.Title = title;
            item.Body = body;

            _dataContext.Entry(item).State = EntityState.Modified;
            _dataContext.SaveChanges();
        }

        public void Delete(long id)
        {
            var answersId = _dataContext.Answer
                .Where(answer => answer.QuestionId == id)
                .Select(answer => answer.Id)
                .ToList();

            foreach (var answer in answersId)
            {
                _answerRepository.Delete(answer);
            }

            var comments = _dataContext.CommentaryQuestion.Where(comment => comment.QuestionId == id).ToList();
            foreach (var comment in comments)
            {
                _dataContext.CommentaryQuestion.Remove(comment);
            }

            var item = Find(id);
            Entity.Remove(item);
            _dataContext.SaveChanges();
        }

        public void UsefulnessUp(long id)
        {
            var item = Find(id);
            item.Usefulness++;
            _dataContext.SaveChanges();
        }

        public void UsefulnessDown(long id)
        {
            var item = Find(id);
            item.Usefulness--;
            _dataContext.SaveChanges();
        }
    
    }
}
