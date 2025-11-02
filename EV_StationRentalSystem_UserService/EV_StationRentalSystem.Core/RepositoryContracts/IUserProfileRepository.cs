using EV_StationRentalSystem.Core.Entities;

namespace EV_StationRentalSystem.Core.RepositoryContracts
{
    public interface IUserProfileRepository
    {
        Task<UserProfile?> GetByUserIdAsync(string userId);
        Task<UserProfile> AddAsync(UserProfile userProfile);
        Task<UserProfile> UpdateAsync(UserProfile userProfile);
        Task<bool> DeleteAsync(Guid id);
    }
}
