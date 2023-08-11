﻿using HomeBankingMinHub.Controllers;
using HomeBankingMinHub.Models.Entities;
using HomeBankingMinHub.Models.Enum;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using System;
using System.Linq;
using System.Net.Mime;

namespace HomeBankingMinHub.Models
{
    public class DbInitializer
    {
        public static void Initialize(HomeBankingContext context)
        { 
            if (!context.Clients.Any())
            {
                var clients = new Client[]
                {
                    new Client
                    {
                        Email = "pablofrers93@gmail.com",
                        FirstName = "Pablo",
                        LastName = "Frers",
                        Password = "123456"
                    },
                    new Client
                    {
                        Email = "Roberto@gmail.com",
                        FirstName = "Roberto",
                        LastName = "Gomez",
                        Password = "987654"
                    }
                };
                foreach (var client in clients)
                {
                    context.Clients.Add(client);    
                }
                context.SaveChanges();
            }
            if (!context.Accounts.Any())
            {
                var clientes = context.Clients.ToList();
                int numberAccount = 0;
                foreach (Client client in clientes)
                {
                    Random rnd = new Random();                   
                    var accountClient = context.Clients.FirstOrDefault(c => c.Email == client.Email);
                    if (accountClient != null)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            Account account = new Account
                            {
                                ClientId = accountClient.Id,
                                CreationDate = DateTime.Now,
                                Number = "VIN-" + new Random().Next(10000, 99999).ToString(),
                                Balance = rnd.Next(1000, 50000)
                            };
                            context.Accounts.Add(account);
                            numberAccount++;
                        }
                    }
                    
                }           
                context.SaveChanges();
            }
            if (!context.Transactions.Any())
            {
                var account1 = context.Accounts.FirstOrDefault(c => c.Number == "VIN0");
                if (account1 != null)
                {
                    var transactions = new Transaction[]
                    {
                        new Transaction { AccountId= account1.Id, Amount = 10000, Date= DateTime.Now.AddHours(-5), Description = "Acreditación sueldo Agosto", Type = TransactionType.CREDIT.ToString() },
                        new Transaction { AccountId= account1.Id, Amount = -2000, Date= DateTime.Now.AddHours(-6), Description = "Debito cajero automatico Banco Provincia", Type = TransactionType.DEBIT.ToString() },
                        new Transaction { AccountId= account1.Id, Amount = -3000, Date= DateTime.Now.AddHours(-7), Description = "Debito cajero automático Banco Nación", Type = TransactionType.DEBIT.ToString() },
                    };
                    foreach (Transaction transaction in transactions)
                    {
                        context.Transactions.Add(transaction);
                    }
                    context.SaveChanges();
                }
            }
            if (!context.Loans.Any())   
            {
                //crearemos 3 prestamos Hipotecario, Personal y Automotriz
                var loans = new Loan[]
                {
                    new Loan { Name = "Hipotecario", MaxAmount = 500000, Payments = "12,24,36,48,60" },
                    new Loan { Name = "Personal", MaxAmount = 100000, Payments = "6,12,24" },
                    new Loan { Name = "Automotriz", MaxAmount = 300000, Payments = "6,12,24,36" },
                };

                foreach (Loan loan in loans)
                {
                    context.Loans.Add(loan);
                }

                context.SaveChanges();

                //ahora agregaremos los clientloan (Prestamos del cliente)
                //usaremos al único cliente que tenemos y le agregaremos un préstamo de cada item
                var client1 = context.Clients.FirstOrDefault(c => c.Email == "pablofrers93@gmail.com");
                if (client1 != null)
                {
                    //ahora usaremos los 3 tipos de prestamos
                    var loan1 = context.Loans.FirstOrDefault(l => l.Name == "Hipotecario");
                    if (loan1 != null)
                    {
                        var clientLoan1 = new ClientLoan
                        {
                            Amount = 400000,
                            ClientId = client1.Id,
                            LoanId = loan1.Id,
                            Payments = "60"
                        };
                        context.ClientLoans.Add(clientLoan1);
                    }

                    var loan2 = context.Loans.FirstOrDefault(l => l.Name == "Personal");
                    if (loan2 != null)
                    {
                        var clientLoan2 = new ClientLoan
                        {
                            Amount = 50000,
                            ClientId = client1.Id,
                            LoanId = loan2.Id,
                            Payments = "12"
                        };
                        context.ClientLoans.Add(clientLoan2);
                    }
                    
                    var loan3 = context.Loans.FirstOrDefault(l => l.Name == "Automotriz");
                    if (loan3 != null)
                    {
                        var clientLoan3 = new ClientLoan
                        {
                            Amount = 100000,
                            ClientId = client1.Id,
                            LoanId = loan3.Id,
                            Payments = "24"
                        };
                        context.ClientLoans.Add(clientLoan3);
                    }
                    //guardamos todos los prestamos
                    context.SaveChanges();
                }
            }
            if (!context.Cards.Any())
            {
                //buscamos al unico cliente
                var client1 = context.Clients.FirstOrDefault(c => c.Email == "pablofrers93@gmail.com");
                if (client1 != null)
                {
                    //le agregamos 2 tarjetas de crédito una GOLD y una TITANIUM, de tipo DEBITO Y CREDITO RESPECTIVAMENTE
                    var cards = new Card[]
                    {
                        new Card {
                            ClientId= client1.Id,
                            CardHolder = client1.FirstName + " " + client1.LastName,
                            Type = CardType.DEBIT.ToString(),
                            Color = CardColor.GOLD.ToString(),
                            Number = "3325-6745-7876-4445",
                            Cvv = 990,
                            FromDate= DateTime.Now,
                            ThruDate= DateTime.Now.AddYears(4),
                        },
                        new Card {
                            ClientId= client1.Id,
                            CardHolder = client1.FirstName + " " + client1.LastName,
                            Type = CardType.CREDIT.ToString(),
                            Color = CardColor.TITANIUM.ToString(),
                            Number = "2234-6745-552-7888",
                            Cvv = 750,
                            FromDate= DateTime.Now,
                            ThruDate= DateTime.Now.AddYears(5),
                        },
                    };

                    foreach (Card card in cards)
                    {
                        context.Cards.Add(card);
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}
