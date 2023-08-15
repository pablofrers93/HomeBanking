using HomeBankingMinHub.Models.Entities;

namespace HomeBankingMinHub.Repositories.Interfaces
{
    public interface ITransactionRepository
    {
        void Save(Transaction transaction);
        Transaction FindByNumber(long id);
    }
}
