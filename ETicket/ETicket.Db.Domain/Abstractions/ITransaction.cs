using System;
using System.Threading.Tasks;

namespace ETicket.Db.Domain.Abstractions
{
    public interface ITransaction : IDisposable
    {
        Task CommitAsync();

        Task RollbackAsync();
    }
}