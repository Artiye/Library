using AutoMapper;
using Library.Application.DTOs.BookClubDTOs;
using Library.Application.RepositoryInterfaces;
using Library.Application.Responses;
using Library.Application.Services.Interfaces;
using Library.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Application.Services
{
    public class BookClubService(IBookClubRepository bookClubRepository, IMapper mapper, IAuthorRepository authorRepository, IBookRepository bookRepository) : IBookClubService
    {
        private readonly IBookClubRepository _bookClubRepository = bookClubRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IAuthorRepository _authorRepository = authorRepository;
        private readonly IBookRepository _bookRepository = bookRepository;

        public async Task<ApiResponse> AddBookClub(AddBookClubDTOs dto)
        {
            if (dto != null)
            {
                if (string.IsNullOrEmpty(dto.Description) && string.IsNullOrEmpty(dto.Name))
                    return new ApiResponse(400, "Do not leave inputs empty");

                try
                {
                    var bookClub = _mapper.Map<BookClub>(dto);

                    bookClub.Authors ??= [];

                    bookClub.Books ??= [];

                    foreach (var authorId in dto.AuthorIds)
                    {
                        var author = await _authorRepository.GetAuthorById(authorId);
                        if (author == null)

                            return new ApiResponse(404, $"Author with ID {authorId} not found");



                        bookClub.Authors.Add(author);
                    }

                    foreach (var bookId in dto.BookIds)
                    {
                        var book = await _bookRepository.GetBookById(bookId);
                        if (book == null)
                            return new ApiResponse(404, $"Book with ID {bookId} not found");

                        bookClub.Books.Add(book);
                    }

                    await _bookClubRepository.AddBookClub(bookClub);
                    return new ApiResponse(200, "Added bookClub successfully");

                }
                catch (Exception ex)
                {
                    return new ApiResponse(500, $"Failed to add book club:  {ex.Message} ");
                }


            }
            return new ApiResponse(400, "Failed to add book");
        }

        public async Task<ApiResponse> DeleteBookClub(int id)
        {
            var bookClub = await _bookClubRepository.GetBookClubById(id);
            if (bookClub != null)
            {
                await _bookClubRepository.DeleteBookClub(bookClub);
                return new ApiResponse(200, "Deleted Book");
            }
            return new ApiResponse(400, "Failed to delete");

        }


        public async Task<ApiResponse> EditBookClub(EditBookClubDTO dto)
        {
            var bookClub = await _bookClubRepository.GetBookClubById(dto.BookClubId);

            if (bookClub != null)
            {
                if (bookClub.Description == dto.Description && bookClub.Name == dto.Name)
                    return new ApiResponse(400, "Nothing has changed");

                if (string.IsNullOrEmpty(dto.Description) && string.IsNullOrEmpty(dto.Name))
                    return new ApiResponse(400, "Do not leave inputs blank");

                bookClub.Description = dto.Description;
                bookClub.Name = dto.Name;

                await _bookClubRepository.EditBookClub(bookClub);
                return new ApiResponse(200, "Edited successfully");
            }
            return new ApiResponse(400, "Failed to edit");
        }

        public async Task<GetBookClubDTO> GetBookClubById(int id)
        {
            if (id == 0)
            {
                throw new Exception("id cannot be 0");
            }
            var bookClub = await _bookClubRepository.GetBookClubById(id) ??throw new Exception ($"BookClub with id {id} does not exist") ;               
            var bookClubDto = _mapper.Map<GetBookClubDTO>(bookClub);
            return bookClubDto;               
        }
            
        

        public async Task<GetBookClubDTO> GetBookClubByName(string name)
        {
            if (name == null)
            {
                throw new Exception("Name cannot be null");
            }
                var bookClub = await _bookClubRepository.GetBookClubByName(name) ?? throw new Exception($"Bookclub with the name {name} does not exist");
                var bookClubDto = _mapper.Map<GetBookClubDTO>(bookClub);
                return bookClubDto;
         }
            
            
        

        public async Task<List<GetBookClubDTO>> GetBookClubs()
        {
            var bookClub = await _bookClubRepository.GetAllBookClubs();
            var bookClubList = _mapper.Map<List<GetBookClubDTO>>(bookClub);
            return bookClubList;
        }
    }
}
