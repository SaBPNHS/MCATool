using System.Collections.Generic;

namespace Sfw.Sabp.Mca.Web.ViewModels
{
    public class QuestionAnswerViewModel
    {
        public string StageDescription { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string FurtherInformation { get; set; }
    }
    public class QuestionAnswerListViewModel
    {
        public IEnumerable<QuestionAnswerViewModel> Items { get; set; }
    }
}