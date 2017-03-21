using Sfw.Sabp.Mca.Service.Commands;

namespace Sfw.Sabp.Mca.Service.CommandHandlers
{
    public interface ICommandHandler<in T> where T : ICommand
    {
        void Execute(T assessmentCommand);
    }

}
