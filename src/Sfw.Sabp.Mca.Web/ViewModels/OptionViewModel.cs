using System;

namespace Sfw.Sabp.Mca.Web.ViewModels
{
    public class OptionViewModel
    {
        public Guid OptionId { get; set; }

        public string Description { get; set; }

        public bool Selected { get; set; }

        public Guid QuestionOptionId { get; set; }

        public string FurtherQuestion { get; set; }

        public string FurtherQuestionAnswer { get; set; }

        public bool HasFurterQuestion
        {
            get { return !string.IsNullOrWhiteSpace(FurtherQuestion); }
        }
    }
}