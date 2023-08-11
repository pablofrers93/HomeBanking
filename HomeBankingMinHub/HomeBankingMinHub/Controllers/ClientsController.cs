using HomeBankingMinHub.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Linq;
using HomeBankingMinHub.Models.Entities;
using HomeBankingMinHub.Repositories.Interfaces;
using System.Security.Principal;
using Microsoft.AspNetCore.Authorization;
using HomeBankingMinHub.Models.Enum;

namespace HomeBankingMinHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private IClientRepository _clientRepository;
        private AccountsController _accountsController;
        private CardsController _cardsController;
        public ClientsController(IClientRepository clientRepository,
                                 AccountsController accountsController,
                                 CardsController cardsController)
        {
            _clientRepository = clientRepository;
            _accountsController = accountsController;
            _cardsController = cardsController;
        }

        // El siguiente método devuelve un IActionResult con una lista de todos los clientes en nuestro home banking usando una lista de ClientDTO.
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var clients = _clientRepository.GetAllClients();
                var clientsDTO = new List<ClientDTO>();
                foreach (Client client in clients)
                {
                    var newClientDTO = new ClientDTO
                    {
                        Id = client.Id,
                        Email = client.Email,
                        FirstName = client.FirstName,
                        LastName = client.LastName,
                        Accounts = client.Accounts.Select(ac => new AccountDTO
                        {
                            Id = ac.Id,
                            Balance = ac.Balance,
                            CreationDate = ac.CreationDate,
                            Number = ac.Number
                        }).ToList(),
                        Credits = client.ClientLoans.Select(cl => new ClientLoanDTO
                        {
                            Id = cl.Id,
                            LoanId = cl.LoanId,
                            Name = cl.Loan.Name,
                            Amount = cl.Amount,
                            Payments = int.Parse(cl.Payments)
                        }).ToList(),
                        Cards = client.Cards.Select(c => new CardDTO
                        {
                            Id = c.Id,
                            CardHolder = c.CardHolder,
                            Color = c.Color,
                            Cvv = c.Cvv,
                            FromDate = c.FromDate,
                            Number = c.Number,
                            ThruDate = c.ThruDate,
                            Type = c.Type
                        }).ToList()
                    };
                    clientsDTO.Add(newClientDTO);
                }
                return Ok(clientsDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // El siguiente método recibe un clientId y devuelve un IActionResult con una instancia de ClientDTO.
        [HttpGet("{id}")]
        public IActionResult Get(long id)
        {
            try
            {
                var client = _clientRepository.FindById(id);
                if (client == null)
                {
                    return Forbid();
                }
                var clientDTO = new ClientDTO
                {
                    Id = client.Id,
                    Email = client.Email,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Accounts = client.Accounts.Select(ac => new AccountDTO
                    {
                        Id = ac.Id,
                        Balance = ac.Balance,
                        CreationDate = ac.CreationDate,
                        Number = ac.Number
                    }).ToList(),
                    Credits = client.ClientLoans.Select(cl => new ClientLoanDTO
                    {
                        Id = cl.Id,
                        LoanId = cl.LoanId,
                        Name = cl.Loan.Name,
                        Amount = cl.Amount,
                        Payments = int.Parse(cl.Payments)
                    }).ToList(),
                    Cards = client.Cards.Select(c => new CardDTO
                    {
                        Id = c.Id,
                        CardHolder = c.CardHolder,
                        Color = c.Color,
                        Cvv = c.Cvv,
                        FromDate = c.FromDate,
                        Number = c.Number,
                        ThruDate = c.ThruDate,
                        Type = c.Type
                    }).ToList()
                };
                return Ok(clientDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // El siguiente método devuelve un IActionResult con una instancia de ClientDTO del cliente logueado actualmente.
        [HttpGet("current")]
        public IActionResult GetCurrent()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return NotFound();
                }

                Client client = _clientRepository.FindByEmail(email);

                if (client == null)
                {
                    return Forbid();
                }

                var clientDTO = new ClientDTO
                {
                    Id = client.Id,
                    Email = client.Email,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    Accounts = client.Accounts.Select(ac => new AccountDTO
                    {
                        Id = ac.Id,
                        Balance = ac.Balance,
                        CreationDate = ac.CreationDate,
                        Number = ac.Number
                    }).ToList(),
                    Credits = client.ClientLoans.Select(cl => new ClientLoanDTO
                    {
                        Id = cl.Id,
                        LoanId = cl.LoanId,
                        Name = cl.Loan.Name,
                        Amount = cl.Amount,
                        Payments = int.Parse(cl.Payments)
                    }).ToList(),
                    Cards = client.Cards.Select(c => new CardDTO
                    {
                        Id = c.Id,
                        CardHolder = c.CardHolder,
                        Color = c.Color,
                        Cvv = c.Cvv,
                        FromDate = c.FromDate,
                        Number = c.Number,
                        ThruDate = c.ThruDate,
                        Type = c.Type
                    }).ToList()
                };

                return Ok(clientDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Método para registrar cliente y crearle su primera cuenta
        [HttpPost]
        public IActionResult Post([FromBody] Client client)
        {
            try
            {
                // Validación de datos
                if (String.IsNullOrEmpty(client.Email) ||
                    String.IsNullOrEmpty(client.Password) ||
                    String.IsNullOrEmpty(client.FirstName) ||
                    String.IsNullOrEmpty(client.LastName))
                    return StatusCode(403, "Datos inválidos");

                // Buscamos si el cliente existe
                Client user = _clientRepository.FindByEmail(client.Email);
                if (user != null)
                {
                    return StatusCode(403, "Email en uso, intente con otro");
                }

                Client newClient = new Client
                {
                    Email = client.Email,
                    Password = client.Password,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                };

                _clientRepository.Save(newClient);

                _accountsController.Post(newClient.Id);

                return Created("", newClient);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // El siguiente método realiza las validaciones solicitadas para agregar una nueva tarjeta.
        // Generada la instancia de Card se guarda con el CardsController y se obtiene newCardDTO para enviarlo al front.
        [HttpPost("current/cards")]
        public IActionResult PostCards([FromBody] Card card)
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return NotFound();
                }

                Client client = _clientRepository.FindByEmail(email);

                int typeCardCount = client.Cards.Where(c => c.Type == card.Type).Count();
                if (typeCardCount > 2)
                {
                    return StatusCode(403, "Ya tiene 3 tarjetas de credito o debito");
                }
                if (card.Color != CardColor.GOLD.ToString() && card.Color != CardColor.SILVER.ToString() && card.Type != CardColor.SILVER.ToString())
                {
                    return BadRequest("El color de tarjeta no es valido");
                }
                int sameCardCount = client.Cards.Where(c => c.Color == card.Color && c.Type == card.Type).Count();
                if (sameCardCount == 1)
                {
                    return StatusCode(403, "Ya tiene esa tarjeta del mismo tipo y color");
                }

                Card newCard = new Card
                {
                    CardHolder = client.FirstName + " " + client.LastName,
                    Type = card.Type,
                    Color = card.Color,
                    Number = new Random().Next(1000, 9999).ToString() + "-" +
                                             new Random().Next(1000, 9999).ToString() + "-" +
                                             new Random().Next(1000, 9999).ToString() + "-" +
                                             new Random().Next(1000, 9999).ToString(),
                    Cvv = new Random().Next(100, 999),
                    FromDate = DateTime.Now,
                    ThruDate = DateTime.Now.AddYears(4),
                    ClientId = client.Id
                };

                var newCardDTO = _cardsController.Post(newCard);
                if (newCard == null)
                {
                    return StatusCode(500, "Error al cargar la tarjeta");
                }

                return Created("", newCardDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // El siguiente método realiza las validaciones solicitadas para agregar una nueva cuenta al cliente logueado actualmente.
        // Se envía al AccountsController el clientId para generar y guardar la cuenta
        // y se obtiene una instancia de AccountDTO para envíar al front.
        [HttpPost("current/accounts")]
        [Authorize]
        public IActionResult PostAccounts()
        {
            try
            {
                string email = User.FindFirst("Client") != null ? User.FindFirst("Client").Value : string.Empty;
                if (email == string.Empty)
                {
                    return NotFound();
                }
                Client client = _clientRepository.FindByEmail(email);
                if (client == null)
                {
                    return NotFound();
                }
                if (client.Accounts.Count > 2)
                {
                    return StatusCode(403, "Prohibido. El cliente ya tiene 3 cuentas registradas.");
                }

                var account = _accountsController.Post(client.Id);
                if (account == null)
                {
                    return StatusCode(500, "Error al crear la cuenta");
                }
                return Created("", account);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpGet("current/accounts")]
        public IActionResult GetAccounts()
        {
            try
            {
                return GetClientProperty(client => client.Accounts);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet("current/cards")]
        public IActionResult GetCards()
        {
            try
            {
                return GetClientProperty(client => client.Cards);
            }
            catch (Exception)
            {
                throw;
            }
        }
        private IActionResult GetClientProperty(Func<Client, IEnumerable<object>> propertySelector)
        {
            string email = User.FindFirst("Client")?.Value;
            if (string.IsNullOrEmpty(email))
            {
                return NotFound();
            }

            Client client = _clientRepository.FindByEmail(email);
            if (client == null)
            {
                return NotFound();
            }

            var propertyValue = propertySelector(client) 
                ;

            return Ok(propertyValue);
        }
    }
}