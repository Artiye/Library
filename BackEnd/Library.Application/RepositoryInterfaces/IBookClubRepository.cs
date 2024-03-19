using Library.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Application.RepositoryInterfaces
{
    public interface IBookClubRepository
    {   
        Task<BookClub> AddBookClub(BookClub bookClub);

        Task<BookClub> EditBookClub(BookClub bookClub);

        Task<BookClub> DeleteBookClub(BookClub bookClub);

        Task<BookClub> GetBookClubById(int id);

        Task<BookClub> GetBookClubByName(string name);

        Task<List<BookClub>> GetAllBookClubs();

        Task<List<BookClub>> GetBookClubByGenre(string genre);

        Task<List<BookClub>> GetBookClubByLanguage(string language);
    }
}
