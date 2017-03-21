using Ninject;
using Sfw.Sabp.Mca.Core.Contracts;

namespace Sfw.Sabp.Mca.Service.QueryHandlers
{
    public class QueryDispatcher : IQueryDispatcher
    {
        private readonly IKernel _kernel;

        public QueryDispatcher(IKernel kernel)
        {
            _kernel = kernel;
        }

        public TResult Dispatch<TParameter, TResult>(TParameter query) where TParameter : IQuery where TResult : IQueryResult
        {
            var dispatcher = _kernel.Get<IQueryHandler<TParameter, TResult>>();

            return dispatcher.Retrieve(query);
        }
    }
}
