using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Core.Enum;
using Sfw.Sabp.Mca.Infrastructure.Providers;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Helpers;
using Sfw.Sabp.Mca.Service.QueryHandlers;
using Sfw.Sabp.Mca.Web.Attributes;

namespace Sfw.Sabp.Mca.Web.Tests.Attributes
{
    [TestClass]
    public class AssessmentCompleteActionFilterTests
    {
        private AssessmentCompleteActionFilter _assessmentCompleteActionFilter;
        private IUserPrincipalProvider _userPrincipalProvider;
        private IUserRoleProvider _userRoleProvider;
        private IAssessmentHelper _assessmentHelper;
        private HttpRequestBase _httpRequest;

        [TestInitialize]
        public void Setup()
        {
            A.Fake<IQueryDispatcher>();
            _userPrincipalProvider = A.Fake<IUserPrincipalProvider>();
            _userRoleProvider = A.Fake<IUserRoleProvider>();
            _assessmentHelper = A.Fake<IAssessmentHelper>();

            _assessmentCompleteActionFilter = new AssessmentCompleteActionFilter("assessmentId", _userPrincipalProvider, _userRoleProvider, _assessmentHelper);
        }

        [TestMethod]
        public void AssessmentCompleteActionFilter_GivenNullActionParameterId_ArgumentNullExceptionShouldBeThrown()
        {
            Action filter = () => new AssessmentCompleteActionFilter(null, _userPrincipalProvider, _userRoleProvider, _assessmentHelper);

            filter.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void AssessmentCompleteActionFilter_GivenEmptyActionParameterId_ArgumentNullExceptionShouldBeThrown()
        {
            Action filter = () => new AssessmentCompleteActionFilter("", _userPrincipalProvider, _userRoleProvider, _assessmentHelper);

            filter.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void AssessmentCompleteActionFilter_GivenWhitespaceActionParameterId_ArgumentNullExceptionShouldBeThrown()
        {
            Action filter = () => new AssessmentCompleteActionFilter(" ", _userPrincipalProvider, _userRoleProvider, _assessmentHelper);

            filter.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void OnActionExecuting_GivenNullAssessmentId_NullReferenceExceptionExpected()
        {
            var filterContext = new ActionExecutingContext();

            _assessmentCompleteActionFilter.Invoking(x => x.OnActionExecuting(filterContext)).ShouldThrow<NullReferenceException>();
        }

        [TestMethod]
        public void OnActionExecuting_GivenEmptyAssessmentId_InvalidCastExceptionExpected()
        {
            var filterContext = new ActionExecutingContext()
            {
                ActionParameters = new Dictionary<string, object>() { { "assessmentId", "" } }
            };

            _assessmentCompleteActionFilter.Invoking(x => x.OnActionExecuting(filterContext)).ShouldThrow<InvalidCastException>();
        }

        [TestMethod]
        public void OnActionExecuting_GivenAssessmentId_AssessmentShouldBeRetrieved()
        {
            var assessmentId = Guid.NewGuid();

            _assessmentCompleteActionFilter.OnActionExecuting(GetActionExecutingContext(assessmentId));

            A.CallTo(() => _assessmentHelper.GetAssessment(assessmentId)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void OnActionExecuting_GivenAssessmentIsNotComplete_UserShouldBeRedirectedToPatientSearch()
        {
            var filterContext = GetActionExecutingContext(Guid.NewGuid());

            var assessment = new Assessment
            {
                StatusId = (int) AssessmentStatusEnum.InProgress
            };

            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._)).Returns(assessment);

            _assessmentCompleteActionFilter.OnActionExecuting(filterContext);

            AssertPatientSearch(filterContext);
        }

        [TestMethod]
        public void OnActionExecuting_GivenAssessmentIsCompleteIsAssignedToAnotherUser_UserShouldBeRedirectedToPatientSearch()
        {
            var filterContext = GetActionExecutingContext(Guid.NewGuid());

            var assessment = new Assessment()
            {
                AssessorDomainName = "user2@domain.com",
                StatusId = (int)AssessmentStatusEnum.Complete
            };

            A.CallTo(() => _userPrincipalProvider.CurrentUserName).Returns("user@domain.com");
            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._)).Returns(assessment);

            _assessmentCompleteActionFilter.OnActionExecuting(filterContext);

            AssertPatientSearch(filterContext);
        }

        [TestMethod]
        public void OnActionExecuting_GivenAssessmentIsCompleteAndIsAssignedToAnotherUserAndUserIsNotAdministrator_UserShouldBeRedirectedToPatientSearch()
        {
            var filterContext = GetActionExecutingContext(Guid.NewGuid());

            var assessment = new Assessment()
            {
                AssessorDomainName = "user2@domain.com",
                StatusId = (int)AssessmentStatusEnum.Complete
            };

            A.CallTo(() => _userRoleProvider.CurrentUserInAdministratorRole()).Returns(false);
            A.CallTo(() => _userPrincipalProvider.CurrentUserName).Returns("user@domain.com");
            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._)).Returns(assessment);

            _assessmentCompleteActionFilter.OnActionExecuting(filterContext);

            AssertPatientSearch(filterContext);
        }

        [TestMethod]
        public void OnActionExecuting_GivenAssessmentCompleteAndIsAssignedToAnotherUserAndUserIsAdministrator_UserCanAccessAssessment()
        {
            var filterContext = GetActionExecutingContext(Guid.NewGuid());

            var assessment = new Assessment()
            {
                AssessorDomainName = "user2@domain.com",
                StatusId = (int)AssessmentStatusEnum.Complete
            };

            A.CallTo(() => _userRoleProvider.CurrentUserInAdministratorRole()).Returns(true);
            A.CallTo(() => _userPrincipalProvider.CurrentUserName).Returns("user@domain.com");
            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._)).Returns(assessment);

            _assessmentCompleteActionFilter.OnActionExecuting(filterContext);

            filterContext.Result.Should().BeNull();
        }

        [TestMethod]
        public void OnActionExecuting_GivenAssessmentIsNotFound_UserShouldBeRedirectedToPatientSearch()
        {
            var filterContext = GetActionExecutingContext(Guid.NewGuid());

            A.CallTo(() => _assessmentHelper.GetAssessment(A<Guid>._)).Returns(null);

            _assessmentCompleteActionFilter.OnActionExecuting(filterContext);

            AssertPatientSearch(filterContext);
        }


        #region private

        private void AssertPatientSearch(ActionExecutingContext filterContext)
        {
            var result = filterContext.Result as RedirectToRouteResult;

            result.RouteValues["action"].Should().Be(MVC.Person.ActionNames.Index);
            result.RouteValues["controller"].Should().Be(MVC.Person.Name);
        }

        private ActionExecutingContext GetActionExecutingContext(Guid assessmentId)
        {
            var httpContext = A.Fake<HttpContextBase>();
            _httpRequest = A.Fake<HttpRequestBase>();

            A.CallTo(() => httpContext.Request).Returns(_httpRequest);
            
            var filterContext = new ActionExecutingContext()
            {
                ActionParameters = new Dictionary<string, object>() { { "assessmentId", assessmentId } },
                HttpContext = httpContext
            };
            return filterContext;
        }

        #endregion
    }
}
