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
            authorService = new AuthorService(authorRepositoryMock.Object, mapperMock.Object,bookRepositoryMock.Object, encryptionServiceMock.Object);
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

            authorRepositoryMock.Verify(a => a.AddAuthor(It.IsAny<Author>()), Times.Once);

            }
            
        }
    }

