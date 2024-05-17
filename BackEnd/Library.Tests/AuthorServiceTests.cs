using AutoMapper;
using Library.Application.DTOs.AuthorDTOs;
using Library.Application.Encryption;
using Library.Application.RepositoryInterfaces;
using Library.Application.Services;
using Library.Domain.Entity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Tests
{
    public class AuthorServiceTests
    {
        private Mock<IMapper> mapperMock;
        private Mock<IEncryptionService> encryptionServiceMock;
        private Mock<IAuthorRepository> authorRepositoryMock;
        private Mock<IBookRepository> bookRepositoryMock;
        private AuthorService authorService;
        public AuthorServiceTests()
        {
            mapperMock = new Mock<IMapper>();
            encryptionServiceMock = new Mock<IEncryptionService>();
            authorRepositoryMock = new Mock<IAuthorRepository>();
            bookRepositoryMock = new Mock<IBookRepository>();
            authorService = new AuthorService(authorRepositoryMock.Object, mapperMock.Object, bookRepositoryMock.Object, encryptionServiceMock.Object);
        }
        [Fact]

        public async Task AddAuthor_Success()
        {
            // Arrange

            var addAuthorDTO = new AddAuthorDTO
            {
                FullName = "Testing",
                Nationality = "Albanian",
                BioGraphy = "Programer i forte",
                ProfileImage = "profile.jpg"
            };

            mapperMock.Setup(m => m.Map<Author>(addAuthorDTO)).Returns(new Author());
            encryptionServiceMock.Setup(e => e.EncryptData(It.IsAny<string>())).Returns<string>(s => "encrypted_" + s);

            // Act

            var result = await authorService.AddAuthor(addAuthorDTO);

            //Assert
            Assert.Equal(200, result.Status);
            Assert.Equal("Added author successfully", result.Message);

            authorRepositoryMock.Verify(a => a.AddAuthor(It.Is<Author>(author =>
              author.FullName == "encrypted_Testing" &&
              author.Nationality == "encrypted_Albanian" &&
              author.Biography == "encrypted_Programer i forte" &&
              author.ProfileImage == "encrypted_profile.jpg")), Times.Once);

        }
        [Fact]
        public async Task AddAuthor_Failure()
        {
            // Arrange
            var addAuthorDTO = new AddAuthorDTO
            {
                FullName = "Testing",
                Nationality = "Albanian",
                BioGraphy = "Programer i forte",
                ProfileImage = "profile.jpg"
            };

            mapperMock.Setup(m => m.Map<Author>(addAuthorDTO)).Throws<InvalidOperationException>();

            // Act
            var result = await authorService.AddAuthor(addAuthorDTO);

            // Assert
            Assert.Equal(400, result.Status);
            Assert.Equal("Failed to add author", result.Message);

            authorRepositoryMock.Verify(a => a.AddAuthor(It.IsAny<Author>()), Times.Never);
        }
        

       

        [Fact]

        public async Task AddBookToAuthor_Success()
        {

            // Arrange
            var authorId = 1;
            var bookId = 1;
            var author = new Author { AuthorId = authorId };
            var book = new Book { BookId = bookId };

            authorRepositoryMock.Setup(a => a.GetAuthorById(authorId)).ReturnsAsync(author);
            bookRepositoryMock.Setup(b => b.GetBookById(bookId)).ReturnsAsync(book);

            // Act
            var result = await authorService.AddBookToAuthor(authorId, bookId);

            // Assert

            Assert.Equal(200, result.Status);
            Assert.Equal("Added book to author", result.Message);

            authorRepositoryMock.Verify(a => a.EditAuthor(author), Times.Once);
        }

        [Fact]

        public async Task AddBookToAuthor_Failure()
        {
            // Arrange
            var authorId = 1;
            var bookId = 1;
            var author = new Author { AuthorId = authorId };
            var book = new Book { BookId = bookId };

            authorRepositoryMock.Setup(a => a.GetAuthorById(authorId)).Throws<InvalidOperationException>();
            bookRepositoryMock.Setup(b => b.GetBookById(bookId)).Throws<InvalidOperationException>();

            // Act
            var result = await authorService.AddBookToAuthor(authorId, bookId);

            // Assert

            Assert.Equal(400, result.Status);
            Assert.Equal("Failed to add book to author", result.Message);

            authorRepositoryMock.Verify(a => a.EditAuthor(author), Times.Never);
        }
        [Fact]
        public async Task DeleteAuthor_Success()
        {

            // Arrange
            var authorId = 1;
            var author = new Author { AuthorId = authorId };


            authorRepositoryMock.Setup(a => a.GetAuthorById(authorId)).ReturnsAsync(author);

            // Act
            var result = await authorService.DeleteAuthor(authorId);

            // Assert

            Assert.Equal(200, result.Status);
            Assert.Equal("Deleted Author", result.Message);

            authorRepositoryMock.Verify(a => a.DeleteAuthor(author), Times.Once);

        }

        [Fact]

        public async Task DeleteAuthor_Failure()
        {
            // Arrange
            var authorId = 1;
            var author = new Author { AuthorId = authorId };

            authorRepositoryMock.Setup(a => a.GetAuthorById(authorId)).Throws<InvalidOperationException>();

            // Act

            var result = await authorService.DeleteAuthor(authorId);

            // Assert

            Assert.Equal(400, result.Status);
            Assert.Equal("Failed to delete author", result.Message);

            authorRepositoryMock.Verify(a => a.DeleteAuthor(author), Times.Never);

        }

        [Fact]
        public async Task EditAuthor_Success()
        {
            // Arrange
            var editAuthorDTO = new EditAuthorDTO
            {
                AuthorId = 1,
                FullName = "Test",
                Nationality = "Albanian",
                Biography = "Programer i forte",
                ProfileImage = "profile.jpg"
            };


            var retrievedAuthor = new Author
            {
                AuthorId = 1,
                FullName = "Old Name",
                Nationality = "Old Nationality",
                Biography = "Old Biography",
                ProfileImage = "Old Profile Image"
            };

            mapperMock.Setup(a => a.Map<Author>(editAuthorDTO)).Returns(retrievedAuthor);
            encryptionServiceMock.Setup(e => e.EncryptData(It.IsAny<string>())).Returns<string>(s => "encrypted_" + s);
            authorRepositoryMock.Setup(a => a.GetAuthorById(editAuthorDTO.AuthorId)).ReturnsAsync(retrievedAuthor);

            // Act
            var result = await authorService.EditAuthor(editAuthorDTO);

            // Assert
            Assert.Equal(200, result.Status);
            Assert.Equal("Edited author successfully", result.Message);


            authorRepositoryMock.Verify(a => a.EditAuthor(It.Is<Author>(author =>
              author.FullName == "encrypted_Test" &&
              author.Nationality == "encrypted_Albanian" &&
              author.Biography == "encrypted_Programer i forte" &&
              author.ProfileImage == "encrypted_profile.jpg")), Times.Once);
        }
        [Fact]
        public async Task EditAuthor_Failure()
        {
            // Arrange
            var editAuthorDTO = new EditAuthorDTO
            {
                AuthorId = 1,
                FullName = "Test",
                Nationality = "Albanian",
                Biography = "Programer i forte",
                ProfileImage = "profile.jpg"
            };
            var retrievedAuthor = new Author
            {
                AuthorId = 1,
                FullName = "Old Name",
                Nationality = "Old Nationality",
                Biography = "Old Biography",
                ProfileImage = "Old Profile Image"
            };
            mapperMock.Setup(a => a.Map<Author>(editAuthorDTO)).Throws<InvalidOperationException>();

            // Act
            var result = await authorService.EditAuthor(editAuthorDTO);

            // Assert

            Assert.Equal(400, result.Status);
            Assert.Equal("Failed to edit", result.Message);

            authorRepositoryMock.Verify(a => a.EditAuthor(It.IsAny<Author>()), Times.Never);

        }

        [Fact]

        public async Task EditAuthor_NoChangesMade()
        {
            // Arrange
            var editAuthorDTO = new EditAuthorDTO
            {
                AuthorId = 1,
                FullName = "Old Name",
                Nationality = "Old Nationality",
                Biography = "Old Biography",
                ProfileImage = "Old Profile Image"
            };

            var retrievedAuthor = new Author
            {
                AuthorId = 1,
                FullName = "Old Name",
                Nationality = "Old Nationality",
                Biography = "Old Biography",
                ProfileImage = "Old Profile Image"
            };
            authorRepositoryMock.Setup(a => a.GetAuthorById(editAuthorDTO.AuthorId)).ReturnsAsync(retrievedAuthor);

            // Act

            var result = await authorService.EditAuthor(editAuthorDTO);

            // Assert

            Assert.Equal(400, result.Status);
            Assert.Equal("Nothing was edited", result.Message);

            authorRepositoryMock.Verify(a => a.EditAuthor(It.IsAny<Author>()), Times.Never);
        }
      
    }

}



