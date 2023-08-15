using HomeBankingMinHub.Models;
using HomeBankingMinHub.Models.Entities;
using HomeBankingMinHub.Repositories.Interfaces;
using System.Linq;

namespace HomeBankingMinHub.Repositories
{
    public class TransactionRepository : RepositoryBase<Transaction>, ITransactionRepository
    {
        public TransactionRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }
        public Transaction FindByNumber(long id)
        {
            return FindByCondition(transaction => transaction.Id == id).FirstOrDefault();
        }
        public void Save(Transaction transaction)
        {
            Create(transaction);
            SaveChanges();
        }
    }
}
