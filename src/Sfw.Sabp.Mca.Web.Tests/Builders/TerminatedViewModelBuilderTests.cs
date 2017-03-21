using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Infrastructure.Providers;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Web.Builders;
using Sfw.Sabp.Mca.Web.ViewModels;
using System;

namespace Sfw.Sabp.Mca.Web.Tests.Builders
{
    [TestClass]
    public class TerminatedViewModelBuilderTests
    {
        private TerminatedViewModelBuilder _builder;
        private IDateTimeProvider _dateTimeProvider;

        [TestInitialize]
        public void Setup()
        {
            _dateTimeProvider = A.Fake<IDateTimeProvider>();
            A.CallTo(() => _dateTimeProvider.Now).Returns(new DateTime(2015, 1, 1));

            _builder = new TerminatedViewModelBuilder(_dateTimeProvider);
        }

        [TestMethod]
        public void BuildTerminatedViewModel_GivenValidModel_ViewModelShouldBeReturned()
        {
            var result = _builder.BuildTerminatedAssessmentViewModel(A.Dummy<Assessment>());

            result.Should().BeOfType<TerminatedViewModel>();
        }

        [TestMethod]
        public void BuildTerminatedViewModel_GivenValidModel_TerminatedViewModelPropertiesShouldBeMapped()
        {
            var assessment = A.Fake<Assessment>();

            var result = _builder.BuildTerminatedAssessmentViewModel(assessment);

            result.DateAssessmentEnded.ShouldBeEquivalentTo(new DateTime(2015, 1, 1));
        }

        [TestMethod]
        public void BuildTerminatedViewModel_GivenNullAssessment_ArgumentNullExceptionShouldBeThrown()
        {
            _builder.Invoking(x => x.BuildTerminatedAssessmentViewModel(null)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void BuildTerminatedViewModel_GivenValidAssessment_AssessmentPropertiesShouldBeMapped()
        {
            var assessment = new Assessment()
            {
                DateAssessmentEnded = new DateTime(2015, 05, 08)
            };

            var result = _builder.BuildTerminatedAssessmentViewModel(assessment);

            result.DateAssessmentEnded.ShouldBeEquivalentTo(new DateTime(2015, 05, 08));
        }
    }
}
