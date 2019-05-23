using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EFiscal.JWT.AuthServer.Common.Data;
using EFiscal.JWT.AuthServer.Common.Security;
using EFiscal.JWT.AuthServer.Data;
using EFiscal.JWT.AuthServer.Models.Security;
using EFiscal.JWT.AuthServer.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EFiscal.JWT.AuthServer
{
    public class Program
    {
        static bool isSeed = false;
        static bool isTest = false;
        static bool isExit = false;

        public static void Main(string[] args)
        {
            Console.Title = "EFiscal.JWT.AuthServer";

            isSeed = args.Any(x => x == "/seed");
            isTest = args.Any(x => x == "/test");
            isExit = args.Any(x => x == "/exit");

            var host = BuildWebHost(args);

            isSeed = true;
            isTest = true;
            isExit = true;

            if (isSeed)
            {
                RunSeed(host);
            }
            if (isTest)
            {
                RunTest(host);
            }
            if (isExit)
            {
                return;
            }

            host.Run();

        }

        //https://alunablog.com/2018/03/16/csharp-retrieve-local-public-ip-address/
        public static IWebHost BuildWebHost(string[] args)
        {
            // Build a IP Adress list for host
            #region IPAdress : urlsList
            var urls = new List<string>
            {
                "http://localhost:5000", "https://localhost:5001"
            };

            var server = Dns.GetHostName();
            IPHostEntry heserver = Dns.GetHostEntry(server);
            foreach (IPAddress curAdd in heserver.AddressList)
            {
                if (curAdd.ToString().StartsWith("10.10."))
                {
                    urls.Add(string.Format("http://{0}:5000", curAdd.ToString()));
                    urls.Add(string.Format("https://{0}:5001", curAdd.ToString()));

                    Console.WriteLine("\n\t[ Add Local Host IPAddress: " + curAdd.ToString() + " ]\n");
                }
            }
            #endregion

            return WebHost.CreateDefaultBuilder(args)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseUrls(urls.ToArray())
                .UseKestrel()
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();
        }

        #region SEED AND TEST
        private static void RunSeed(IWebHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetService<SecurityDbContext>();
                var passwordHasher = services.GetService<IPasswordHasher>();

                DatabaseSeed.Seed(context, passwordHasher);

                Console.WriteLine("SEED OK... ! ( Presione enter para continuar )");
                Console.ReadKey();
                Console.Clear();
            }
        }

        private static void RunTest(IWebHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                dynamic r1;

                var services = scope.ServiceProvider;
                var context = services.GetService<SecurityDbContext>();
                var passwordHasher = services.GetService<IPasswordHasher>();
                var userService = services.GetService<UserService>();

                Console.WriteLine("Testing UserService.CreateUserAsync...");

                r1 = userService.CreateUserAsync(
                    new User { Name = "usuario_4", Email = "usuario_4@algo.com", Password = passwordHasher.HashPassword("12345678") }
                    ).Result;
                //Console.WriteLine( JsonConvert.SerializeObject(r1).ToString() );

                Console.WriteLine("Testing UserService.ChangeUserNameAsync...");
                r1 = userService.ChangeUserNameAsync("usuario_4", "usuario_4x").Result;

                Console.WriteLine("Testing UserService.ChangeUserNameAsync...");
                r1 = userService.ChangeUserEmailAsync("usuario_4x", "nuevo.email@mail.com.mx").Result;

                Console.WriteLine("Testing UserService.ChangeUserPasswordAsync...");
                r1 = userService.ChangeUserPasswordAsync("usuario_4x", "21051971").Result;
                Console.WriteLine("Testing UserService.ValidatePassword...");
                r1 = userService.ValidatePassword("usuario_4x", "21051971");

                Console.WriteLine("Testing UserService.AddRolesAsync...");
                var u = userService.FindByNameAsync("usuario_4x").Result;
                r1 = userService.AddRoleAsync("usuario_4x", ERole.Fiscal).Result;
                r1 = userService.AddRoleAsync("usuario_4x", ERole.Administrator).Result;
                u = userService.FindByNameAsync("usuario_4x").Result;
                Console.WriteLine("Testing UserService.RemoveRoleAsync...");
                r1 = userService.RemoveRoleAsync("usuario_4x", ERole.Administrator).Result;
                u = userService.FindByNameAsync("usuario_4x").Result;
                Console.WriteLine("Testing UserService.FindByEmailAsync...");
                u = userService.FindByEmailAsync("nuevo.email@mail.com.mx").Result;
                if (u.NormalizedEmail != "nuevo.email@mail.com.mx")
                    throw new Exception("El email no es el mismo !");
                Console.WriteLine("Testing UserService.RemoveUserAsync...");
                r1 = userService.RemoveUserAsync("usuario_4x").Result;


                Console.WriteLine("TEST OK... ! ( Presione enter para continuar )");
                Console.ReadKey();
                Console.Clear();
            }
        }
        #endregion


    }

}

