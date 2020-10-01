using Contracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository
{
    public class AccountRepository : RepositoryBase<Account>, IAccountRepository
    {
        public AccountRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }

        public IEnumerable<Account> AccountsByOwner(Guid ownerId)
        {
            return FindByCondition(a => a.OwnerId == ownerId).ToList();
        }

        public Account GetAccountById(Guid id)
        {
            return FindByCondition(a => a.Id.Equals(id)).FirstOrDefault();
        }

        public IEnumerable<Account> GetAllAccounts()
        {
            return FindAll();
        }

        public void CreateAccount(Account account)
        {
            Create(account);
        }

        public void UpdateAccount(Account account)
        {
            Update(account);
        }

        public void DeleteAccount(Account account)
        {
            Delete(account);
        }
    }
}
