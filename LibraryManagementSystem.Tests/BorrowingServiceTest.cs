using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using Xunit;

namespace LibraryManagementSystem.Tests;

public class BorrowingServiceTest
{
    private readonly LibraryContext _context;
    private readonly BorrowingService _borrowingService;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Guid _fakeUserId;

    public BorrowingServiceTest()
    {
        var options = new DbContextOptionsBuilder<LibraryContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new LibraryContext(options);
        var loggerMock = new Mock<ILogger<BorrowingService>>();

        _fakeUserId = Guid.NewGuid();

     
        var httpContext = new DefaultHttpContext();
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, _fakeUserId.ToString())
        }, "TestAuth");
        httpContext.User = new ClaimsPrincipal(identity);

        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(httpContext);

        _borrowingService = new BorrowingService(_context, _httpContextAccessorMock.Object, loggerMock.Object);

        
        _context.Users.Add(new User
        {
            Id = _fakeUserId,
            Email = "test@example.com",
            IsActive = true,
            Password = "Aa$123",
            Role = "Member"
        });
        _context.SaveChanges(); 
    }

    [Fact]
    public async Task BorrowBookAsync_ReturnsError_WhenBookNotFound()
    {
        // arrange
        //act
        var result = await _borrowingService.BorrowBookAsync(Guid.NewGuid());
        //assert
        Assert.Equal("Book not found.", result);
    }
    [Fact]
    public async Task BorrowBookAsynch_ReturnError_WhenBookOutOfStock()
    {//arrange
        var bookId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _context.Users.Add(new User
        {
            Id = userId,
            Email = "test@gmail.com",
            Password = "Aa$123",
            Role = "Member",
            IsActive = true
        });
        _context.Books.Add(new Book
        {
            Id = bookId,
            Title = "Test Book",
            Author = "Test Author",
            ISBN = "1234567890",
            Genre = "Fiction",
            Quantity = 0,
            AvailableCopies = 0
        });
        //act
        await _context.SaveChangesAsync();
        
        var result = await _borrowingService.BorrowBookAsync(bookId);
        //assert
        Assert.Equal("Book out of stock.", result);

    }
    //[Fact]
    //public async Task BorrowBookAsync_ReturnsError_WhenUserNotFound()
    //{
    //    // Arrange
    //    var bookId = Guid.NewGuid();
    //    _context.Books.Add(new Book
    //    {
    //        Id = bookId,
    //        Title = "Test Book",
    //        Author = "Test Author",
    //        ISBN = "1234567890",
    //        Genre = "Fiction",
    //        Quantity = 5,
    //        AvailableCopies = 5
    //    });
    //    await _context.SaveChangesAsync();
    //    // Act
    //    var result = await _borrowingService.BorrowBookAsync(bookId);
    //    // Assert
    //    Assert.Equal("User not found.", result);
    //}
    [Fact]
    public async Task BorrowBookAsync_ReturnsError_WhenUserISNotActive()
    {
        var bookId = Guid.NewGuid();
        var userId = Guid.NewGuid();

       
        _context.Users.Add(new User
        {
            Id = userId,
            Email = "test@example.com",
            Password = "Aa$123",
            Role = "Member",
            IsActive = false
        });
        await _context.SaveChangesAsync();
        var loggerMock = new Mock<ILogger<BorrowingService>>();

       
        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
    new Claim(ClaimTypes.NameIdentifier, userId.ToString())
}, "TestAuth"));
        Mock<IHttpContextAccessor> _httpContextAccessorMock;
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(httpContext);

        _context.Books.Add(new Book
        {
            Id = bookId,
            Title = "Test Book",
            Author = "Test Author",
            ISBN = "1234567890",
            Genre = "Fiction",
            Quantity = 5,
            AvailableCopies = 5
        });
        await _context.SaveChangesAsync();
       var service = new BorrowingService(_context, _httpContextAccessorMock.Object,  loggerMock.Object);

        var result = await service.BorrowBookAsync(bookId);
        Assert.Equal("User is not active.", result);
    }
    [Fact]
    public async Task BorrowBookAsync_ReturnsError_WhenBookAlreadyBorrowed()
    {
        var bookId = Guid.NewGuid();
       // var userId = Guid.NewGuid();

        //_context.Users.Add(new User
        //{
        //    Id = _fakeUserId,
        //    Email = "example@gmail.com",
        //    Password = "Aa$123",
        //    Role = "Member",
        //    IsActive = false
        //});
        _context.Books.Add(new Book
        {
            Id = bookId,
            Title = "Test Book",
            Author = "Test Author",
            ISBN = "1234567890",
            Genre = "Fiction",
            Quantity = 5,
            AvailableCopies = 5
        });
        await _context.SaveChangesAsync();
        _context.Borrowings.Add(new Borrowing
        {
            Id = Guid.NewGuid(),
            BookId = bookId,
            UserId = _fakeUserId,
            BorrowDate = DateTime.Now,
            ReturnDate = DateTime.MinValue
        });
        await _context.SaveChangesAsync();
        var result = await _borrowingService.BorrowBookAsync(bookId);
        Assert.Equal("You have already borrowed this book and haven't returned it yet.", result);


    }
    [Fact]
    public async Task BorrowBookAsync_ReturnsError_WhenUserHasThreeActiveBorrowing()
    {
        var bookId = Guid.NewGuid();
        var bookId2 = Guid.NewGuid();
        var bookId3 = Guid.NewGuid();
        var bookId4 = Guid.NewGuid();
       // var userId = Guid.NewGuid();

        //_context.Users.Add(new User
        //{
        //    Id = _fakeUserId,
        //    Email = "example@gmail.com",
        //    Password = "Aa$123",
        //    Role = "Member",
        //    IsActive = false
        //});
        _context.Books.Add(new Book
        {
            Id = bookId,
            Title = "Test Book",
            Author = "Test Author",
            ISBN = "1234567890",
            Genre = "Fiction",
            Quantity = 5,
            AvailableCopies = 5
        });
        _context.Books.Add(new Book
        {
            Id = bookId2,
            Title = "Test Book",
            Author = "Test Author",
            ISBN = "1234567890",
            Genre = "Fiction",
            Quantity = 5,
            AvailableCopies = 5
        });
        _context.Books.Add(new Book
        {
            Id = bookId3,
            Title = "Test Book",
            Author = "Test Author",
            ISBN = "1234567890",
            Genre = "Fiction",
            Quantity = 5,
            AvailableCopies = 5
        });
        _context.Books.Add(new Book
        {
            Id = bookId4,
            Title = "Test Book",
            Author = "Test Author",
            ISBN = "1234567890",
            Genre = "Fiction",
            Quantity = 5,
            AvailableCopies = 5
        });


        _context.Borrowings.Add(new Borrowing
        {
            Id = Guid.NewGuid(),
            BookId = bookId,
            UserId = _fakeUserId,
            BorrowDate = DateTime.Now,
            ReturnDate = DateTime.MinValue
        });
        _context.Borrowings.Add(new Borrowing
        {
            Id = Guid.NewGuid(),
            BookId = bookId2,
            UserId =_fakeUserId,
            BorrowDate = DateTime.Now,
            ReturnDate = DateTime.MinValue
        });
        _context.Borrowings.Add(new Borrowing
        {
            Id = Guid.NewGuid(),
            BookId = bookId3,
            UserId = _fakeUserId,
            BorrowDate = DateTime.Now,
            ReturnDate = DateTime.MinValue
        });

        await _context.SaveChangesAsync();
        var result = await _borrowingService.BorrowBookAsync(bookId4);
        Assert.Equal("You can't borrow more than 3 books.", result);
    }
    [Fact]
    public async Task BorrowBookAsync_WhenBorrowDoneSuccessfully()
    {
        var bookId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _context.Books.Add(new Book
        {
            Id = bookId,
            Title = "Test Book",
            Author = "Test Author",
            ISBN = "1234567890",
            Genre = "Fiction",
            Quantity = 5,
            AvailableCopies = 5
        });
        _context.Users.Add(new User
        {
            Id = userId,
            Email = "example@gmail.com",
            Password = "Aa$123",
            Role = "Member",
            IsActive = true
        });
        await _context.SaveChangesAsync();
        var result = await _borrowingService.BorrowBookAsync(bookId);
        Assert.Equal("Book borrowed successfully.", result);


    }
    [Fact]
    public async Task ReturnBookAsync_ReturnsError_whenBorrowRecordNotFound()
    {
        // Arrange
        var borrowingId = Guid.NewGuid();
        // Act
        var result = await _borrowingService.ReturnBookAsync(borrowingId);
        // Assert
        Assert.Equal("Borrowing record not found.", result);
    }
    [Fact]
    public async Task ReturnBookAsync_ReturnsError_whenBookAlreadyReturned()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var bookId = Guid.NewGuid();
        _context.Books.Add(new Book
        {
            Id = bookId,
            Title = "Test Book",
            Author = "Test Author",
            ISBN = "1234567890",
            Genre = "Fiction",
            Quantity = 5,
            AvailableCopies = 5
        });
        _context.Users.Add(new User
        {
            Id = userId,
            Email = "example@gmail.com",
            Password = "Aa$123",
            Role = "Member",
            IsActive = true
        });
        await _context.SaveChangesAsync();
        var borrowingId = Guid.NewGuid();
        var borrowing = new Borrowing
        {
            Id = borrowingId,
            BookId = bookId,
            UserId = userId,
            BorrowDate = DateTime.Now,
            ReturnDate = DateTime.Now,
            IsReturned = true
        };
        _context.Borrowings.Add(borrowing);
        await _context.SaveChangesAsync();
        // Act
        var result = await _borrowingService.ReturnBookAsync(borrowingId);

        // Assert
        Assert.Equal("This book has already been returned.", result);
    }
    [Fact]
    public async Task ReturnBookAsync_ReturnsSuccess_whenBookReturned()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        //var userId = Guid.NewGuid();

        _context.Books.Add(new Book
        {
            Id = bookId,
            Title = "Test Book",
            Author = "Test Author",
            ISBN = "1234567890",
            Genre = "Fiction",
            Quantity = 5,
            AvailableCopies = 5
        });
        //_context.Users.Add(new User
        //{
        //    Id =_fakeUserId,
        //    Email = "example@gmail.com",
        //    Password = "Aa$123",
        //    Role = "Member",
        //    IsActive = true
        //});
        await _context.SaveChangesAsync();
        var borrowingId = Guid.NewGuid();
        var borrowing = new Borrowing
        {
            Id = borrowingId,
            BookId = bookId,
            UserId = _fakeUserId,
            BorrowDate = DateTime.Now,
            ReturnDate = DateTime.MinValue
        };
        _context.Borrowings.Add(borrowing);
        await _context.SaveChangesAsync();
        // Act
        var result = await _borrowingService.ReturnBookAsync(borrowingId);
        // Assert
        Assert.Equal("Book returned successfully.", result);
    }

    [Fact]
    public async Task Borrowing_WhenBorrowingIsOverDue()
    {
        var bookId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var borrowingId = Guid.NewGuid();


        _context.Books.Add(new Book
        {
            Id = bookId,
            Title = "Test Book",
            Author = "Test Author",
            ISBN = "1234567890",
            Genre = "Fiction",
            Quantity = 5,
            AvailableCopies = 5
        });
        _context.Users.Add(new User
        {
            Id = userId,
            Email = "Example@gmail.com",
            Password = "Aa$123",
            Role = "Member",
            IsActive = true
        });
        var borrowDate = DateTime.Now.AddDays(-15);

        var borrowing = new Borrowing
        {
            Id = borrowingId,
            BookId = bookId,
            UserId = userId,
            BorrowDate = borrowDate,
            ReturnDate = DateTime.MinValue,
            IsReturned = false,
            DueDate = borrowDate.AddDays(14)

        };
        _context.Borrowings.Add(borrowing);

        await _context.SaveChangesAsync();
        Assert.True(borrowing.IsOverdue);



    }

    [Fact]
    public async Task getBorrowingById_Test()
    {
        var bookId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var borrowingId = Guid.NewGuid();
        _context.Books.Add(new Book
        {
            Id = bookId,
            Title = "Test Book",
            Author = "Test Author",
            ISBN = "1234567890",
            Genre = "Fiction",
            Quantity = 5,
            AvailableCopies = 5
        });


        _context.Users.Add(new User
        {
            Id = userId,
            Email = "Example@gmail.com",
            Password = "Aa$123",
            Role = "Member",
            IsActive = true
        });


        var borrowing = new Borrowing
        {
            Id = borrowingId,
            BookId = bookId,
            UserId = userId,
            BorrowDate = DateTime.Now,
            ReturnDate = DateTime.MinValue,
            IsReturned = false,


        };
        _context.Borrowings.Add(borrowing);
        await _context.SaveChangesAsync();
        var result = await _borrowingService.GetBorrowingByIdAsync(borrowingId);
        Assert.NotNull(result);
        Assert.Equal(borrowingId, result.Id);
        Assert.Equal(bookId, result.BookId);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(borrowing.BorrowDate, result.BorrowDate);
        Assert.Equal(borrowing.ReturnDate, result.ReturnDate);
        Assert.Equal(borrowing.IsReturned, result.IsReturned);

        Assert.Equal(borrowing.DueDate, result.DueDate);
        Assert.Equal(borrowing.IsOverdue, result.IsOverdue);




    }
    [Fact]
    public async Task GetborrowingById_WhenNotFound()
    {
        var result = await _borrowingService.GetBorrowingByIdAsync(Guid.NewGuid());
        Assert.Null(result);
    }
    [Fact]
    public async Task UpdateBorrowing_Test()
    {
        var bookId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var borrowingId = Guid.NewGuid();
        _context.Books.Add(new Book
        {
            Id = bookId,
            Title = "Test Book",
            Author = "Test Author",
            ISBN = "1234567890",
            Genre = "Fiction",
            Quantity = 5,
            AvailableCopies = 5
        });
        _context.Users.Add(new User
        {
            Id = userId,
            Email = "Example@gmail.com",
            Password = "Aa$123",
            Role = "Member",
            IsActive = true
        });


        var borrowing = new Borrowing
        {
            Id = borrowingId,
            BookId = bookId,
            UserId = userId,
            BorrowDate = DateTime.Now,
            ReturnDate = DateTime.MinValue,
            IsReturned = false,


        };

        _context.Borrowings.Add(borrowing);
        await _context.SaveChangesAsync();
        borrowing.ReturnDate = DateTime.Now;
        borrowing.IsReturned = true;
        var result = await _borrowingService.UpdateBorrowingAsync(borrowing);
        var updatedBorrowing = await _borrowingService.GetBorrowingByIdAsync(borrowingId);
        Assert.NotNull(updatedBorrowing);
        Assert.Equal(borrowingId, updatedBorrowing.Id);
        Assert.Equal(bookId, updatedBorrowing.BookId);
        Assert.Equal(userId, updatedBorrowing.UserId);
        Assert.Equal(borrowing.BorrowDate, updatedBorrowing.BorrowDate);
        Assert.Equal(borrowing.ReturnDate, updatedBorrowing.ReturnDate);
        Assert.Equal(borrowing.IsReturned, updatedBorrowing.IsReturned);
        var updated = await _context.Borrowings.FindAsync(borrowingId);
        Assert.True(updated.IsReturned);
        Assert.NotEqual(DateTime.MinValue, updated.ReturnDate);
        Assert.Equal("Borrowing updated successfully.", result);

    }
    [Fact]
    public async Task UpdateBorrowing_WhenBorrowingNotFound()
    {
        var borrowingId = Guid.NewGuid();
        var bookId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _context.Users.Add(new User
        {
            Id = userId,
            Email = "Example@gmail.com",
            Password = "Aa$123",
            Role = "Member",
            IsActive = true
        });
        _context.Books.Add(new Book
        {
            Id = bookId,
            Title = "Test Book",
            Author = "Test Author",
            ISBN = "1234567890",
            Genre = "Fiction",
            Quantity = 5,
            AvailableCopies = 5
        });


        var borrowing = new Borrowing
        {
            Id = borrowingId,
            BookId = bookId,
            UserId = userId,
            BorrowDate = DateTime.Now,
            ReturnDate = DateTime.MinValue,
            IsReturned = false,


        };

        await _context.SaveChangesAsync();
        var result = await _borrowingService.UpdateBorrowingAsync(borrowing);
        Assert.Null(result);
    }
    [Fact]
    public async Task GetBorrowingHistory_ReturnsOk_WhenBorrowingsExist()
    {
        // Arrange
       // var userId = Guid.NewGuid();
        var bookId = Guid.NewGuid();
        _context.Books.Add(new Book
        {
            Id = bookId,
            Title = "Test Book",
            Author = "Test Author",
            ISBN = "1234567890",
            Genre = "Fiction",
            Quantity = 5,
            AvailableCopies = 5
        });
        //_context.Users.Add(new User
        //{
        //    Id = _fakeUserId,
        //    Email = "example@gmail.com",
        //    Password = "Aa$123",
        //    Role = "Member",
        //});
        var borrowingList = new List<Borrowing>
        {
            new Borrowing { Id = Guid.NewGuid(), UserId = _fakeUserId, BookId =bookId, ReturnDate = DateTime.Now }
        };
        _context.Borrowings.AddRange(borrowingList);
        await _context.SaveChangesAsync();



        // Act
        var result = await _borrowingService.GetBorrowingHistoryAsynch();

        // Assert
        Assert.IsType<List<BorrowingHistoryDto>>(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task GetBorrowingHistory_ReturnsNotFound_WhenNoData()
    {
        // Arrange
        var userId = Guid.NewGuid();



        // Act
        var result = await _borrowingService.GetBorrowingHistoryAsynch();

        // Assert
        Assert.IsType<List<BorrowingHistoryDto>>(result);
        Assert.Equal([], result);
    }

    [Fact]

    public async Task GetActiveBorrowings_ReturnsOk_WhenActiveBorrowingsExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var bookId1 = Guid.NewGuid();
        var bookId2 = Guid.NewGuid();
        _context.Books.Add(new Book
        {
            Id = bookId1,
            Title = "Test Book",
            Author = "Test Author",
            ISBN = "1234567890",
            Genre = "Fiction",
            Quantity = 5,
            AvailableCopies = 5
        });
        _context.Books.Add(new Book
        {
            Id = bookId2,
            Title = "Test Book 2",
            Author = "Test Author 2",
            ISBN = "0987654321",
            Genre = "Non-Fiction",
            Quantity = 3,
            AvailableCopies = 3
        });
        await _context.SaveChangesAsync();

        var activeBorrowings = new List<Borrowing>
    {
        new Borrowing
        {
            Id = Guid.NewGuid(),
            UserId = _fakeUserId,
            BookId =bookId1,
            BorrowDate = DateTime.Now.AddDays(-5),
            ReturnDate = DateTime.MinValue, 
            IsReturned = false
        },
        new Borrowing
        {
            Id = Guid.NewGuid(),
            UserId = _fakeUserId,
            BookId = bookId2,
            BorrowDate = DateTime.Now.AddDays(-10),
            ReturnDate = DateTime.MinValue, 
            IsReturned = false
        }
    };

        _context.Borrowings.AddRange(activeBorrowings);
        await _context.SaveChangesAsync();

        // Act
        var result = await _borrowingService.GetCurrentBorrowingsAsync();

        // Assert
        Assert.IsType<List<BorrowingHistoryDto>>(result);
        Assert.NotEmpty(result);
        Assert.All(result, b => Assert.False(b.IsReturned)); // Ensure all borrowings are active
    }


    [Fact]
    public async Task GetActiveBorrowings_ReturnsNotFound_WhenNoActiveBorrowings()
    {
        // Arrange
        var userId = Guid.NewGuid();


        // Act
        var result = await _borrowingService.GetBorrowingHistoryAsynch();

        // Assert
        Assert.Empty(result);
    }










}
