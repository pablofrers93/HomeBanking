using HomeBankingMinHub.Models.Entities;

namespace HomeBankingMinHub.Repositories.Interfaces
{
    public interface IClientLoanRepository
    {
        void Save(ClientLoan clientLoan);
    }
}
