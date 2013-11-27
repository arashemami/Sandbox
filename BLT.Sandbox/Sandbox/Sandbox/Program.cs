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
        static void Main(string[] args)
        {
            Initialize().Wait();
            //RunTest().Wait();

            while (true)
                Console.Read();
        }

        //private static async System.Threading.Tasks.Task RunTest()
        //{
        //    var dataContext = CreateContext();

        //    var userId = Guid.NewGuid();
        //    var categoryId = Guid.NewGuid();

        //    var userPostsByCategory = await dataContext
        //        .Users
        //        //.Where(o => userId == o.UserId)
        //        .SelectMany(o => o.Campaigns)
        //        .SelectMany(o => o.Projects)
        //        .Where(o => categoryId == o.CategoryId)
        //        .Select(o => o.Name)
        //        .AsNoTracking()
        //        .ToListAsync();

        //    var newUser = new User
        //    {
        //        UserId = Guid.NewGuid(),
        //    };
        //    dataContext.Users.Add(newUser);
        //    await dataContext.SaveChangesAsync();

            

        //    var users = await dataContext.Users.Select(o => o.Id).ToListAsync();
        //    foreach (var user in users)
        //        Console.WriteLine(user);
        //}

        static async Task Initialize()
        {
            using (var context = CreateContext())
            {
                context.Database.Delete();
            }

            await CreateGroups();
            await OutputGroups();
            await AddCampaignsToGroups();
            await OutputCampaigns();
            await AddProjectsToCampaigns();
            await OutputProjects();

            await CreateUser();
            await OutputGroupsForUser();
            await OutputCampaignsForUser();

            //await DeleteGroup();

            //await OutputGroups();
            //await OutputCampaigns();
            //await OutputProjects();
        }

        static async Task CreateGroups()
        {
            using (var context = CreateContext())
            {
                context.Groups.AddRange(new[] {
                    new Group
                    {
                        Id = Guid.Parse("1d5067434147480ab826efb7e11939a8"),
                        Name = "CBS",
                    },
                    new Group
                    {
                        Id = Guid.Parse("b5948857f8b44a529a375cff56788797"),
                        Name = "Marvel",
                    },
                    new Group
                    {
                        Id = Guid.Parse("d3a8298754cb462fbc0096285cca2623"),
                        Name = "20th Century Fox",
                    },
                });

                await context.SaveChangesAsync();
            }
        }

        static async Task OutputGroups()
        {
            using (var context = CreateContext())
            {
                Console.WriteLine("GroupS:");
                Console.WriteLine("***********");

                var Groups = await context.Groups.ToListAsync();

                foreach (var Group in Groups.Select(o => o.Name))
                    Console.WriteLine(Group);

                Console.WriteLine();
            }
        }

        static async Task AddCampaignsToGroups()
        {
            using (var context = CreateContext())
            {
                var marvel = await context.Groups.SingleAsync(o => o.Name.Equals("marvel", StringComparison.OrdinalIgnoreCase));

                context.Campaigns.AddRange(new[]
                {
                    new Campaign
                    {
                        Id = Guid.Parse("e17ee3df2a19402784e4568edcfab8e3"),
                        Group = marvel,
                        Name = "Silver Surfer movie",
                    },
                    new Campaign
                    {
                        Id = Guid.Parse("5d893561243149a08b12d33b13334d7e"),
                        Group = marvel,
                        Name = "Avengers 3",
                    },
                    new Campaign
                    {
                        Id = Guid.Parse("208c26eb8fdd44779647445b5e0c0611"),
                        Group = marvel,
                        Name = "Dr. Strange movie",
                    },
                });

                await context.SaveChangesAsync();
            }

            using (var context = CreateContext())
            {
                var cbs = await context.Groups.SingleAsync(o => o.Name.Equals("cbs", StringComparison.OrdinalIgnoreCase));

                cbs.Campaigns.Add(
                    new Campaign
                    {
                        Id = Guid.Parse("601c080013bf45919abdab3a29c3ec1b"),
                        Group = cbs,
                        Name = "How I Met Your Mother",
                    });

                await context.SaveChangesAsync();
            }

            Group fox;

            using (var context = CreateContext())
            {
                fox = await context.Groups.SingleAsync(o => o.Name.Contains("fox"));

                fox.Add(
                    new Campaign
                    {
                        Id = Guid.Parse("f3ee12fb592b45c0a593f09a9c7f1230"),
                        Name = "The Simpsons",
                    });

                await context.SaveChangesAsync();
            }

            using (var context = CreateContext())
            {
                context.Campaigns.Add(
                    new Campaign
                    {
                        Id = Guid.Parse("a9f7bd5b4599483194ccaf5ba50a163d"),
                        GroupId = fox.Id,
                        Name = "Family Guy",
                    });

                await context.SaveChangesAsync();
            }
        }

        static async Task OutputCampaigns()
        {
            using (var context = CreateContext())
            {
                var campaigns = await context
                    .Campaigns
                    .Include(o => o.Group)  // tells the query to *eager* load Group
                    .Select(o => new { GroupName = o.Group.Name, CampaignName = o.Name })
                    .OrderBy(o => o.GroupName)
                    .ThenBy(o => o.CampaignName)
                    .ToListAsync();

                Console.WriteLine("CAMPAIGNS:");
                Console.WriteLine("***********");

                foreach (var campaign in campaigns)
                    Console.WriteLine("{0} - {1}", campaign.GroupName, campaign.CampaignName);

                Console.WriteLine();
            }
        }

        static async Task AddProjectsToCampaigns()
        {
            using (var context = CreateContext())
            {
                var avengers = await context.Campaigns.SingleAsync(o => o.Name.Contains("avengers"));

                avengers.Add(
                    new Project
                    {
                        Id = Guid.Parse("e34256bc5509453d9e969bee2ed9b439"), 
                        Name = "Avengers Blog #1"
                    });

                avengers.Add(
                    new Project
                    {
                        Id = Guid.Parse("feddea4607084a2d8fa28a3fe3800678"),
                        Name = "Avengers Blog #2"
                    });

                await context.SaveChangesAsync();
            }

            using (var context = CreateContext())
            {
                var simpsons = await context.Campaigns.SingleAsync(o => o.Name.Contains("simpsons"));

                simpsons.Add(
                    new Project
                    {
                        Id = Guid.Parse("c5fd3e6593584593bd49a88e78ffb4cf"),
                        Name = "Simpsons Fox.com site"
                    });

                simpsons.Add(
                    new Project
                    {
                        Id = Guid.Parse("5281c1fe5a6040d19d56c986dbaaa529"),
                        Name = "Halloween website"
                    });

                await context.SaveChangesAsync();
            }

            using (var context = CreateContext())
            {
                var campaigns = await context.Campaigns.ToListAsync();

                foreach (var campaign in campaigns)
                {
                    campaign.Add(
                        new Project
                        {
                            Id = Guid.NewGuid(),
                            Name = "Banners"
                        });
                    campaign.Add(
                        new Project
                        {
                            Id = Guid.NewGuid(),
                            Name = "Website"
                        });
                }

                await context.SaveChangesAsync();
            }
        }

        static async Task OutputProjects()
        {
            using (var context = CreateContext())
            {
                var projects = await context
                    .Projects
                    .Include(o => o.Group)  // tells the query to *eager* load Group
                    .Include(o => o.Campaign)
                    .Select(o => new { GroupName = o.Group.Name, CampaignName = o.Campaign.Name, ProjectName = o.Name })
                    .OrderBy(o => o.GroupName)
                    .ThenBy(o => o.CampaignName)
                    .ThenBy(o => o.ProjectName)
                    .ToListAsync();

                Console.WriteLine("PROJECTS:");
                Console.WriteLine("***********");

                foreach (var o in projects)
                    Console.WriteLine("{0} - {1} - {2}", o.GroupName, o.CampaignName, o.ProjectName);

                Console.WriteLine();
            }
        }

        static async Task DeleteGroup()
        {
            using (var context = CreateContext())
            {
                var marvel = await context.Groups.SingleAsync(o => o.Name.Contains("marvel"));

                Console.WriteLine("******** DELETING MARVEL ********");

                context.Groups.Remove(marvel);
                await context.SaveChangesAsync();

                Console.WriteLine();
            }
        }

        static async Task CreateUser()
        {
            using (var context = CreateContext())
            {
                var user = new User
                {
                    Id = Guid.Parse("9063bb54f4444eeeb719ae2e4d9edfd0"),
                    GroupId = Guid.Parse("b5948857f8b44a529a375cff56788797")
                };

                var projects = await context.Projects.ToListAsync();

                foreach (var project in projects)
                {
                    user.AddAccessTo(project);
                }

                context.Users.Add(user);
                await context.SaveChangesAsync();
            }
        }

        static async Task OutputGroupsForUser()
        {
            using (var context = CreateContext())
            {
                var groupsForUser = await context
                    .Users
                    .WithId(Guid.Parse("9063bb54f4444eeeb719ae2e4d9edfd0"))
                    .GetAccessibleGroups()
                    .Select(o => o.Name)
                    .Distinct()
                    .ToListAsync();

                Console.WriteLine("GROUPS FOR USER:");
                Console.WriteLine("***********");

                foreach (var group in groupsForUser)
                    Console.WriteLine("{0}", group);

                Console.WriteLine();
            }
        }

        static async Task OutputCampaignsForUser()
        {
            using (var context = CreateContext())
            {
                var campaignsForUser = await context
                    .Users
                    .WithId(Guid.Parse("9063bb54f4444eeeb719ae2e4d9edfd0"))
                    .SelectMany(o => o.AccessibleProjects)
                    .Select(o => o.Project.Campaign)
                    .Select(o => o.Name)
                    .Distinct()
                    .ToListAsync();

                Console.WriteLine("CAMPAIGNS FOR USER:");
                Console.WriteLine("***********");

                foreach (var campaign in campaignsForUser)
                    Console.WriteLine("{0}", campaign);

                Console.WriteLine();
            }
        }


        static DataContext CreateContext()
        {
            return new DataContext();
        }
    }
}
//  24803455