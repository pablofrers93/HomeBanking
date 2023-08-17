using HomeBankingMinHub.Models;
using HomeBankingMinHub.Models.Entities;
using HomeBankingMinHub.Repositories.Interfaces;

namespace HomeBankingMinHub.Repositories
{
    public class ClientLoanRepository : RepositoryBase<ClientLoan>, IClientLoanRepository
    {
        public ClientLoanRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }

        public void Save(ClientLoan clientLoan)
        {
            Create(clientLoan);
            SaveChanges();
        }
    }
}
