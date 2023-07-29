using HomeBankingMinHub.Models;
using System.Collections.Generic;

namespace HomeBankingMinHub.Repositories
{
    public interface IAccountRepository
    {
        IEnumerable<Account> GetAllAccounts();
        void Save(Account account);
        Account FindById(long id);
    }
}
