using System.Linq;
using RazorEngine;
using Stackoverflow.SqlContext.Concrete.TemplateMessage;
using Itransition.SendingEmails.Models;

namespace Itransition.SendingEmails.Templates
{
    public class TemplateEmail
    {
        private const string AnswerToTheQuestion = "AnswerToTheQuestion";

        private readonly TemplateMessageRepository _templateMessage = new TemplateMessageRepository();

        public string SubjectAnswerToTheQuestion()
        {
            return _templateMessage.Entity.Where(item => item.Name == AnswerToTheQuestion).Select(item => item.Subject).FirstOrDefault();
        }

        public string BodyAnswerToTheQuestion(QuestionSendModel model)
        {
            var templateBody = _templateMessage.Entity.Where(item => item.Name == AnswerToTheQuestion).Select(item => item.Body).FirstOrDefault();
            var result = Razor.Parse(templateBody, model);
            return result;
        }

    }
}
