using HomeBankingMinHub.Controllers;
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
                                Number = "VIN" + numberAccount,
                                Balance = rnd.Next(1000, 50000)
                            };
                            context.Accounts.Add(account);
                            numberAccount++;
                        }
                    }
                    
                }           
                context.SaveChanges();
            }
        }
    }
}
