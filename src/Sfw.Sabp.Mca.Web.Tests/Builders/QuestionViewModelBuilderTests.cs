using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Web.Builders;
using Sfw.Sabp.Mca.Web.ViewModels;

namespace Sfw.Sabp.Mca.Web.Tests.Builders
{
    [TestClass]
    public class QuestionViewModelBuilderTests
    {
        private QuestionViewModelBuilder _questionViewModelBuilder;
        private Assessment _assessment;
        private Guid? _chosenOption;

        [TestInitialize]
        public void Setup()
        {
            _chosenOption = null;
            _questionViewModelBuilder = new QuestionViewModelBuilder();
        }

        [TestMethod]
        public void BuildQuestionViewModel_GivenNullAssessment_AgrumentNullExceptionExpected()
        {
            _questionViewModelBuilder.Invoking(x => x.BuildQuestionViewModel(null, A<QuestionAnswer>._)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void BuildQuestionViewModel_GivenAssessment_QuestionViewModelShouldBeReturned()
        {
            var result = _questionViewModelBuilder.BuildQuestionViewModel(A.Dummy<Assessment>(), A.Dummy<QuestionAnswer>());

            result.Should().BeOfType<QuestionViewModel>();
            result.Should().NotBeNull();
        }

        [TestMethod]
        public void BuildQuestionViewModel_GivenAssessment_StageDescriptionPropertyShouldBeMapped()
        {
            const string stageDescription = "stageDescription";

            _assessment = ValidAssessment();
            _assessment.WorkflowQuestion.WorkflowStage.Description = stageDescription;

            var result = BuildQuestionViewModel();

            result.StageDescription.Should().Be(stageDescription);
        }

        [TestMethod]
        public void BuildQuestionViewModel_GivenAssessmentWithNullCurrentWorkflowQuestionId_NullWorkFlowQuestionExceptionExpected()
        {
            _assessment = new Assessment();

            _questionViewModelBuilder.Invoking(x => x.BuildQuestionViewModel(_assessment, A<QuestionAnswer>._))
                .ShouldThrow<QuestionViewModelBuilder.NullWorkFlowQuestionException>();
        }

        [TestMethod]
        public void BuildQuestionViewModel_GivenAssessment_QuestionGuidanceTextShouldBeMapped()
        {
            const string guidanceText = "guidanceText";

            _assessment = ValidAssessment();
            _assessment.WorkflowQuestion.Question.Guidance = guidanceText;

            var result = BuildQuestionViewModel();

            result.Guidance.Should().Be(guidanceText);
        }

        [TestMethod]
        public void BuildQuestionViewModel_GivenAssessment_QuestionTextShouldBeMapped()
        {
            const string question = "question";

            _assessment = ValidAssessment();
            _assessment.WorkflowQuestion.Question.Description = question;

            var result = BuildQuestionViewModel();

            result.Question.Should().Be(question);
        }

        [TestMethod]
        public void BuildQuestionViewModel_GivenAssessmentQuestionWithNullGuidance_DisplayGuidanceShouldBeFalse()
        {
            _assessment = ValidAssessment();
            _assessment.WorkflowQuestion.Question.Guidance = string.Empty;

            var result = BuildQuestionViewModel();
            result.DisplayGuidance.Should().BeFalse();
        }

        [TestMethod]
        public void BuildQuestionViewModel_GivenAssessmentQuestionWithEmptyGuidance_DisplayGuidanceShouldBeFalse()
        {
            _assessment = ValidAssessment();
            _assessment.WorkflowQuestion.Question.Guidance = string.Empty;

            var result = BuildQuestionViewModel();
            result.DisplayGuidance.Should().BeFalse();
        }

        [TestMethod]
        public void BuildQuestionViewModel_GivenAssessmentQuestionWithWhitespaceGuidance_DisplayGuidanceShouldBeFalse()
        {
            _assessment = ValidAssessment();
            _assessment.WorkflowQuestion.Question.Guidance = " ";

            var result = BuildQuestionViewModel();
            result.DisplayGuidance.Should().BeFalse();
        }

        [TestMethod]
        public void BuildQuestionViewModel_GivenAssessmentQuestionWithGuidance_DisplayGuidanceShouldBetrue()
        {
            _assessment = ValidAssessment();
            _assessment.WorkflowQuestion.Question.Guidance = "guidance";

            var result = BuildQuestionViewModel();
            result.DisplayGuidance.Should().BeTrue();
        }

        [TestMethod]
        public void BuildQuestionViewModel_GivenAssessmentQuestionWithOptions_QuestionOptionsShouldBeMapped()
        {
            var firstQuestionOptionId = Guid.NewGuid();
            var secondQuestionOptionId = Guid.NewGuid();
            var firstOptionId = Guid.NewGuid();
            var secondOptionId = Guid.NewGuid();

            _assessment = new Assessment()
            {
                WorkflowQuestion = new WorkflowQuestion()
                {
                    WorkflowStage = new WorkflowStage(),
                    Question = new Question()
                    {
                        QuestionOptions = new Collection<QuestionOption>()
                        {
                            new QuestionOption()
                            {
                                QuestionOptionId = firstQuestionOptionId,
                                Option = new Option()
                                {
                                    OptionId = firstOptionId,
                                    Description = "option1"
                                }
                            },
                            new QuestionOption()
                            {
                                QuestionOptionId = secondQuestionOptionId,
                                Option = new Option()
                                {
                                    OptionId = secondOptionId,
                                    Description = "option2"
                                }
                            }
                        }
                    }
                }
            };
            
            var result = BuildQuestionViewModel();

            result.Options.Should().Contain(x => x.Description == "option1");
            result.Options.Should().Contain(x => x.OptionId == firstOptionId);
            result.Options.Should().Contain(x => x.QuestionOptionId == firstQuestionOptionId);
            result.Options.Should().Contain(x => x.Description == "option2");
            result.Options.Should().Contain(x => x.OptionId == secondOptionId);
            result.Options.Should().Contain(x => x.QuestionOptionId == secondQuestionOptionId);
            result.Options.Should().HaveCount(2);
        }

        [TestMethod]
        public void BuildQuestionViewModel_GivenAssessmentQuestionWithNoOptions_QuestionOptionsShouldBeMapped()
        {
            _assessment = ValidAssessment();
            _assessment.WorkflowQuestion.Question.QuestionOptions.Clear();

            var result = BuildQuestionViewModel();

            result.Options.Should().HaveCount(0);
        }


        [TestMethod]
        public void BuildQuestionViewModel_GivenAssessment_AssessmentIdPropertyShouldBeMapped()
        {
            var assessmentId = Guid.NewGuid();

            _assessment = ValidAssessment();
            _assessment.AssessmentId = assessmentId;

            var result = BuildQuestionViewModel();

            result.AssessmentId.Should().Be(assessmentId);
        }

        [TestMethod]
        public void BuildQuestionViewModel_GivenAssessmentWithChosenOptionAndChosenOptionHasFurtherQuestion_FurtherQuestionPropertiesShouldBeMapped()
        {
            _assessment = new Assessment()
            {
                WorkflowQuestion = new WorkflowQuestion()
                {
                    WorkflowStage = new WorkflowStage(),
                    Question = new Question()
                    {
                        QuestionOptions = new Collection<QuestionOption>()
                        {
                            new QuestionOption()
                            {
                                Option = new Option(),
                                FurtherQuestion = new Question()
                                {
                                    Description = "furtherquestion"
                                }
                            }
                        }
                    }
                }
            };

            var result = BuildQuestionViewModel();

            result.Options.First().FurtherQuestion.Should().Be("furtherquestion");
            result.Options.First().HasFurterQuestion.Should().BeTrue();
        }

        [TestMethod]
        public void BuildQuestionViewModel_GivenAssessmentWithChosenOptionAndChosenOptionHasNoFurtherQuestion_DisplayFurtherInformationQuestionShouldBeFalse()
        {
            _chosenOption = Guid.NewGuid();
            _assessment = ValidAssessment();
            _assessment.WorkflowQuestion.Question.QuestionOptions.Add(new QuestionOption()
            {
                Option = new Option()
                {
                    OptionId = Guid.NewGuid()
                },
                OptionId = _chosenOption.Value
            });

            var result = BuildQuestionViewModel();

            result.DisplayFurtherInformationQuestion.Should().BeFalse();
        }

        [TestMethod]
        public void BuildQuestionViewModel_GivenAssessmentWithNoChosenOption_DisplayFurtherInformationQuestionShouldBeFalse()
        {
            _assessment = ValidAssessment();

            var result = BuildQuestionViewModel();

            result.DisplayFurtherInformationQuestion.Should().BeFalse();
        }

        [TestMethod]
        public void UpdateQuestionViewModel_GivenNullViewModel_ArgumentNullExceptionExpected()
        {
            _questionViewModelBuilder.Invoking(x => x.UpdateQuestionViewModel(null, A<Guid>._)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void UpdateQuestionViewModel_GivenValidViewModelAndChosenValue_ChosenOptionShouldBeSelected()
        {
            var chosenOption = Guid.NewGuid();

            var options = new List<OptionViewModel>()
            {
                new OptionViewModel() {QuestionOptionId = chosenOption},
                new OptionViewModel() {QuestionOptionId = Guid.NewGuid()}
            };

            var model = new QuestionViewModel()
            {
                Options = options
            };

            var result = _questionViewModelBuilder.UpdateQuestionViewModel(model, chosenOption);

            result.Options.First(x => x.QuestionOptionId == chosenOption).Selected.Should().BeTrue();
        }

        [TestMethod]
        public void UpdateQuestionViewModel_GivenValidViewModelAndChosenValue_ChosenOptionViewModelPropertyShouldBeSet()
        {
            var chosenOption = Guid.NewGuid();

            var options = new List<OptionViewModel>()
            {
                new OptionViewModel()
                {
                    QuestionOptionId = chosenOption
                },
            };

            var model = new QuestionViewModel()
            {
                Options = options
            };

            var result = _questionViewModelBuilder.UpdateQuestionViewModel(model, chosenOption);

            result.ChosenOption.Should().Be(chosenOption);
        }

        [TestMethod]
        public void UpdateQuestionViewModel_GivenValidViewModelAndChosenValueAndChosenOptionHasFurtherQuestion_FurtherQuestionPropertiesShouldBeSet()
        {
            var chosenOption = Guid.NewGuid();

            var options = new List<OptionViewModel>()
            {
                new OptionViewModel()
                {
                    QuestionOptionId = chosenOption,
                    FurtherQuestion = "furtherquestion"
                },
            };

            var model = new QuestionViewModel()
            {
                Options = options
            };

            var result = _questionViewModelBuilder.UpdateQuestionViewModel(model, chosenOption);

            result.FurtherInformationQuestion.Should().Be("furtherquestion");
            result.DisplayFurtherInformationQuestion.Should().BeTrue();
        }

        [TestMethod]
        public void UpdateQuestionViewModel_GivenValidViewModelAndChosenValueAndChosenOptionHasNoFurtherQuestion_FurtherQuestionShouldNotBeSet()
        {
            var chosenOption = Guid.NewGuid();

            var options = new List<OptionViewModel>()
            {
                new OptionViewModel()
                {
                    QuestionOptionId = chosenOption,
                    FurtherQuestion = ""
                },
            };

            var model = new QuestionViewModel()
            {
                DisplayFurtherInformationQuestion = true,
                Options = options
            };

            var result = _questionViewModelBuilder.UpdateQuestionViewModel(model, chosenOption);

            result.DisplayFurtherInformationQuestion.Should().BeFalse();
        }

        [TestMethod]
        public void BuildQuestionViewModel_GivenSingleOption_ChosenOptionShouldBeSet()
        {
            var questionOptionId = Guid.NewGuid();

            var options = new List<QuestionOption>()
            {
                new QuestionOption()
                {
                    Option = A.Fake<Option>(),
                    Question = A.Fake<Question>(),
                    QuestionOptionId = questionOptionId
                }
            };

            var assessment = A.Fake<Assessment>();
            A.CallTo(() => assessment.WorkflowQuestion.Question.QuestionOptions).Returns(options);

            var result = _questionViewModelBuilder.BuildQuestionViewModel(assessment, A.Dummy<QuestionAnswer>());

            result.ChosenOption.ShouldBeEquivalentTo(questionOptionId);
        }

        [TestMethod]
        public void BuildQuestionViewModel_GivenSingleOptionThatHasFurtherInformation_FurtherInformationPropertiesShouldBeSet()
        {
            var options = new List<QuestionOption>()
            {
                new QuestionOption()
                {
                    Option = A.Fake<Option>(),
                    Question = A.Fake<Question>(),
                    FurtherQuestion = new Question()
                    {
                        Description = "question"
                    }
                }
            };

            var assessment = A.Fake<Assessment>();
            A.CallTo(() => assessment.WorkflowQuestion.Question.QuestionOptions).Returns(options);

            var result = _questionViewModelBuilder.BuildQuestionViewModel(assessment, A.Dummy<QuestionAnswer>());

            result.DisplayFurtherInformationQuestion.Should().BeTrue();
            result.FurtherInformationQuestion.Should().Be("question");
        }

        [TestMethod]
        public void BuildQuestionViewModel_GivenMultipleOptions_ChosenOptionShouldNotBeSet()
        {
            var options = new List<QuestionOption>()
            {
                new QuestionOption()
                {
                    Option = A.Fake<Option>(),
                    Question = A.Fake<Question>(),
                    QuestionOptionId = Guid.NewGuid()
                },
                new QuestionOption()
                {
                    Option = A.Fake<Option>(),
                    Question = A.Fake<Question>(),
                    QuestionOptionId = Guid.NewGuid()
                }
            };

            var assessment = A.Fake<Assessment>();
            A.CallTo(() => assessment.WorkflowQuestion.Question.QuestionOptions).Returns(options);

            var result = _questionViewModelBuilder.BuildQuestionViewModel(assessment, A.Dummy<QuestionAnswer>());

            result.ChosenOption.Should().NotHaveValue();
        }

        [TestMethod]
        public void BuildQuestionViewModel_GivenMultipleOptions_FurtherInformationPropertiesShouldNotBeSet()
        {
            var options = new List<QuestionOption>()
            {
                new QuestionOption()
                {
                    Option = A.Fake<Option>(),
                    Question = A.Fake<Question>(),
                    QuestionOptionId = Guid.NewGuid()
                },
                new QuestionOption()
                {
                    Option = A.Fake<Option>(),
                    Question = A.Fake<Question>(),
                    QuestionOptionId = Guid.NewGuid()
                }
            };

            var assessment = A.Fake<Assessment>();
            A.CallTo(() => assessment.WorkflowQuestion.Question.QuestionOptions).Returns(options);

            var result = _questionViewModelBuilder.BuildQuestionViewModel(assessment, A.Dummy<QuestionAnswer>());

            result.DisplayFurtherInformationQuestion.Should().BeFalse();
            result.FurtherInformationQuestion.Should().BeNullOrWhiteSpace();
        }

        [TestMethod]
        public void BuildQuestionViewModel_GivenAssessment_PatientIdPropertyShouldBeMapped()
        {
            var patientId = Guid.NewGuid();

            _assessment = ValidAssessment();
            _assessment.PatientId = patientId;

            var result = BuildQuestionViewModel();

            result.PatientId.Should().Be(patientId);
        }

        [TestMethod]
        public void BuildQuestionViewModel_GivenAssessment_QuestionOptionsShouldBeOrderedByAscendingOrder()
        {
            var assessment = new Assessment()
            {
                WorkflowQuestion = new WorkflowQuestion()
                {
                    WorkflowStage = new WorkflowStage(),
                    Question = new Question()
                    {
                        QuestionOptions = new Collection<QuestionOption>()
                        {
                            new QuestionOption()
                            {
                                Option = new Option()
                                {
                                    OptionId = Guid.NewGuid(),
                                    Description = "option1",
                                    
                                },
                                Order = 10
                            },
                            new QuestionOption()
                            {
                                Option = new Option()
                                {
                                    OptionId = Guid.NewGuid(),
                                    Description = "option2"
                                },
                                Order = 5
                            }
                        }
                    }
                }
            };

            var result = _questionViewModelBuilder.BuildQuestionViewModel(assessment, A.Dummy<QuestionAnswer>());

            result.Options.ElementAt(0).Description.Should().Be("option2");
            result.Options.ElementAt(1).Description.Should().Be("option1");

        }

        [TestMethod]
        public void BuildQuestionViewModel_GivenAssessment_StageDescriptionStylePropertyShouldBeSet()
        {
            _assessment = ValidAssessment();

            var result = BuildQuestionViewModel();

            result.StageDescriptionStyle.Should().Be("stage");
        }

        [TestMethod]
        public void BuildQuestionViewModel_GivenAssessmentWhereBackButtonWasClicked_BackButtonShouldBeDisabled()
        {
            var questionId = Guid.NewGuid();

            _assessment = new Assessment()
            {
                CurrentWorkflowQuestionId = questionId,
                PreviousWorkflowQuestionId = questionId,
                WorkflowQuestion = A.Fake<WorkflowQuestion>()
            };

            var result = BuildQuestionViewModel();

            result.DisableBackButton.Should().BeTrue();
        }

        [TestMethod]
        public void BuildQuestionViewModel_GivenAssessmentWhereBackButtonWasNotClicked_BackButtonShouldBeEnabled()
        {
            var currentQuestionId = Guid.NewGuid();
            var previousQuestionId = Guid.NewGuid();

            _assessment = new Assessment()
            {
                CurrentWorkflowQuestionId = currentQuestionId,
                PreviousWorkflowQuestionId = previousQuestionId,
                WorkflowQuestion = A.Fake<WorkflowQuestion>()
            };

            var result = BuildQuestionViewModel();

            result.DisableBackButton.Should().BeFalse();
        }

        [TestMethod]
        public void BuildQuestionViewModel_GivenAssessmentWithNoPreviousQuestion_BackButtonShouldBeDisabled()
        {
            var currentQuestionId = Guid.NewGuid();

            _assessment = new Assessment()
            {
                CurrentWorkflowQuestionId = currentQuestionId,
                PreviousWorkflowQuestionId = null,
                WorkflowQuestion = A.Fake<WorkflowQuestion>()
            };

            var result = BuildQuestionViewModel();

            result.DisableBackButton.Should().BeTrue();
        }

        [TestMethod]
        public void BuildQuestionViewModel_GivenAssessment_ReadOnlyPropertyShouldBeSet()
        {
            _assessment = ValidAssessment();
            _assessment.ReadOnly = true;

            var result = BuildQuestionViewModel();

            result.ReadOnly.Should().BeTrue();
        }

        [TestMethod]
        public void BuildQuestionViewModel_GivenAssessmentWithQuestionThatHasPreviouslySelectedOption_ChosenOptionPropertiesShouldBeSelected()
        {
            var selectedId = Guid.NewGuid();
            var notSelectedId = Guid.NewGuid();

            _assessment = new Assessment()
            {
                WorkflowQuestion = new WorkflowQuestion()
                {
                    WorkflowStage = A.Fake<WorkflowStage>(),
                    Question = new Question()
                    {
                        QuestionOptions = new List<QuestionOption>()
                        {
                            new QuestionOption()
                            {
                                QuestionOptionId = selectedId,
                                Option = A.Fake<Option>()
                            },
                            new QuestionOption()
                            {
                                QuestionOptionId = notSelectedId,
                                Option = A.Fake<Option>()
                            }
                        }
                    }
                }
            };

            var questionAnswer = new QuestionAnswer()
            {
                QuestionOptionId = selectedId
            };

            var result = _questionViewModelBuilder.BuildQuestionViewModel(_assessment, questionAnswer);

            result.ChosenOption.Should().Be(selectedId);
            result.Options.First(x => x.Selected).QuestionOptionId.Should().Be(selectedId);
            result.Options.Count(x => x.Selected).Should().Be(1);
            result.Options.First(x => !x.Selected).QuestionOptionId.Should().Be(notSelectedId);
            result.Options.Count(x => !x.Selected).Should().Be(1);
        }

        [TestMethod]
        public void BuildQuestionViewModel_GivenAssessmentWithQuestionThatHasPreviouslySelectedOptionWithFurtherInformation_FurtherInformationPropertiesShouldBeSet()
        {
            var selectedId = Guid.NewGuid();

            _assessment = new Assessment()
            {
                WorkflowQuestion = new WorkflowQuestion()
                {
                    WorkflowStage = A.Fake<WorkflowStage>(),
                    Question = new Question()
                    {
                        QuestionOptions = new List<QuestionOption>()
                        {
                            new QuestionOption()
                            {
                                QuestionOptionId = selectedId,
                                Option = A.Fake<Option>(),
                                FurtherQuestion = new Question()
                                {
                                    Description = "description"
                                }
                            }
                        }
                    }
                }
            };

            var questionAnswer = new QuestionAnswer()
            {
                QuestionOptionId = selectedId,
                FurtherInformation = "furtherinfo"
            };

            var result = _questionViewModelBuilder.BuildQuestionViewModel(_assessment, questionAnswer);

            result.DisplayFurtherInformationQuestion.Should().BeTrue();
            result.FurtherInformationAnswer.Should().Be("furtherinfo");
        }

        [TestMethod]
        public void BuildQuestionViewModel_GivenAssessmentIsReadonly_BackButtonShouldBeDisabled()
        {
            _assessment = new Assessment()
            {
                ReadOnly = true,
                WorkflowQuestion = A.Fake<WorkflowQuestion>(),
                PreviousWorkflowQuestionId = Guid.NewGuid()
            };

            var result = BuildQuestionViewModel();

            result.DisableBackButton.Should().BeTrue();
        }

        [TestMethod]
        public void BuildQuestionViewModel_GivenAssessmentIsReadonly_ResetButtonShouldBeDisabled()
        {
            _assessment = new Assessment()
            {
                ReadOnly = true,
                WorkflowQuestion = A.Fake<WorkflowQuestion>()
            };

            var result = BuildQuestionViewModel();

            result.DisableResetButton.Should().BeTrue();
        }

        [TestMethod]
        public void BuildQuestionViewModel_GivenAssessmentIsNotReadonly_ResetButtonShouldBeEnabled()
        {
            _assessment = new Assessment()
            {
                ReadOnly = false,
                WorkflowQuestion = A.Fake<WorkflowQuestion>()
            };

            var result = BuildQuestionViewModel();

            result.DisableResetButton.Should().BeFalse();
        }

        [TestMethod]
        public void UpdateQuestionViewModel_GivenTheChosenOptionHasChanged_OnlyTheNewlyChosenOptionShouldBeSelected()
        {
            var oldSelectedOptionId = Guid.NewGuid();
            var newSelectedOptionId = Guid.NewGuid();

            var viewModel = new QuestionViewModel()
            {
                Options = new List<OptionViewModel>()
                {
                    new OptionViewModel()
                    {
                        QuestionOptionId = oldSelectedOptionId,
                        Selected = true
                    },
                    new OptionViewModel()
                    {
                        QuestionOptionId = newSelectedOptionId,
                        Selected = false
                    }
                },
                ChosenOption = oldSelectedOptionId
            };

            var result = _questionViewModelBuilder.UpdateQuestionViewModel(viewModel, newSelectedOptionId);

            result.Options.First(x => x.QuestionOptionId == oldSelectedOptionId).Selected.Should().BeFalse();
            result.Options.First(x => x.QuestionOptionId == newSelectedOptionId).Selected.Should().BeTrue();
        }

        [TestMethod]
        public void BuildQuestionViewModel_GivenAssessment_Stage1DecisionMadeShouldBeMapped()
        {
            const string stage1DecisionMade = "Stage1DecisionMade";

            _assessment = ValidAssessment();
            _assessment.Stage1DecisionToBeMade = stage1DecisionMade;

            var result = BuildQuestionViewModel();

            result.Stage1DecisionMade.Should().Be(stage1DecisionMade);
        }

        [TestMethod]
        public void BuildQuestionViewModel_GivenAssessment_DisplayStage1DecisionMadeMadeShouldBeMappedAndTrue()
        {
            const bool displayStage1DecisionMade = true;

            _assessment = ValidAssessment();
            _assessment.WorkflowQuestion.WorkflowStage.DisplayStage1DecisionMade = displayStage1DecisionMade;

            var result = BuildQuestionViewModel();

            result.DisplayStage1DecisionMade.Should().Be(displayStage1DecisionMade);
            result.DisplayStage1DecisionMade.Should().BeTrue(); 
        }

        [TestMethod]
        public void BuildQuestionViewModel_GivenAssessment_DisplayStage1DecisionMadeMadeShouldBeMappedAndFalse()
        {
            const bool displayStage1DecisionMade = false;

            _assessment = ValidAssessment();
            _assessment.WorkflowQuestion.WorkflowStage.DisplayStage1DecisionMade = displayStage1DecisionMade;

            var result = BuildQuestionViewModel();

            result.DisplayStage1DecisionMade.Should().Be(displayStage1DecisionMade);
            result.DisplayStage1DecisionMade.Should().BeFalse();            
        }

        [TestMethod]
        public void BuildQuestionViewModel_GivenQuestionAnswer_QuestionAnswerIdPropertyWouldBeMapped()
        {
            var questionAnswer = new QuestionAnswer()
            {
                QuestionAnswerId = Guid.NewGuid()
            };

            var result = _questionViewModelBuilder.BuildQuestionViewModel(ValidAssessment(), questionAnswer);

            result.QuestionAnswerId.Should().Be(questionAnswer.QuestionAnswerId);
        }

        [TestMethod]
        public void BuildQuestionViewModel_GivenQuestionAnswerIsNull_QuestionAnswerIdPropertyShouldNotBeSet()
        {
            var result = _questionViewModelBuilder.BuildQuestionViewModel(ValidAssessment(), null);

            result.QuestionAnswerId.Should().Be(Guid.Empty);
        }

        #region private

        private QuestionViewModel BuildQuestionViewModel()
        {
            return _questionViewModelBuilder.BuildQuestionViewModel(_assessment, new QuestionAnswer() {QuestionAnswerId = Guid.NewGuid()});
        }

        private Assessment ValidAssessment()
        {
            var assessment = new Assessment()
            {
                WorkflowQuestion = new WorkflowQuestion()
                {
                    WorkflowStage = new WorkflowStage()
                    {
                        ShortDescription = "stage"
                    },
                    Question = new Question()
                    {
                        QuestionOptions = new Collection<QuestionOption>()
                        {
                            new QuestionOption()
                            {
                                Option = new Option()
                                {
                                    OptionId = Guid.NewGuid(),
                                    Description = "option1"
                                }
                            },
                            new QuestionOption()
                            {
                                Option = new Option()
                                {
                                    OptionId = Guid.NewGuid(),
                                    Description = "option2"
                                }
                            }
                        }
                    }
                }
            };
            return assessment;
        }

        #endregion
    }
}
