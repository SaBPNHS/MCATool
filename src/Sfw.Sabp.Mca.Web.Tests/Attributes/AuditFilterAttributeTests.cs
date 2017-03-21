using System;
using System.Collections.Specialized;
using System.Web.Mvc;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sfw.Sabp.Mca.Infrastructure.Providers;
using Sfw.Sabp.Mca.Web.Attributes;
using FakeItEasy;
using Sfw.Sabp.Mca.Service.CommandHandlers;
using Sfw.Sabp.Mca.DataAccess;
using Sfw.Sabp.Mca.Web.Builders;
using Sfw.Sabp.Mca.Web.ViewModels;
using Sfw.Sabp.Mca.Service.Commands;

namespace Sfw.Sabp.Mca.Web.Tests.Attributes
{
    [TestClass]
    public class AuditFilterAttributeTests
    {
        private ICommandDispatcher _commandDispatcher;
        private IUnitOfWork _unitOfWork;
        private IAuditLogModelBuilder _auditLogModelBuilder;
        private IDateTimeProvider _dateTimeProvider;
        private IUserPrincipalProvider _userPrincipalProvider;
        private IAuditFormatter _auditFormatter;
        private ActionExecutedContext _actionExecutingContext;
        private NameValueCollection _nameValueCollection;
        private NameValueCollectionValueProvider _nameValueProviderCollection;
        private const string Controller = "controller";
        private const string Action = "action";
        private readonly DateTime _date = new DateTime(2015, 1, 1);
        private ControllerBase _controllerBase;
        private const string User = "user";

        [TestInitialize]
        public void Initialise()
        {
            _auditLogModelBuilder = A.Fake<IAuditLogModelBuilder>();
            _dateTimeProvider = A.Fake<IDateTimeProvider>();
            _userPrincipalProvider = A.Fake<IUserPrincipalProvider>();
            _commandDispatcher = A.Fake<ICommandDispatcher>();
            _unitOfWork = A.Fake<IUnitOfWork>();
            _auditFormatter = A.Fake<IAuditFormatter>();

            A.CallTo(() => _dateTimeProvider.Now).Returns(_date);
            A.CallTo(() => _userPrincipalProvider.CurrentUserName).Returns(User);
            A.CallTo(() => _auditFormatter.AuditValues(A<IValueProvider>._, A<string>._)).Returns("default");

            var actionDescriptor = A.Fake<ActionDescriptor>();
            _controllerBase = A.Fake<ControllerBase>();

            A.CallTo(() => actionDescriptor.ActionName).Returns(Action);
            A.CallTo(() => actionDescriptor.ControllerDescriptor.ControllerName).Returns(Controller);

            _nameValueCollection = new NameValueCollection();
            _nameValueProviderCollection = new NameValueCollectionValueProvider(_nameValueCollection, null);
            _controllerBase.ValueProvider = _nameValueProviderCollection;

            _actionExecutingContext = new ActionExecutedContext()
            {
                ActionDescriptor = actionDescriptor,
                Controller = _controllerBase
            };
        }

        [TestMethod]
        public void AuditFilterAttribute_GivenNullAuditProperties_ArgumentNullExceptionExpected()
        {
            Action attribute = () => AuditFilterAttribute(null);

            attribute.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void AuditFilterAttribute_GivenEmptyAuditProperties_ArgumentNullExceptionExpected()
        {
            Action attribute = () => AuditFilterAttribute("");

            attribute.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void AuditFilterAttribute_GivenWhiteSpaceAuditProperties_ArgumentNullExceptionExpected()
        {
            Action attribute = () => AuditFilterAttribute(" ");

            attribute.ShouldThrow<ArgumentNullException>();
        }

        [TestMethod]
        public void AuditFilterAttribute_GivenAuditValues_CommandDispatcherShouldBeCalledWithValues()
        {
            const string auditProperties = "auditMe";
            
            var attribute = AuditFilterAttribute(auditProperties);

            var auditLogId = Guid.NewGuid();

            var addAuditLogCommand = new AddAuditLogCommand()
            {
                Controller = Controller,
                Action = Action,
                AuditLogId = auditLogId,
                EventDateTime = _date,
                AuditData = "auditValues",
                User = User
            };

            A.CallTo(() => _auditLogModelBuilder.BuildAddAuditLogCommand(A<AuditLogModel>._)).Returns(addAuditLogCommand);
    
            attribute.OnActionExecuted(_actionExecutingContext);

            A.CallTo(() => _commandDispatcher.Dispatch(addAuditLogCommand)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void AuditFilterAttribute_GivenAuditValues_BuildAddAuditCommandShouldBeCalled()
        {
            const string auditProperties = "auditMe";
            const string auditData = "auditMeValue";

            A.CallTo(() => _auditFormatter.AuditValues(A<IValueProvider>._, auditProperties)).Returns(auditData);

            var attribute = AuditFilterAttribute(auditProperties);

            attribute.OnActionExecuted(_actionExecutingContext);

            A.CallTo(() => _auditLogModelBuilder.BuildAddAuditLogCommand(A<AuditLogModel>.That.Matches(x => x.Action == Action
                && x.Controller == Controller
                && x.EventDateTime == _date
                && x.AuditData == auditData
                && x.User == User))).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void AuditFilterAttribute_GivenAuditValues_ChangesShouldBeCommitted()
        {
            var attribute = AuditFilterAttribute("auditMe");

            attribute.OnActionExecuted(_actionExecutingContext);

            A.CallTo(() => _unitOfWork.SaveChanges()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void OnActionExecuted_GivenModelStateIsInvalid_ChangesShouldNotBeCommitted()
        {
            _controllerBase.ViewData.ModelState.AddModelError("error", "error");

            var attribute = AuditFilterAttribute("auditMe");

            attribute.OnActionExecuted(_actionExecutingContext);

            A.CallTo(() => _unitOfWork.SaveChanges()).MustNotHaveHappened();
        }

        #region private

        private AuditFilterAttribute AuditFilterAttribute(string auditProperties)
        {
            return new AuditFilterAttribute(auditProperties, _commandDispatcher, _unitOfWork, _auditLogModelBuilder, _userPrincipalProvider, _dateTimeProvider, _auditFormatter);
        }

        #endregion
    }
}
