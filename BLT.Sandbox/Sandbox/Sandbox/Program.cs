﻿using Sandbox.Data;
using Sandbox.Data.Entity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Sandbox
{
    class Program
    {
        static string connectionString = "test22";

        static void Main(string[] args)
        {
            Initialize().Wait();
            //RunTest().Wait();

            while (true)
                Console.Read();
        }

        private static async System.Threading.Tasks.Task RunTest()
        {
            var dataContext = new DataContext(connectionString);

            var userId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();

            var userPostsByCategory = await dataContext
                .Users
                //.Where(o => userId == o.UserId)
                .SelectMany(o => o.Projects)
                .SelectMany(o => o.Posts)
                .Where(o => categoryId == o.CategoryId)
                .Select(o => o.Name)
                .AsNoTracking()
                .ToListAsync();

            var newUser = new User
            {
                UserId = Guid.NewGuid(),
            };
            dataContext.Users.Add(newUser);
            await dataContext.SaveChangesAsync();

            

            var users = await dataContext.Users.Select(o => o.Id).ToListAsync();
            foreach (var user in users)
                Console.WriteLine(user);
        }

        static async Task Initialize()
        {
            await CreateClients();
            await VerifyClientsCreated();
            await AddProjectsToClients();
            await VerifyProjectsCreated();
        }

        static async Task CreateClients()
        {
            using (var context = new DataContext(connectionString))
            {
                context.Clients.AddRange(new[] {
                    new Client
                    {
                        ClientId = Guid.Parse("1d5067434147480ab826efb7e11939a8"),
                        Name = "CBS",
                    },
                    new Client
                    {
                        ClientId = Guid.Parse("b5948857f8b44a529a375cff56788797"),
                        Name = "Marvel",
                    },
                    new Client
                    {
                        ClientId = Guid.Parse("d3a8298754cb462fbc0096285cca2623"),
                        Name = "20th Century Fox",
                    },
                });

                await context.SaveChangesAsync();
            }
        }

        static async Task VerifyClientsCreated()
        {
            using (var context = new DataContext(connectionString))
            {
                Console.WriteLine("CLIENTS:");
                Console.WriteLine("***********");

                var clients = await context.Clients.ToListAsync();

                foreach (var client in clients.Select(o => o.Name))
                    Console.WriteLine(client);

                Console.WriteLine();
            }
        }

        static async Task AddProjectsToClients()
        {
            using (var context = new DataContext(connectionString))
            {
                var marvel = await context.Clients.SingleAsync(o => o.Name.Equals("marvel", StringComparison.OrdinalIgnoreCase));

                context.Projects.AddRange(new[]
                {
                    new Project
                    {
                        ProjectId = Guid.Parse("e17ee3df2a19402784e4568edcfab8e3"),
                        Client = marvel,
                        Name = "Silver Surfer movie",
                    },
                    new Project
                    {
                        ProjectId = Guid.Parse("5d893561243149a08b12d33b13334d7e"),
                        Client = marvel,
                        Name = "Avengers 3",
                    },
                    new Project
                    {
                        ProjectId = Guid.Parse("208c26eb8fdd44779647445b5e0c0611"),
                        Client = marvel,
                        Name = "Dr. Strange movie",
                    },
                });

                await context.SaveChangesAsync();
            }

            using (var context = new DataContext(connectionString))
            {
                var cbs = await context.Clients.SingleAsync(o => o.Name.Equals("cbs", StringComparison.OrdinalIgnoreCase));

                cbs.Projects.Add(
                    new Project
                    {
                        ProjectId = Guid.Parse("601c080013bf45919abdab3a29c3ec1b"),
                        Client = cbs,
                        Name = "How I Met Your Mother",
                    });

                await context.SaveChangesAsync();
            }

            Client fox;

            using (var context = new DataContext(connectionString))
            {
                fox = await context.Clients.SingleAsync(o => o.Name.Contains("fox"));

                fox.AddProject(
                    new Project
                    {
                        ProjectId = Guid.Parse("f3ee12fb592b45c0a593f09a9c7f1230"),
                        Name = "The Simpsons",
                    });

                await context.SaveChangesAsync();
            }

            using (var context = new DataContext(connectionString))
            {
                context.Projects.Add(
                    new Project
                    {
                        ProjectId = Guid.Parse("a9f7bd5b4599483194ccaf5ba50a163d"),
                        ClientId = fox.ClientId,
                        Name = "Family Guy",
                    });

                await context.SaveChangesAsync();
            }
        }

        static async Task VerifyProjectsCreated()
        {
            using (var context = new DataContext(connectionString))
            {
                var projects = await context
                    .Projects
                    .Include(o => o.Client)  // tells the query to *eager* load Client
                    .Select(o => new { ClientName = o.Client.Name, ProjectName = o.Name })
                    .ToListAsync();

                Console.WriteLine("PROJECTS:");
                Console.WriteLine("***********");

                foreach (var project in projects)
                    //Console.WriteLine("{0} - {1}", project.Client.Name, project.Name);
                    Console.WriteLine("{0} - {1}", project.ClientName, project.ProjectName);

                Console.WriteLine();
            }
        }
    }

    /*
     
     What I did for today:  went over the EF documentation, modified the data models
     to turn on Lazy Loading by default, and also changed them slightly to reflect
     best-practices.
     
     Disabled Migrations on the DataContext, which is enabled by default
     
     Turned on SQL logging (right now logging to the Console window but can change that
     to use the Azure Table logging Kaz created)
     
     Setup explicit Many-To-Many mapping in the DataContext (for User-Clients, User-Projects)
     
     Modified test query to use async for asynchronouse execution of the SQL satement
     
     Verified that the SQL being generated for the query is optimal (it is)
    
     * */
}
//  24803455