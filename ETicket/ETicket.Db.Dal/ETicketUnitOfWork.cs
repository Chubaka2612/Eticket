
using ETicket.Db.Domain.Abstractions;
using ETicket.Db.Domain.Entities;
using System.Data;
using Microsoft.EntityFrameworkCore; 

namespace ETicket.Db.Dal
{
    public class ETicketUnitOfWork : IUnitOfWork
    {
        private readonly ETicketDbContext _context;

        public ETicketUnitOfWork(ETicketDbContext dbContext)
        {
            _context = dbContext;
        }

        public IRepository<TEntity> Repository<TEntity>() where TEntity : Entity
        {
            return new Repository<TEntity>(_context);
        }

        public ITransaction BeginTransaction()
        {
            return new Transaction(_context.Database.BeginTransaction(IsolationLevel.Serializable));
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}