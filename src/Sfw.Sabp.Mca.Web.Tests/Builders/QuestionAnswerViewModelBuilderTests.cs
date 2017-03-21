using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Web.Builders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sfw.Sabp.Mca.Web.Tests.Builders
{
    [TestClass]
    public class QuestionAnswerViewModelBuilderTests
    {
        private QuestionAnswerViewModelBuilder _builder;

        [TestInitialize]
        public void Setup()
        {
            _builder = new QuestionAnswerViewModelBuilder();
        }

        [TestMethod]
        public void BuildQuestionAnswerListViewModel_WhenCalledWithNullArguments_ShouldThrowArgumentNullException()
        {
            _builder.Invoking(x => x.BuildQuestionAnswerListViewModel(null)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void BuildQuestionAnswerListViewModel_WhenCalledWithValidModel_ShouldReturnViewModel()
        {
            var questionAnswersList = new List<QuestionAnswer>
            {
                new QuestionAnswer()
                {
                    FurtherInformation = string.Empty,
                    QuestionOption = new QuestionOption()
                    {
                        Option = new Option() {Description = "Yes"},
                    },
                    WorkflowQuestion = new WorkflowQuestion()
                    {
                        Question = new Question() {Description = "First Question"},
                        WorkflowStage = new WorkflowStage(){Description = "Stage 1"}
                    }
                },
                new QuestionAnswer()
                {
                    FurtherInformation = "Further information on second question",
                    QuestionOption = new QuestionOption()
                    {
                        Option = new Option() {Description = "No"}
                    },
                    WorkflowQuestion = new WorkflowQuestion()
                    {
                        Question = new Question() {Description = "Second Question"},
                        WorkflowStage = new WorkflowStage(){Description = "Stage 2"}
                    }
                },
                new QuestionAnswer()
                {
                    FurtherInformation = "Further information on third question",
                    QuestionOption = new QuestionOption()
                    {
                        Option = new Option(){Description = "Single option"}
                    },
                    WorkflowQuestion = new WorkflowQuestion()
                    {
                        Question = new Question(){Description = "Third Question"}, 
                        WorkflowStage = new WorkflowStage(){Description = "Stage 3"}
                    }
                },
                new QuestionAnswer()
                {
                    QuestionOption = new QuestionOption()
                    {
                        Option = new Option(){Description = "Yes"}
                    },
                    WorkflowQuestion = new WorkflowQuestion()
                    {
                        Question = new Question(){Description = "Fourth Question"}, 
                        WorkflowStage = new WorkflowStage(){Description = "Stage 4"}
                    }
                }
            };
            var questionAnswers = new QuestionAnswers()
            {
                Items = questionAnswersList
            };

            var result = _builder.BuildQuestionAnswerListViewModel(questionAnswers);

            result.Items.Should().HaveCount(4);
            result.Items.ElementAt(0).Question.Should().Contain("First Question");
            result.Items.ElementAt(0).Answer.Should().Contain("Yes");
            result.Items.ElementAt(0).FurtherInformation.Should().BeEmpty();
            result.Items.ElementAt(0).StageDescription.Should().Be("Stage 1");

            result.Items.ElementAt(1).Question.Should().Contain("Second Question");
            result.Items.ElementAt(1).Answer.Should().Contain("No");
            result.Items.ElementAt(1).FurtherInformation.Should().Contain("Further information on second question");
            result.Items.ElementAt(1).StageDescription.Should().Be("Stage 2");

            result.Items.ElementAt(2).Question.Should().Contain("Third Question");
            result.Items.ElementAt(2).Answer.Should().BeEmpty();
            result.Items.ElementAt(2).FurtherInformation.Should().Contain("Further information on third question");
            result.Items.ElementAt(2).StageDescription.Should().Be("Stage 3");

            result.Items.ElementAt(3).Question.Should().Contain("Fourth Question");
            result.Items.ElementAt(3).Answer.Should().Contain("Yes");
            result.Items.ElementAt(3).FurtherInformation.Should().BeEmpty();
            result.Items.ElementAt(3).StageDescription.Should().Be("Stage 4");
        }
    }
}
