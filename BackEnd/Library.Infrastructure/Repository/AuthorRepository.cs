using AutoMapper;
using Library.Application.DTOs.BookDTOs;
using Library.Application.RepositoryInterfaces;
using Library.Domain.Entity;
using Library.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Infrastructure.Repository
{
    public class AuthorRepository(ApplicationDbContext context, IMapper mapper) : IAuthorRepository
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<Author> AddAuthor(Author author)
        {
            await _context.Authors.AddAsync(author);
            await _context.SaveChangesAsync();
            return author;
        }
       


        public async Task<Author> DeleteAuthor(Author author)
        {
            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
            return author;
        }

        public async Task<Author> EditAuthor(Author author)
        {
            _context.Authors.Update(author);
            await _context.SaveChangesAsync();
            return author;
        }

        public async Task<List<Author>> GetAllAuthorList()
        {
            var authorList = await _context.Authors.Include(a => a.Books).ToListAsync();
            return authorList;
        }

        public async Task<Author> GetAuthorById(int id)
        {           
            var author = await _context.Authors.Include(a => a.Books).FirstOrDefaultAsync(i => i.AuthorId == id);
            return author;              
        }

        public async Task<Author> GetAuthorByName(string name)
        {
            var author = await _context.Authors.Include(a => a.Books).FirstOrDefaultAsync(s => s.FullName == name);
            return author;
        }

        public async Task<List<GetBookDTO>> GetBooksByAuthorId(int authorId)
        {
            var author = await _context.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.AuthorId == authorId);

            if (author != null)
            {
                
                var booksDto = _mapper.Map<List<GetBookDTO>>(author.Books);
                return booksDto;
            }

            
            return new List<GetBookDTO>();
        }
    }
}
