using System;
using AutoMapper;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Web.ViewModels;
using System.Collections.Generic;

namespace Sfw.Sabp.Mca.Web.Builders
{
    public class QuestionAnswerViewModelBuilder : IQuestionAnswerViewModelBuilder
    {
        public QuestionAnswerListViewModel BuildQuestionAnswerListViewModel(QuestionAnswers questionAnswers)
        {
            if (questionAnswers == null) throw new ArgumentNullException("questionAnswers");
            var questionAnswerViewModels = new List<QuestionAnswerViewModel>();
            foreach (var questionAnswer in questionAnswers.Items)
            {
                var questionAnswerViewModel = Mapper.DynamicMap<QuestionAnswer, QuestionAnswerViewModel>(questionAnswer);
                questionAnswerViewModel.Question = questionAnswer.WorkflowQuestion.Question.Description;
                questionAnswerViewModel.StageDescription = questionAnswer.WorkflowQuestion.WorkflowStage.Description;
                questionAnswerViewModel.Answer = questionAnswer.QuestionOption.Option.Description.Contains("Single option") ? string.Empty : questionAnswer.QuestionOption.Option.Description;
                questionAnswerViewModel.FurtherInformation = questionAnswer.FurtherInformation ?? string.Empty;
                questionAnswerViewModels.Add(questionAnswerViewModel);
            }
            var viewModel = new QuestionAnswerListViewModel
            {
                Items = questionAnswerViewModels
            };
            return viewModel;
        }
    }
}