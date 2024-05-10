
using ETicket.Db.Domain.Entities;
using System.Threading.Tasks;

namespace ETicket.Db.Domain.Abstractions
{
    public interface IUnitOfWork
    {
        IRepository<TEntity> Repository<TEntity>() where TEntity : Entity;

        Task SaveChangesAsync();

        ITransaction BeginTransaction();
    }
}