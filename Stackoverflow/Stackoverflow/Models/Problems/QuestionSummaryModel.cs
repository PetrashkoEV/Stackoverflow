using System;
using Stackoverflow.Models.Questions;

namespace Stackoverflow.Models.Problems
{
    public class QuestionSummaryModel
    {
        public long Id { get; set; }
        public int Usefulness { get; set; } // like
        public int AmountAnswer { get; set; }
        public int BestAnswer { get; set; } 
        public string Title { get; set; }
        public string Body { get; set; }

        public UserModel User { get; set; }
        public DateTime PublicationDate { get; set; }

    }
}