using Sfw.Sabp.Mca.Core.Contracts;

namespace Sfw.Sabp.Mca.Service.QueryHandlers
{
    public interface IQueryHandler<in TParameter, out TResult> 
        where TParameter : IQuery 
        where TResult : IQueryResult
    {
        TResult Retrieve(TParameter query);
    }
}