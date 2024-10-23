using AccountService;
using Microsoft.AspNetCore.Mvc;
using Models;
using Moq;
using Moq.EntityFrameworkCore;

namespace TwitterSystemUnitTests;

public class AccountServiceTests
{
    [Fact]
    public void GetAccountByIDReturnsAccountOk()
    {
        // === Arrange ===
        // DB context mock
        Mock<ASDBContext> _db = new();
        // Define mock data
        var accountsMockData = new List<Account>
        {
            new()
            {
                ID = 1,
                Username = "Asbjørn",
                Password = "123456",
                Bio = "A programmer",
                Email = "asbjorn@gmail.com"
            },
            new()
            {
                ID = 2,
                Username = "Esben",
                Password = "123456",
                Bio = "A programmer",
                Email = "esben@gmail.com"
            },
            new()
            {
                ID = 3,
                Username = "AppleTree",
                Password = "123456",
                Bio = "A very delicious programmer",
                Email = "apple@gmail.com"
            }
        };

        // Setup mock return data
        _db.Setup(db => db.Accounts).ReturnsDbSet(accountsMockData);

        // Account controller with mock database context
        var ac = new AccountController(_db.Object);

        // ===== Act =====
        var dataRet = (ac.GetAccountByID(1) as OkObjectResult)!.Value as Account;

        // === Assert ====
        Assert.NotNull(dataRet);
        Assert.Equal(accountsMockData[0], dataRet);
    }

    [Fact]
    public void GetAccountByNameReturnsAccountOk()
    {
        // === Arrange ===
        // DB context mock
        Mock<ASDBContext> _db = new();
        // Define mock data
        var accountsMockData = new List<Account>
        {
            new()
            {
                ID = 1,
                Username = "Asbjørn",
                Password = "123456",
                Bio = "A programmer",
                Email = "asbjorn@gmail.com"
            },
            new()
            {
                ID = 2,
                Username = "Esben",
                Password = "123456",
                Bio = "A programmer",
                Email = "esben@gmail.com"
            },
            new()
            {
                ID = 3,
                Username = "AppleTree",
                Password = "123456",
                Bio = "A very delicious programmer",
                Email = "apple@gmail.com"
            }
        };

        // Setup mock return data
        _db.Setup(db => db.Accounts).ReturnsDbSet(accountsMockData);

        // Account controller with mock database context
        var ac = new AccountController(_db.Object);

        // ===== Act =====
        var dataRet = (ac.GetAccountByName("Esben") as OkObjectResult)!.Value as Account;

        // === Assert ====
        Assert.NotNull(dataRet);
        Assert.Equal(accountsMockData[1], dataRet);
    } 
    
    [Fact]
    public void CreateAccountRunsSuccessfully()
    {
        // === Arrange ===
        // DB context mock
        Mock<ASDBContext> _db = new();

        // Fake account data
        Account fakeAccountData = new()
        {
            ID = 3,
            Username = "AppleTree",
            Password = "123456",
            Bio = "A very delicious programmer",
            Email = "apple@gmail.com"
        };
        List<Account> fakeAccountsTable = new();

        _db.Setup(db => db.Accounts).ReturnsDbSet(fakeAccountsTable);

        // Account controller with mock database context
        var ac = new AccountController(_db.Object);

        // ===== Act =====
        var result = ac.CreateAccount(fakeAccountData);

        // === Assert ====
        Assert.IsType<OkResult>(result);
        _db.Verify(db => db.Accounts.Add(fakeAccountData), Times.Once);
        _db.Verify(db => db.SaveChanges(), Times.Once);
    }

    [Fact]
        public void UpdateAccountRunsSuccessfully()
        {
            // === Arrange ===
            // DB context mock
            Mock<ASDBContext> _db = new();
    
            // Fake account data
            Account newFakeAccountData = new()
            {
                ID = 1,
                Username = "PearTree",
                Password = "654321",
                Bio = "A another very delicious programmer",
                Email = "pear@gmail.com"
            };
            List<Account> fakeAccountsTable = new()
            {
                new Account
                {
                    ID = 3,
                    Username = "AppleTree",
                    Password = "123456",
                    Bio = "A very delicious programmer",
                    Email = "apple@gmail.com"
                }
            };
    
            _db.Setup(db => db.Accounts).ReturnsDbSet(fakeAccountsTable);
    
            // Account controller with mock database context
            var ac = new AccountController(_db.Object);
    
            // ===== Act =====
            var result = ac.UpdateAccount(newFakeAccountData);
    
            // === Assert ====
            Assert.IsType<OkResult>(result);
            _db.Verify(db => db.Accounts.Update(newFakeAccountData), Times.Once);
            _db.Verify(db => db.SaveChanges(), Times.Once);
        }

    [Fact]
    public void DeleteAccountRunsSuccessfully()
    {
        // === Arrange ===
        // DB context mock
        Mock<ASDBContext> _db = new();

        // Fake account data
        Account newFakeAccountData = new()
        {
            ID = 1,
            Username = "PearTree",
            Password = "654321",
            Bio = "A another very delicious programmer",
            Email = "pear@gmail.com"
        };
        List<Account> fakeAccountsTable = new()
        {
            new Account
            {
                ID = 1,
                Username = "AppleTree",
                Password = "123456",
                Bio = "A very delicious programmer",
                Email = "apple@gmail.com"
            }
        };

        _db.Setup(db => db.Accounts).ReturnsDbSet(fakeAccountsTable);

        // Account controller with mock database context
        var ac = new AccountController(_db.Object);

        // ===== Act =====
        var result = ac.DeleteAccount(newFakeAccountData.ID);

        // === Assert ====
        Assert.IsType<OkObjectResult>(result);
        _db.Verify(db => db.Accounts.Remove(fakeAccountsTable.First()), Times.Once);
        _db.Verify(db => db.SaveChanges(), Times.Once);
    }
}