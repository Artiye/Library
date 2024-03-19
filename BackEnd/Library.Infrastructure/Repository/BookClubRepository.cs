using Library.Application.RepositoryInterfaces;
using Library.Domain.Entity;
using Library.Domain.Enums;
using Library.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Infrastructure.Repository
{
    public class BookClubRepository(ApplicationDbContext context) : IBookClubRepository
    {
        private readonly ApplicationDbContext _context = context;

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
        public async Task<List<BookClub>> GetBookClubByLanguage(string language)
        {
            if (!Enum.TryParse<Languages>(language, true, out var selectedLanguage))
                return new List<BookClub>();

            var bookclubList = await _context.Clubs
                .Include(b => b.Authors)
                .Include(b => b.Books)
                .Where(b => b.Languages == selectedLanguage)
                .ToListAsync();

            return bookclubList;
        }

        public async Task<List<BookClub>> GetBookClubByGenre(string genre)
        {
            if (!Enum.TryParse<GenreTypes>(genre, true, out var selectedGenre))
                return new List<BookClub>();

            var bookclubList = await _context.Clubs
                .Include(b => b.Authors)
                .Include(b => b.Books)
                .Where(b => b.Genre == selectedGenre)
                .ToListAsync();
            
            return bookclubList;
        }
    }
}
