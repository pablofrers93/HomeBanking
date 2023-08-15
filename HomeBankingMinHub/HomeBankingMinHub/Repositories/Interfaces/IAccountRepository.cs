using HomeBankingMinHub.Models.Entities;
using System.Collections.Generic;

namespace HomeBankingMinHub.Repositories.Interfaces
{
    public interface IAccountRepository
    {
        IEnumerable<Account> GetAllAccounts();
        void Save(Account account);
        Account FindById(long id);
        IEnumerable<Account> GetAccountsByClient(long clientId);
        Account GetLastAccountRegistered();
        Account FindByNumber(string number);
    }
}
