using HomeBankingMinHub.Models;
using HomeBankingMinHub.Models.Entities;
using HomeBankingMinHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace HomeBankingMinHub.Repositories
{
    public class AccountRepository : RepositoryBase<Account>, IAccountRepository
    {
        public AccountRepository(HomeBankingContext repositoryContext) : base(repositoryContext)
        {
        }
        public Account FindById(long id)
        {
            return FindByCondition(account => account.Id == id)
                .Include(account => account.Transactions)
                .FirstOrDefault();
        }
        public IEnumerable<Account> GetAccountsByClient(long clientId)
        {
            return FindByCondition(account => account.ClientId == clientId)
           .Include(account => account.Transactions)
           .ToList();
        }
        public IEnumerable<Account> GetAllAccounts()
        {
            return FindAll()
                .Include(account => account.Transactions)
                .ToList();
        }
        public void Save(Account account)
        {
            if (account.Id == 0)
            {
                Create(account);
            }
            else
            {
                Update(account);
            }
            SaveChanges();
        }
        public Account GetLastAccountRegistered()
        {
            return RepositoryContext.Accounts.OrderByDescending(account => account.Number).FirstOrDefault();
        }
        public Account FindByNumber(string number)
        {
            return FindByCondition(account => account.Number.ToUpper() == number.ToUpper())
            .Include(account => account.Transactions)
            .FirstOrDefault();
        }
    }
}
