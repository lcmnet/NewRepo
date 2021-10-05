using CreateUserAPI.Data;
using CreateUserAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace TestProject1
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {


            var mock = new Mock<ILogger<CreateUserAPI.Controllers.UsersController>>();
            ILogger<CreateUserAPI.Controllers.UsersController> logger = mock.Object;

            //or use this short equivalent 
            //logger = Mock.Of<ILogger<BlogController>>()


            //var data1 = new List<User>();
            //data1.Add(new User { Id = 1 });
            //var data = data1.AsQueryable();


            ////Define the mock type as DbSet<Location>
            //var mockSet = new Mock<DbSet<User>>();

            ////Define the mock Repository as databaseEf
            //var mockContext = new Mock<UserContext>();

            //mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(data.Provider);
            //mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(data.Expression);
            //mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(data.ElementType);
            //mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            ////Setting up the mockSet to mockContext
            //mockContext.Setup(c => c.).Returns(mockSet.Object);


            //var moviesMock = CreateDbSetMock(GetFakeListOfMovies());
            //var mockDbContext = new Mock<UserContext>();
            //mockDbContext.Setup(x => x.Users).Returns(moviesMock.Object);




            var data = GetFakeListOfMovies().AsQueryable();

            var mockSet = new Mock<DbSet<User>>();
            mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<UserContext>();
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);

            var c = new CreateUserAPI.Controllers.UsersController(mockContext.Object, logger);
            var r = c.GetUsers().Result;

            Assert.Pass();
        }

        private IEnumerable<User> GetFakeListOfMovies()
        {
            var movies = new List<User>
        {
            new User {Id = 1},
            new User {Id = 2},
            new User {Id = 3}
        };

            return movies;
        }

        private static Mock<DbSet<T>> CreateDbSetMock<T>(IEnumerable<T> elements) where T : class
        {
            var elementsAsQueryable = elements.AsQueryable();
            var dbSetMock = new Mock<DbSet<T>>();

            dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(elementsAsQueryable.Provider);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(elementsAsQueryable.Expression);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(elementsAsQueryable.ElementType);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(elementsAsQueryable.GetEnumerator());

            return dbSetMock;
        }
    }
}