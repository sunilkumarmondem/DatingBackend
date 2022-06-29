using DatingBackEnd.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace DatingBackEnd.Data
{
    public class DataSeeder
    {
       
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<DataContext>();
                if (!context.Users.Any())
                {
                    // var userData =  System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
                    string fileName = "Data/UserSeedData.json";
                    string jsonString = File.ReadAllText(fileName);
                    var users = JsonSerializer.Deserialize<List<AppUser>>(jsonString);
                    foreach (var user in users)
                    {
                        using var hmac = new HMACSHA512();
                        user.UserName = user.UserName.ToLower();
                        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Password"));
                        user.PasswordSalt = hmac.Key;

                        context.Users.Add(user);
                    }
                    context.SaveChanges();
                }

            }

        }
    }
}
