using HomeBankingMinHub.Models;
using HomeBankingMinHub.Models.Entities;
using HomeBankingMinHub.Repositories;
using HomeBankingMinHub.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Principal;

namespace HomeBankingMinHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardsController : ControllerBase
    {
        private readonly ICardRepository _cardRepository;
        public CardsController(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        public IActionResult Post(Card newCard)
        {
            try
            {        
                _cardRepository.Save(newCard);

                CardDTO newCardDTO = new CardDTO
                {
                    CardHolder = newCard.CardHolder,
                    Type = newCard.Type,
                    Color = newCard.Color,
                    Number = newCard.Number,
                    Cvv = newCard.Cvv,
                    FromDate = newCard.FromDate,
                    ThruDate = newCard.ThruDate,
                };
                return Created("", newCardDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }          
        }
    }
}
