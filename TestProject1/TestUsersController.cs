using AutoFixture;
using CreateUserAPI.Data;
using CreateUserAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject1
{
    public class TestUsersController
    {
        Mock<DbSet<User>> usersMock = null;
        Mock<UserContext> userContextMock = null;
        Mock<ILogger<CreateUserAPI.Controllers.UsersController>> logger = null;
        CreateUserAPI.Controllers.UsersController usersService = null;

        [SetUp]
        public void Setup()
        {
            var fixture = new Fixture();
            var firstUser = fixture.Build<User>().With(u => u.Name, "Lee").With(u => u.Id, 1).With(u => u.Email, "lee@mail.com").Create();
            var users = new List<User>
              {
                firstUser,
                fixture.Build<User>().With(u => u.Name, "Choong").With(u => u.Id, 2).With( u => u.Email, "choong@mail.com").Create(),
                fixture.Build<User>().With(u => u.Name, "Meng").With(u => u.Id, 3).With( u => u.Email, "meng@mail.com").Create()
              }.AsQueryable();

            usersMock = new Mock<DbSet<User>>();
            usersMock.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            usersMock.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            usersMock.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            usersMock.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());
            usersMock.As<IAsyncEnumerable<User>>().Setup(x => x.GetAsyncEnumerator(default)).Returns(new TestAsyncEnumerator<User>(users.GetEnumerator()));
            usersMock.Setup(o => o.FindAsync((long)20)).ReturnsAsync(new User { Id = 20, Name = "Alice" });

            userContextMock = new Mock<UserContext>();
            userContextMock.Setup(x => x.Users).Returns(usersMock.Object);

            logger = new Mock<ILogger<CreateUserAPI.Controllers.UsersController>>();
            usersService = new CreateUserAPI.Controllers.UsersController(userContextMock.Object, logger.Object);
        }


        [Test]
        public async Task TestGetUsers()
        {
            // Act
            var allUsers = await usersService.GetUsers();

            // Assert
            Assert.AreEqual(allUsers.Value.Count(), 3);

        }

        [Test]
        public async Task PassGetSingleUser()
        {
            // Act
            var oneUser = await usersService.GetUser(20);

            // Assert
            Assert.AreEqual(oneUser.Value.Name, "Alice");

        }

        [Test]
        public async Task FailGetSingleUser()
        {
            // Act
            var oneUser = await usersService.GetUser(20);

            // Assert
            Assert.AreNotEqual(oneUser.Value.Name, "Lee");

        }

        [Test]
        public async Task PassCreateUser()
        {
            var fixture = new Fixture();
            var firstUser = fixture.Build<User>().With(u => u.Name, "Alan").With(u => u.Id, 10).With(u => u.Email, "alan@mail.com").Create();

            // Act
            var x = await usersService.PostUser(firstUser);
            var y = (CreatedAtActionResult)x.Result;

            // Assert
            Assert.AreEqual(y.StatusCode, 201);

        }

        [Test]
        public async Task FailCreateUser()
        {
            // Act
            var x = await usersService.PostUser(user: null);
            var y = ((ObjectResult)x.Result);

            // Assert
            Assert.AreEqual(y.StatusCode, 500);

        }

        [Test]
        public async Task PassDeleteSingleUser()
        {
            // Act
            var oneUser = await usersService.DeleteUser(20);

            // Assert
            Assert.AreEqual(oneUser.Value.Name, "Alice");

        }

        [Test]
        public async Task FailDeleteSingleUser()
        {
            // Act
            var x = await usersService.DeleteUser(999);
            var y = ((NotFoundResult)x.Result);

            // Assert
            Assert.AreEqual(y.StatusCode, 404);

        }
    }
}
