using HomeBankingMinHub.Models;
using HomeBankingMinHub.Models.DTO_s;
using HomeBankingMinHub.Models.Entities;
using HomeBankingMinHub.Models.Enum;
using HomeBankingMinHub.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace HomeBankingMinHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly IClientLoanRepository _clientLoanRepository;
        private readonly ITransactionRepository _transactionRepository;

        public LoansController(IClientRepository clientRepository,
                               IAccountRepository accountRepository,
                               ILoanRepository loanRepository,
                               IClientLoanRepository clientLoanRepository,
                               ITransactionRepository transactionRepository)
        {
            _clientRepository = clientRepository;
            _accountRepository = accountRepository;
            _loanRepository = loanRepository;
            _clientLoanRepository = clientLoanRepository;
            _transactionRepository = transactionRepository;
        }
        [HttpPost]
        public IActionResult Post([FromBody] LoanApplicationDTO loanAppDto)
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
                    return BadRequest("Cliente no validado");
                }

                var loan = _loanRepository.FindById(loanAppDto.LoanId);           
                Account account = _accountRepository.FindByNumber(loanAppDto.ToAccountNumber);
                var acc = client.Accounts.Where(acc => acc.Number == account.Number).FirstOrDefault();

                IActionResult validationStatus = ValidateLoan(loanAppDto, loan, account, acc);
                if (validationStatus != null)
                {
                    return validationStatus;
                }

                account.Balance += loanAppDto.Amount;
                _accountRepository.Save(account);

                Transaction transaction = new Transaction
                {
                    Type = TransactionType.CREDIT.ToString(),
                    Amount = loanAppDto.Amount,
                    Description = loan.Name.ToString()+" loan approbed",
                    Date = DateTime.Now,
                    AccountId = account.Id
                };

                _transactionRepository.Save(transaction);

                ClientLoan clientLoan = new ClientLoan
                {
                    Amount = loanAppDto.Amount + loanAppDto.Amount * 0.2,
                    Payments = loanAppDto.Payments,
                    ClientId = client.Id,
                    LoanId = loan.Id
                };

                _clientLoanRepository.Save(clientLoan);

                return Ok(loanAppDto);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        private IActionResult ValidateLoan(LoanApplicationDTO loanAppDto, Loan loan, Account account, Account acc)
        {
            if (loan == null)
            {
                return BadRequest("Prestamo no encontrado");
            }
            if (loanAppDto.Payments == "" || loanAppDto.Amount == 0 || loanAppDto.ToAccountNumber == "" || loanAppDto.Payments == "0")
            {
                return BadRequest("Hay campos vacios, intente nuevamente");
            }

            if (loanAppDto.Amount > loan.MaxAmount)
            {
                return BadRequest("El monto excede el maximo permitido");
            }

            var paymentsList = loan.Payments.Split(',');

            if (!paymentsList.Contains(loanAppDto.Payments))
            {
                return BadRequest("Error en las cuotas solicitadas");
            }
            if (account == null)
            {
                return BadRequest("Cuenta destino inexistente");
            }
            if (acc == null)
            {
                return BadRequest("Cuenta destino no coincide con el cliente actual");
            }
            return null;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var loans = _loanRepository.GetAll();
                var loansDTO = new List<LoanDTO>();
                foreach (Loan loan in loans)
                {
                    LoanDTO loanDTO = new LoanDTO()
                    {
                        Id = loan.Id,
                        MaxAmount = loan.MaxAmount,
                        Name = loan.Name,
                        Payments = loan.Payments,
                    };
                    loansDTO.Add(loanDTO);
                }
                return Ok(loansDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
