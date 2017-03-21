using Ninject;
using Sfw.Sabp.Mca.Service.Commands;

namespace Sfw.Sabp.Mca.Service.CommandHandlers
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IKernel _kernal;

        public CommandDispatcher(IKernel kernal)
        {
            _kernal = kernal;
        }

        public void Dispatch<TCommand>(TCommand command) where TCommand : ICommand
        {
            var handler = _kernal.Get<ICommandHandler<TCommand>>();

            handler.Execute(command);
        }
    }
}
