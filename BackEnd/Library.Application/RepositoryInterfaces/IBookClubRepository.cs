using Library.Domain.Entity;
using Microsoft.AspNetCore.Identity;
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

        Task<BookClubJoinRequest> AddJoinRequest(BookClubJoinRequest joinRequest);

        Task RemoveJoinRequest(BookClubJoinRequest joinRequest);

        Task<BookClubJoinRequest> GetJoinRequestById(int joinRequestId);

        Task<BookClubJoinRequest> GetJoinRequestByBookClubAndUser(int bookClubId, string userId);

        Task AddMemberToClub(int bookClubId, ApplicationUser user);

        Task<List<BookClubJoinRequest>> GetJoinRequestsForBookClub(int bookClubId);
    }
}
