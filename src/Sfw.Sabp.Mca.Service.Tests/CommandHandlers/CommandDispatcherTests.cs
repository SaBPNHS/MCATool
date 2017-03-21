using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Sfw.Sabp.Mca.Service.CommandHandlers;
using Sfw.Sabp.Mca.Service.Commands;

namespace Sfw.Sabp.Mca.Service.Tests.CommandHandlers
{
    [TestClass]
    public class CommandDispatcherTests
    {
        private CommandDispatcher _dispatcher;
        private IKernel _kernal;
        private ICommandHandler<FakeCommand> _fakeHandler;

        [TestInitialize]
        public void Setup()
        {
            _kernal = new StandardKernel();

            _fakeHandler = A.Fake<ICommandHandler<FakeCommand>>();
            _dispatcher = new CommandDispatcher(_kernal);
        }

        [TestMethod]
        public void Dispatch_GivenHandlerNotMapped_ActivationExceptionShouldBeThrown()
        {
            _dispatcher.Invoking(x => x.Dispatch(A<ICommand>._)).ShouldThrow<ActivationException>();
        }

        [TestMethod]
        public void Dispatch_GivenHandlerMappped_HandlerExecuteMethodShouldBeCalled()
        {
            _kernal.Bind<ICommandHandler<FakeCommand>>().ToConstant(_fakeHandler);

            var command = new FakeCommand();
            _dispatcher.Dispatch(command);

            A.CallTo(() => _fakeHandler.Execute(command)).MustHaveHappened(Repeated.Exactly.Once);
        }

        #region private

        public class FakeCommand : ICommand{}

        #endregion
    }
}
