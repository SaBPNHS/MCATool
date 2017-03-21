using System;
using System.Data.Entity;

namespace Sfw.Sabp.Mca.DataAccess
{
    public interface IUnitOfWork : IDisposable
    {
        DbContext Context { get; }
        void SaveChanges();
    }
}
