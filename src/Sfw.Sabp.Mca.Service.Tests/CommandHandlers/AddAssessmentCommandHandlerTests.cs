using System;
using System.Data.Entity;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Infrastructure.Providers;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.CommandHandlers;
using Sfw.Sabp.Mca.Service.Commands;

namespace Sfw.Sabp.Mca.Service.Tests.CommandHandlers
{
    [TestClass]
    public class AddAssessmentCommandHandlerTests
    {
        private IUnitOfWork _unitOfWork;
        private IUserPrincipalProvider _userPrincipalProvider;
        private IActiveDirectoryPrincipal _activeDirectoryPrincipalProvider;

        private AddAssessmentCommandHandler _handler;

        [TestInitialize]
        public void Startup()
        {
            _unitOfWork = A.Fake<IUnitOfWork>();
            _userPrincipalProvider = A.Fake<IUserPrincipalProvider>();
            _activeDirectoryPrincipalProvider = A.Fake<IActiveDirectoryPrincipal>();

            A.CallTo(() => _userPrincipalProvider.CurrentUserName).Returns("user");

            _handler = new AddAssessmentCommandHandler(_unitOfWork, _activeDirectoryPrincipalProvider, _userPrincipalProvider);
        }

        [TestMethod]
        public void Execute_GivenAddAssessmentCommandIsNull_ArgumentNullExceptionShouldBeThrown()
        {
            _handler.Invoking(x =>  x.Execute(null)).ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void Execute_GivenAddAssessmentCommand_AssessmentShouldBeAddedToContext()
        {
            var fakeContext = A.Fake<DbContext>();
            var set = A.Fake<DbSet<Assessment>>();
            var workflowVersionId = Guid.NewGuid();
            var initialQuestion = Guid.NewGuid();
            var assessmentId = Guid.NewGuid();
            var patientId = Guid.NewGuid();

            var assessmentCommand = new AddAssessmentCommand()
            {
                AssessmentId = assessmentId,
                DateAssessmentStarted = new DateTime(2015, 01, 01),
                Stage1DecisionToBeMade = "decision",
                Stage1DecisionClearlyMade = true,
                Stage1DecisionConfirmation = "confirmation",
                Stage1InfoText = "info",
                WorkflowVersionId = workflowVersionId,
                CurrentWorkflowQuestionId = initialQuestion,
                StatusId = 1,
                PatientId = patientId
            };

            A.CallTo(() => _unitOfWork.Context).Returns(fakeContext);
            A.CallTo(() => fakeContext.Set<Assessment>()).Returns(set);
            A.CallTo(() => _activeDirectoryPrincipalProvider.DisplayNameForCurrentUser()).Returns("user");

            _handler.Execute(assessmentCommand);

            A.CallTo(() => fakeContext.Set<Assessment>().Add(A<Assessment>.That.Matches(
                x => x.DateAssessmentStarted.Equals(new DateTime(2015, 01, 01))
                && x.Stage1DecisionClearlyMade
                && x.Stage1DecisionToBeMade == "decision"
                && x.Stage1DecisionConfirmation == "confirmation"
                && x.Stage1InfoText == "info"
                && x.WorkflowVersionId == workflowVersionId
                && x.CurrentWorkflowQuestionId == initialQuestion
                && x.StatusId == 1
                && x.AssessorName == "user"
                && x.AssessmentId == assessmentId
                && x.PatientId == patientId))).MustHaveHappened(Repeated.Exactly.Once);
        }

    }
}
