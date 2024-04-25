
using ETicket.Db.Domain.Abstractions;
using Microsoft.EntityFrameworkCore.Storage;

namespace ETicket.Db.Dal
{
    public class Transaction : ITransaction
    {
        private readonly IDbContextTransaction _efTransaction;

        public Transaction(IDbContextTransaction efTransaction)
        {
            _efTransaction = efTransaction;
        }

        public Task CommitAsync()
        {
            return _efTransaction.CommitAsync();
        }

        public Task RollbackAsync()
        {
            return _efTransaction.RollbackAsync();
        }
    }
}