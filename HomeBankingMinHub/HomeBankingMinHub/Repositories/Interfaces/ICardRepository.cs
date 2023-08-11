using HomeBankingMinHub.Models.Entities;
using System.Collections.Generic;

namespace HomeBankingMinHub.Repositories.Interfaces
{
    public interface ICardRepository
    {
        IEnumerable<Card> GetAllCards();
        void Save(Card card);
        Card FindById(long id);
        IEnumerable<Card> GetCardsByClient(long clientId);
        string GenerateNextCardNumber();
        string GetLastCardNumber();
    }
}
