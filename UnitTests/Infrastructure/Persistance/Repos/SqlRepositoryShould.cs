using Domain.Entities;
using Infrastructure;
using Infrastructure.Persistance.Repos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Infrastructure.Persistance.Repos
{
    public class SqlRepositoryShould : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private SqlRepository<Game> _sut;

        public SqlRepositoryShould()
        {
            var dbName = Guid.NewGuid().ToString();
            _context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
                                                .UseInMemoryDatabase(databaseName: dbName)
                                                .Options);
            _context.Database.EnsureCreated();
            SeedData(_context);
            
            _sut = new SqlRepository<Game>(_context);
        }
        
        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task Returns_Entity_When_Calling_GetByIdAsync()
        {            
            var entityId = 1;
       
            var result = await _sut.GetByIdAsync(entityId);
            
            Assert.NotNull(result);
            Assert.Equal(entityId, result.Id);
        }

        [Fact]
        public async Task Return_All_Entities_When_Calling_GetAllAsync()
        {       
            var result = await _sut.GetAllAsync();
            
            Assert.NotNull(result);
            Assert.Equal(_context.Games.Count(), result.Count());
        }

        [Fact]
        public async Task Add_New_Entity_When_Calling_AddAsync()
        {
            var newEntity = new Game { Id = 4 };

            var result = await _sut.AddAsync(newEntity);

            Assert.NotNull(result);
            Assert.Equal(newEntity.Id, result.Id);
            Assert.True(result.Id > 0);
            Assert.Contains(result, _context.Games);
        }

        [Fact]
        public async Task Update_Existing_Entity_When_Calling_UpdateAsync()
        {
            var entityId = 4;
            var updatedMisteryNumber = 25;
            var entity = new Game { Id = entityId };
            _context.Games.Add(entity);
            await _context.SaveChangesAsync();

            entity.SetMysteryNumber(updatedMisteryNumber);
            var result = await _sut.UpdateAsync(entity);

            Assert.NotNull(result);
            Assert.Equal(updatedMisteryNumber, result.MysteryNumber);
            Assert.Contains(result, _context.Games);
        }

        [Fact]
        public async Task Remove_Entity_When_Calling_RemoveAsync()
        {
            var entityId = 4;
            var entity = new Game { Id = entityId };
            _context.Games.Add(entity);
            await _context.SaveChangesAsync();

            await _sut.RemoveAsync(entity);

            Assert.DoesNotContain(entity, _context.Games);
        }

        private void SeedData(ApplicationDbContext context)
        {
            context.Games.AddRange(new List<Game>
            {
                new Game { Id = 1},
                new Game { Id = 2},
                new Game { Id = 3}
            });
            context.SaveChanges();
        }
    }
}
