using System.Linq;

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
                    }
                };
                foreach (var client in clients)
                {
                    context.Clients.Add(client);    
                }
                context.SaveChanges();
            }
        }
    }
}
