using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Core.Enum;
using Sfw.Sabp.Mca.Web.ViewModels;

namespace Sfw.Sabp.Mca.Web.Tests.ViewModel
{
    [TestClass]
    public class StatusViewModelTests
    {
        private StatusViewModel _model;

        [TestInitialize]
        public void Setup()
        {
            _model = new StatusViewModel();
        }

        [TestMethod]
        public void InProgress_GivenStatusIsInProgress_TrueShouldBeReturned()
        {
            _model.StatusId = (int) AssessmentStatusEnum.InProgress;

            _model.InProgress().Should().BeTrue();
        }

        [TestMethod]
        public void InProgress_GivenStatusIsNotInProgress_FalseShouldBeReturned()
        {
            foreach (var value in Enum.GetValues(typeof(AssessmentStatusEnum)))
            {
                if ((int)value != (int)AssessmentStatusEnum.InProgress)
                {
                    _model.StatusId = (int)value;
                    _model.InProgress().Should().BeFalse();
                }
            }
        }

        [TestMethod]
        public void InBreak_GivenStatusIsBreak_TrueShouldBeReturned()
        {
            _model.StatusId = (int)AssessmentStatusEnum.Break;

            _model.Break().Should().BeTrue();
        }

        [TestMethod]
        public void InBreak_GivenStatusItNotBreak_FalseShouldBeReturned()
        {
            foreach (var value in Enum.GetValues(typeof(AssessmentStatusEnum)))
            {
                if ((int) value != (int)AssessmentStatusEnum.Break)
                {
                    _model.StatusId = (int)value;
                    _model.Break().Should().BeFalse();
                }
            }
        }

        [TestMethod]
        public void Complete_GivenStatusIsComplete_TrueShouldBeReturned()
        {
            _model.StatusId = (int)AssessmentStatusEnum.Complete;

            _model.Complete().Should().BeTrue();
        }

        [TestMethod]
        public void Complete_GivenStatusItNotComplete_FalseShouldBeReturned()
        {
            foreach (var value in Enum.GetValues(typeof(AssessmentStatusEnum)))
            {
                if ((int)value != (int)AssessmentStatusEnum.Complete)
                {
                    _model.StatusId = (int)value;
                    _model.Complete().Should().BeFalse();
                }
            }
        }

        [TestMethod]
        public void ReadyToComplete_GivenStatusIsReadyToComplete_TrueShouldBeReturned()
        {
            _model.StatusId = (int)AssessmentStatusEnum.ReadyToComplete;

            _model.ReadyToComplete().Should().BeTrue();
        }

        [TestMethod]
        public void ReadyToComplete_GivenStatusIsNotReadyToComplete_FalseShouldBeReturned()
        {
            foreach (var value in Enum.GetValues(typeof(AssessmentStatusEnum)).Cast<object>().Where(value => (int)value != (int)AssessmentStatusEnum.ReadyToComplete))
            {
                _model.StatusId = (int)value;
                _model.ReadyToComplete().Should().BeFalse();
            }
        }
    }
}
