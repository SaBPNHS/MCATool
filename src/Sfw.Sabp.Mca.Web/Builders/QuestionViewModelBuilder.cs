using System;
using System.Collections.Generic;
using System.Linq;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Web.ViewModels;

namespace Sfw.Sabp.Mca.Web.Builders
{
    public class QuestionViewModelBuilder : IQuestionViewModelBuilder
    {
        public class NullWorkFlowQuestionException : Exception { }

        public QuestionViewModel BuildQuestionViewModel(Assessment assessment, QuestionAnswer questionAnswer)
        {
            if (assessment == null) throw new ArgumentNullException();
            if (assessment.WorkflowQuestion == null) throw new NullWorkFlowQuestionException();

            var optionsList =
                assessment.WorkflowQuestion.Question.QuestionOptions.OrderBy(x => x.Order).Select(
                    x =>
                        new OptionViewModel
                        {
                            Description = x.Option.Description,
                            OptionId = x.Option.OptionId,
                            QuestionOptionId = x.QuestionOptionId,
                            FurtherQuestion = x.FurtherQuestion != null ? x.FurtherQuestion.Description : string.Empty,
                            Selected = Selected(questionAnswer, x),
                            FurtherQuestionAnswer = Selected(questionAnswer, x) ? questionAnswer.FurtherInformation : string.Empty
                        }).ToList();

            var workflowStage = assessment.WorkflowQuestion.WorkflowStage;

            var buildQuestionViewModel = new QuestionViewModel
            {
                StageDescription = workflowStage.Description,
                StageDescriptionStyle = workflowStage.ShortDescription,
                Guidance = assessment.WorkflowQuestion.Question.Guidance,
                Question = assessment.WorkflowQuestion.Question.Description,
                Options = optionsList,
                AssessmentId = assessment.AssessmentId,
                PatientId = assessment.PatientId,
                DisableBackButton = DisableBackButton(assessment),
                ReadOnly = assessment.ReadOnly,
                DisableResetButton = assessment.ReadOnly,
                Stage1DecisionMade = assessment.Stage1DecisionToBeMade,
                DisplayStage1DecisionMade = workflowStage.DisplayStage1DecisionMade,
                QuestionAnswerId = questionAnswer != null ? questionAnswer.QuestionAnswerId : Guid.Empty
            };

            var option = SelectedOrSingleOption(optionsList);

            if (option != null)
            {
                buildQuestionViewModel.ChosenOption = option.QuestionOptionId;

                if (option.HasFurterQuestion)
                {
                    buildQuestionViewModel.DisplayFurtherInformationQuestion = true;
                    buildQuestionViewModel.FurtherInformationQuestion = option.FurtherQuestion;
                    buildQuestionViewModel.FurtherInformationAnswer = option.FurtherQuestionAnswer;
                }
            }

            return buildQuestionViewModel;
        }

        public QuestionViewModel UpdateQuestionViewModel(QuestionViewModel viewModel, Guid chosenValue)
        {
            if (viewModel == null) throw new ArgumentNullException();

            var chosen = viewModel.Options.First(x => x.QuestionOptionId == chosenValue);

            UpdateAllOptionsToNotSelected(viewModel);

            chosen.Selected = true;

            if (chosen.HasFurterQuestion)
            {
                viewModel.DisplayFurtherInformationQuestion = true;
                viewModel.FurtherInformationQuestion = chosen.FurtherQuestion;
            }
            else
            {
                viewModel.DisplayFurtherInformationQuestion = false;
                viewModel.FurtherInformationQuestion = null;
            }

            viewModel.ChosenOption = chosenValue;
            return viewModel;
        }

        #region private

        private void UpdateAllOptionsToNotSelected(QuestionViewModel viewModel)
        {
            foreach (var option in viewModel.Options)
            {
                option.Selected = false;
            }
        }

        private bool DisableBackButton(Assessment assessment)
        {
            return assessment.CurrentWorkflowQuestionId == assessment.PreviousWorkflowQuestionId || assessment.PreviousWorkflowQuestionId == null || assessment.ReadOnly;
        }

        private bool Selected(QuestionAnswer previousAnswer, QuestionOption x)
        {
            return previousAnswer != null && previousAnswer.QuestionOptionId == x.QuestionOptionId;
        }

        private OptionViewModel SelectedOrSingleOption(IEnumerable<OptionViewModel> options)
        {
            var optionViewModels = options as IList<OptionViewModel> ?? options.ToList();

            if (optionViewModels.Any(x => x.Selected))
                return optionViewModels.First(x => x.Selected);

            if (optionViewModels.Count() == 1)
                return optionViewModels.FirstOrDefault();

            return null;
        }

        #endregion
    }

}