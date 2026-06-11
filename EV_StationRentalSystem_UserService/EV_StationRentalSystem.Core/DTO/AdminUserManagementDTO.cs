using System;

namespace EV_StationRentalSystem.Core.DTO
{
    /// <summary>
    /// Request để admin cập nhật thông tin user
    /// </summary>
    public class AdminUpdateUserRequest
    {
        public string? FullName { get; set; }
        public DateTime? Dob { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Status { get; set; } // "active", "pending", "locked"
        public string? Role { get; set; } // "staff", "customer", "manager"
    }

    /// <summary>
    /// Request để thay đổi role
    /// </summary>
    public class ChangeRoleRequest
    {
        public string Role { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request để khóa/mở user
    /// </summary>
    public class LockUnlockRequest
    {
        public string? Reason { get; set; }
    }

    /// <summary>
    /// Request để xóa user
    /// </summary>
    public class DeleteUserRequestDTO
    {
        public string? Reason { get; set; }
    }
}
