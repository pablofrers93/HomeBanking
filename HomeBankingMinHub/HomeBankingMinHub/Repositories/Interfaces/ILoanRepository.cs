using HomeBankingMinHub.Models.Entities;
using System.Collections.Generic;

namespace HomeBankingMinHub.Repositories.Interfaces
{
    public interface ILoanRepository
    {
        IEnumerable<Loan> GetAll();
        Loan FindById(long id);
    }
}
