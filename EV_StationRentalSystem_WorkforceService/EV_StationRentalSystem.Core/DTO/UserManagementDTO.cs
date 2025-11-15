using System;
using System.Collections.Generic;

namespace EV_StationRentalSystem.Core.DTO
{
    /// <summary>
    /// Request để lấy danh sách users với filter
    /// </summary>
    public class UserFilterRequest
    {
        public string? Role { get; set; } // "staff", "customer", null = all
        public string? Status { get; set; } // "active", "pending", "locked", null = all
        public string? SearchKeyword { get; set; } // Tìm theo email, name, phone
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    /// <summary>
    /// Request để thay đổi role của user
    /// </summary>
    public class ChangeUserRoleRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string NewRole { get; set; } = string.Empty; // "staff", "customer", "manager"
    }

    /// <summary>
    /// Request để khóa/mở khóa user
    /// </summary>
    public class LockUserRequest
    {
        public string UserId { get; set; } = string.Empty;
        public bool IsLocked { get; set; } // true = lock, false = unlock
        public string? Reason { get; set; } // Lý do khóa
    }

    /// <summary>
    /// Response cho danh sách users
    /// </summary>
    public class UserListResponse
    {
        public List<UserProfileResponse> Users { get; set; } = new();
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
    }

    /// <summary>
    /// Request để cập nhật thông tin user (admin)
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
    /// Request để xóa user
    /// </summary>
    public class DeleteUserRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string? Reason { get; set; } // Lý do xóa
    }

    /// <summary>
    /// Request để verify user
    /// </summary>
    public class VerifyUserRequest
    {
        public string Status { get; set; } = string.Empty; // "active", "pending", "locked"
    }
}
