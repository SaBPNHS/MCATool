using Sfw.Sabp.Mca.Core.Contracts;

namespace Sfw.Sabp.Mca.Service.QueryHandlers
{
    public interface IQueryDispatcher
    {
        TResult Dispatch<TParameter, TResult>(TParameter query)
            where TParameter : IQuery
            where TResult : IQueryResult;
    }
}