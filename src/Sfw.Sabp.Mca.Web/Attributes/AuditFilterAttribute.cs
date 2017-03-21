using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Infrastructure.Providers;
using Sfw.Sabp.Mca.Service.CommandHandlers;
using Sfw.Sabp.Mca.Web.Builders;
using Sfw.Sabp.Mca.Web.ViewModels;
using System;
using System.Web.Mvc;

namespace Sfw.Sabp.Mca.Web.Attributes
{
    public class AuditFilterAttribute : ActionFilterAttribute
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditLogModelBuilder _auditLogModelBuilder;
        private readonly IUserPrincipalProvider _userPrincipalProvider;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IAuditFormatter _auditFormatter;
        private readonly string _auditProperties;

        public AuditFilterAttribute(string auditProperties, ICommandDispatcher commandDispatcher, IUnitOfWork unitOfWork, IAuditLogModelBuilder auditLogModelBuilder, IUserPrincipalProvider userPrincipalProvider, IDateTimeProvider dateTimeProvider, IAuditFormatter auditFormatter)
        {
            if (string.IsNullOrWhiteSpace(auditProperties)) throw new ArgumentNullException();

            _commandDispatcher = commandDispatcher;
            _unitOfWork = unitOfWork;
            _auditLogModelBuilder = auditLogModelBuilder;
            _userPrincipalProvider = userPrincipalProvider;
            _dateTimeProvider = dateTimeProvider;
            _auditFormatter = auditFormatter;
            _auditProperties = auditProperties;
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (RequestIsValid(filterContext))
            {
                var auditedData = _auditFormatter.AuditValues(filterContext.Controller.ValueProvider, _auditProperties);

                var auditModel = GetAuditLogModel(filterContext, auditedData);

                var command = _auditLogModelBuilder.BuildAddAuditLogCommand(auditModel);

                _commandDispatcher.Dispatch(command);

                _unitOfWork.SaveChanges();
            }
        }
     
        #region private

        private bool RequestIsValid(ActionExecutedContext filterContext)
        {
            return filterContext.Controller.ViewData.ModelState.IsValid;
        }

        private AuditLogModel GetAuditLogModel(ActionExecutedContext filterContext, string auditedData)
        {
            var auditVm = new AuditLogModel()
            {
                Action = filterContext.ActionDescriptor.ActionName,
                Controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                EventDateTime = _dateTimeProvider.Now,
                AuditData = auditedData,
                User = _userPrincipalProvider.CurrentUserName
            };

            return auditVm;
        }

        #endregion
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AuditAttribute : Attribute
    {
        public string AuditProperties { get; set; }
    }

}

