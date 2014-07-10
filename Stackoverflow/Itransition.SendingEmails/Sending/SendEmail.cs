using System;
using System.Linq;
using System.Net.Mail;
using Itransition.SendingEmails.Models;
using Itransition.SendingEmails.Templates;
using Stackoverflow.SqlContext.Concrete.Questions;

namespace Itransition.SendingEmails.Sending
{
    public class SendEmail
    {
        private const string SmtpClient = "localhost";
        private const string EmailFrom = "support@stackoverflow.com";

        private readonly TemplateEmail _templete = new TemplateEmail();
        private readonly QuestionRepository _questionRepository = new QuestionRepository();

        public void SendEmailAnswerToTheQuestion(long questionId)
        {
            var model = _questionRepository.Entity
                                    .Where(item => item.Id == questionId)
                                    .Select(item => new QuestionSendModel
                                    {
                                        Nickname = item.User.Nickname,
                                        Title = item.Title,
                                        UserEmail = item.User.Email
                                    })
                                    .FirstOrDefault();

            if (model == null)
                throw new Exception("No data question in the database");

            var mail = new MailMessage();

            mail.To.Add(model.UserEmail);
            mail.From = new MailAddress(EmailFrom);

            mail.Subject = _templete.SubjectAnswerToTheQuestion();
            mail.Body = _templete.BodyAnswerToTheQuestion(model);

            //send the message
            var smtp = new SmtpClient(SmtpClient) { UseDefaultCredentials = true };
            smtp.Send(mail);
        }
    }
}
