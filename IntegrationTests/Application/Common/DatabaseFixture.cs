
using Application.Handlers.Gaming.Join;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure;
using Infrastructure.Persistance.Repos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace IntegrationTests.Application.Common
{
    public class DatabaseFixture : IDisposable
    {
        private readonly IConfigurationRoot _configuration;
        private readonly IServiceScopeFactory _scopeFactory;

        public DatabaseFixture()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            _configuration = builder.Build();

            var services = new ServiceCollection();

            // Register DbContext using in-memory database
            services.AddSingleton<IConfiguration>(_configuration);
            var dbName = Guid.NewGuid().ToString();
            services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(dbName));

            // Register repository and mediator
            services.AddSingleton(typeof(IRepository<Game>), typeof(SqlRepository<Game>));
            services.AddMediatR(cof => cof.RegisterServicesFromAssemblyContaining(typeof(JoinGameHandler)));

            _scopeFactory = services.BuildServiceProvider().GetService<IServiceScopeFactory>();
        }

        public IMediator Mediator
        {
            get
            {
                var scope = _scopeFactory.CreateScope();
                return scope.ServiceProvider.GetService<IMediator>();
            }
        }

        public IRepository<Game> GameRepository
        {
            get
            {
                var scope = _scopeFactory.CreateScope();
                return scope.ServiceProvider.GetService<IRepository<Game>>();
            }
        }

        public void Dispose()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
            context.Database.EnsureDeleted();

        }
    }

}
