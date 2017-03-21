using System;
using System.Web.Mvc;
using System.Web.Routing;
using Sfw.Sabp.Mca.Core.Enum;
using Sfw.Sabp.Mca.Infrastructure.Providers;
using Sfw.Sabp.Mca.Model;
using Sfw.Sabp.Mca.Service.Helpers;

namespace Sfw.Sabp.Mca.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class AssessmentCompleteActionFilter : ActionFilterAttribute
    {
        #region exception

        public class AssessmentIdInvalidException : Exception { }

        #endregion

        private readonly IUserPrincipalProvider _userPrincipalProvider;
        private readonly IUserRoleProvider _userRoleProvider;
        private readonly IAssessmentHelper _assessmentHelper;

        private readonly string _actionParameterId;

        public AssessmentCompleteActionFilter(string actionParameterId, IUserPrincipalProvider userPrincipalProvider, IUserRoleProvider userRoleProvider, IAssessmentHelper assessmentHelper)
        {
            if (string.IsNullOrWhiteSpace(actionParameterId)) throw new ArgumentNullException();

            _userPrincipalProvider = userPrincipalProvider;
            _userRoleProvider = userRoleProvider;
            _assessmentHelper = assessmentHelper;
            _actionParameterId = actionParameterId;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var assessmentId = GetAssessmentId(filterContext);

            var assessment = _assessmentHelper.GetAssessment(assessmentId);

            if (assessment == null)
            {
                RedirectResult(filterContext);
                return;
            }

            if (assessment.StatusId != (int)AssessmentStatusEnum.Complete)
            {
                RedirectResult(filterContext);
                return;
            }

            if (NotCurrentUserIsAssessor(assessment) && !_userRoleProvider.CurrentUserInAdministratorRole())
            {
                RedirectResult(filterContext);
                return;
            }

            base.OnActionExecuting(filterContext);
        }

        #region private

        private bool NotCurrentUserIsAssessor(Assessment assessment)
        {
            return assessment.AssessorDomainName != _userPrincipalProvider.CurrentUserName;
        }

        private void RedirectResult(ActionExecutingContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                    {"controller", MVC.Person.Name},
                    {"action", MVC.Person.ActionNames.Index}
                });
        }

        private Guid GetAssessmentId(ActionExecutingContext filterContext)
        {
            var id = (Guid)filterContext.ActionParameters[_actionParameterId];

            if (id == null)
                throw new AssessmentIdInvalidException();

            return id;
        }

        #endregion
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AssessmentCompleteAttribute : Attribute
    {
        public string ActionParameterId { get; set; }
    }
}