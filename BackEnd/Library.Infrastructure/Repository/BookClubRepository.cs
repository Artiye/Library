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
    public class BookClubRepository : IBookClubRepository
    {
        private readonly ApplicationDbContext _context;

        public BookClubRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<BookClub> AddBookClub(BookClub bookClub)
        {
            await _context.Clubs.AddAsync(bookClub);
            await _context.SaveChangesAsync();
            return bookClub;
        }

        public async Task<BookClub> DeleteBookClub(BookClub bookClub)
        {
            _context.Clubs.Remove(bookClub);
            await _context.SaveChangesAsync();
            return bookClub;
        }

        public async Task<BookClub> EditBookClub(BookClub bookClub)
        {
            _context.Clubs.Update(bookClub);
            await _context.SaveChangesAsync();
            return bookClub;
        }

        public async Task<List<BookClub>> GetAllBookClubs()
        {
            var bookList = await _context.Clubs
                .Include(b=> b.Authors)
                .Include(b => b.Books)
                .ToListAsync();
            return bookList;
        }

        public async Task<BookClub> GetBookClubById(int id)
        {
            var bookClub = await _context.Clubs
                .Include(b => b.Authors)
                .Include(b => b.Books)
                .FirstOrDefaultAsync(b => b.BookClubId == id);
            return bookClub;
        }

        public async Task<BookClub> GetBookClubByName(string name)
        {
            var bookClub = await _context.Clubs
                .Include(b => b.Authors)
                .Include(b => b.Books)
                .FirstOrDefaultAsync(b => b.Name == name);
            return bookClub;
        }
    }
}
