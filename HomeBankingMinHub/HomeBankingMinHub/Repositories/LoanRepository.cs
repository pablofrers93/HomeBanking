using HomeBankingMinHub.Models;
using HomeBankingMinHub.Models.Entities;
using HomeBankingMinHub.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace HomeBankingMinHub.Repositories
{
    public class LoanRepository:RepositoryBase<Loan>, ILoanRepository
    {
        public LoanRepository(HomeBankingContext repositoryContext): base(repositoryContext)
        {        
        }

        public Loan FindById(long id)
        {
            return FindByCondition(loan => loan.Id == id)
                .FirstOrDefault();
        }

        public IEnumerable<Loan> GetAll()
        {
            return FindAll()
                .ToList();  
        }
    }
}
