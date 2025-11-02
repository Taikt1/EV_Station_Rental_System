using EV_StationRentalSystem.Core.Entities;
using EV_StationRentalSystem.Core.RepositoryContracts;
using Microsoft.EntityFrameworkCore;

namespace EV_StationRentalSystem.Infrastructure.Repositories
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly UserServiceDbContext _context;

        public UserProfileRepository(UserServiceDbContext context)
        {
            _context = context;
        }

        public async Task<UserProfile?> GetByUserIdAsync(string userId)
        {
            return await _context.UserProfiles
                .Include(p => p.Account)
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<UserProfile> AddAsync(UserProfile userProfile)
        {
            await _context.UserProfiles.AddAsync(userProfile);
            await _context.SaveChangesAsync();
            return userProfile;
        }

        public async Task<UserProfile> UpdateAsync(UserProfile userProfile)
        {
            _context.UserProfiles.Update(userProfile);
            await _context.SaveChangesAsync();
            return userProfile;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var userProfile = await _context.UserProfiles.FindAsync(id);
            if (userProfile == null)
            {
                return false;
            }

            _context.UserProfiles.Remove(userProfile);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
