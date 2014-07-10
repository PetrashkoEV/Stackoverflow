using System.Collections.Generic;

namespace Stackoverflow.Models.Problems
{
    public class ProblemsModel
    {
        public List<QuestionSummaryModel> Questions { get; set; }
        public int CountPages { get; set; }
        public int VisitedPage { get; set; }
    }
}