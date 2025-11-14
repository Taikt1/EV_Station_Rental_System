using AutoMapper;
using EV_StationRentalSystem.Core.DTO;
using EV_StationRentalSystem.Core.Entities;
using EV_StationRentalSystem.Core.HttpClients;
using EV_StationRentalSystem.Core.RepositoryContracts;
using EV_StationRentalSystem.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.Services
{
    public class WorkforceService : IWorkforceService
    {
        private readonly IShiftRepository _shiftRepository;
        private readonly IWorkdayRepository _workdayRepository;
        private readonly IStaffAssignmentRepository _assignmentRepository;
        private readonly UserMicroClient _userMicroClient;
        private readonly IMapper _mapper;

        public WorkforceService(
            IShiftRepository shiftRepository,
            IWorkdayRepository workdayRepository,
            IStaffAssignmentRepository assignmentRepository,
            UserMicroClient userMicroClient,
            IMapper mapper)
        {
            _shiftRepository = shiftRepository;
            _workdayRepository = workdayRepository;
            _assignmentRepository = assignmentRepository;
            _userMicroClient = userMicroClient;
            _mapper = mapper;
        }

        #region Shift Management

        public async Task<ShiftDTO?> GetShiftByIdAsync(Guid shiftId)
        {
            var shift = await _shiftRepository.GetByIdAsync(shiftId);
            return shift == null ? null : _mapper.Map<ShiftDTO>(shift);
        }

        public async Task<List<ShiftDTO>> GetAllShiftsAsync()
        {
            var shifts = await _shiftRepository.GetAllAsync();
            return _mapper.Map<List<ShiftDTO>>(shifts);
        }

        public async Task<ShiftDTO> CreateShiftAsync(CreateShiftRequest request)
        {
            var shift = new Shift
            {
                ShiftId = Guid.NewGuid(),
                ShiftName = request.ShiftName,
                StartTime = request.StartTime,
                EndTime = request.EndTime
            };

            var createdShift = await _shiftRepository.AddAsync(shift);
            return _mapper.Map<ShiftDTO>(createdShift);
        }

        public async Task<ShiftDTO?> UpdateShiftAsync(Guid shiftId, UpdateShiftRequest request)
        {
            var shift = await _shiftRepository.GetByIdAsync(shiftId);
            if (shift == null) return null;

            if (!string.IsNullOrEmpty(request.ShiftName))
                shift.ShiftName = request.ShiftName;

            if (request.StartTime.HasValue)
                shift.StartTime = request.StartTime.Value;

            if (request.EndTime.HasValue)
                shift.EndTime = request.EndTime.Value;

            var updatedShift = await _shiftRepository.UpdateAsync(shift);
            return _mapper.Map<ShiftDTO>(updatedShift);
        }

        public async Task<bool> DeleteShiftAsync(Guid shiftId)
        {
            return await _shiftRepository.DeleteAsync(shiftId);
        }

        #endregion

        #region Workday Management

        public async Task<WorkdayDTO?> GetWorkdayByIdAsync(Guid workdayId, string? authToken = null)
        {
            var workday = await _workdayRepository.GetByIdAsync(workdayId, includeAssignments: true);
            if (workday == null) return null;

            var workdayDto = _mapper.Map<WorkdayDTO>(workday);

            // Lấy thông tin nhân viên từ UserService
            if (authToken != null)
            {
                var userProfile = await _userMicroClient.GetUserProfileAsync(workday.StaffId.ToString(), authToken);
                workdayDto.StaffInfo = userProfile;
            }

            return workdayDto;
        }

        public async Task<List<WorkdayDTO>> GetWorkdaysByFilterAsync(WorkdayFilterRequest filter, string? authToken = null)
        {
            var workdays = await _workdayRepository.GetFilteredAsync(
                filter.StaffId,
                filter.BranchId,
                filter.StartDate,
                filter.EndDate,
                includeAssignments: true
            );

            var workdayDtos = _mapper.Map<List<WorkdayDTO>>(workdays);

            // Lấy thông tin nhân viên cho tất cả workdays
            if (authToken != null && workdayDtos.Any())
            {
                var staffIds = workdayDtos.Select(w => w.StaffId.ToString()).Distinct().ToList();
                var userProfiles = await _userMicroClient.GetMultipleUserProfilesAsync(staffIds, authToken);

                foreach (var workdayDto in workdayDtos)
                {
                    if (userProfiles.TryGetValue(workdayDto.StaffId.ToString(), out var userProfile))
                    {
                        workdayDto.StaffInfo = userProfile;
                    }
                }
            }

            return workdayDtos;
        }

        public async Task<WorkdayDTO> CreateWorkdayAsync(CreateWorkdayRequest request)
        {
            // Kiểm tra xem nhân viên đã có workday trong ngày này chưa
            var exists = await _workdayRepository.StaffHasWorkdayOnDateAsync(request.StaffId, request.Date);
            if (exists)
            {
                throw new InvalidOperationException("Nhân viên đã có lịch làm việc trong ngày này");
            }

            var workday = new Workday
            {
                WorkdayId = Guid.NewGuid(),
                StaffId = request.StaffId,
                BranchId = request.BranchId,
                Date = request.Date.Date // Chỉ lấy phần ngày
            };

            var createdWorkday = await _workdayRepository.AddAsync(workday);
            return _mapper.Map<WorkdayDTO>(createdWorkday);
        }

        public async Task<WorkdayDTO?> UpdateWorkdayAsync(Guid workdayId, UpdateWorkdayRequest request)
        {
            var workday = await _workdayRepository.GetByIdAsync(workdayId);
            if (workday == null) return null;

            if (request.StaffId.HasValue)
                workday.StaffId = request.StaffId.Value;

            if (request.BranchId.HasValue)
                workday.BranchId = request.BranchId.Value;

            if (request.Date.HasValue)
                workday.Date = request.Date.Value.Date;

            var updatedWorkday = await _workdayRepository.UpdateAsync(workday);
            return _mapper.Map<WorkdayDTO>(updatedWorkday);
        }

        public async Task<bool> DeleteWorkdayAsync(Guid workdayId)
        {
            return await _workdayRepository.DeleteAsync(workdayId);
        }

        #endregion

        #region Assignment Management

        public async Task<StaffAssignmentDTO?> GetAssignmentByIdAsync(Guid assignmentId)
        {
            var assignment = await _assignmentRepository.GetByIdAsync(assignmentId, includeRelations: true);
            return assignment == null ? null : _mapper.Map<StaffAssignmentDTO>(assignment);
        }

        public async Task<List<StaffAssignmentDTO>> GetAssignmentsByWorkdayAsync(Guid workdayId)
        {
            var assignments = await _assignmentRepository.GetByWorkdayIdAsync(workdayId, includeRelations: true);
            return _mapper.Map<List<StaffAssignmentDTO>>(assignments);
        }

        public async Task<StaffAssignmentDTO> CreateAssignmentAsync(CreateAssignmentRequest request)
        {
            // Kiểm tra workday tồn tại
            var workdayExists = await _workdayRepository.ExistsAsync(request.WorkdayId);
            if (!workdayExists)
            {
                throw new InvalidOperationException("Workday không tồn tại");
            }

            // Kiểm tra shift tồn tại
            var shiftExists = await _shiftRepository.ExistsAsync(request.ShiftId);
            if (!shiftExists)
            {
                throw new InvalidOperationException("Shift không tồn tại");
            }

            // Kiểm tra xem workday đã có shift này chưa
            var hasShift = await _assignmentRepository.WorkdayHasShiftAsync(request.WorkdayId, request.ShiftId);
            if (hasShift)
            {
                throw new InvalidOperationException("Ca làm việc này đã được phân công cho ngày làm việc này");
            }

            var assignment = new StaffAssignment
            {
                AssignmentId = Guid.NewGuid(),
                WorkdayId = request.WorkdayId,
                ShiftId = request.ShiftId,
                Task = request.Task,
                Status = "Assigned"
            };

            var createdAssignment = await _assignmentRepository.AddAsync(assignment);
            return _mapper.Map<StaffAssignmentDTO>(createdAssignment);
        }

        public async Task<StaffAssignmentDTO?> UpdateAssignmentAsync(Guid assignmentId, UpdateAssignmentRequest request)
        {
            var assignment = await _assignmentRepository.GetByIdAsync(assignmentId);
            if (assignment == null) return null;

            if (request.ShiftId.HasValue)
            {
                // Kiểm tra shift mới tồn tại
                var shiftExists = await _shiftRepository.ExistsAsync(request.ShiftId.Value);
                if (!shiftExists)
                {
                    throw new InvalidOperationException("Shift không tồn tại");
                }

                assignment.ShiftId = request.ShiftId.Value;
            }

            if (request.Task != null)
                assignment.Task = request.Task;

            if (!string.IsNullOrEmpty(request.Status))
                assignment.Status = request.Status;

            var updatedAssignment = await _assignmentRepository.UpdateAsync(assignment);
            return _mapper.Map<StaffAssignmentDTO>(updatedAssignment);
        }

        public async Task<bool> DeleteAssignmentAsync(Guid assignmentId)
        {
            return await _assignmentRepository.DeleteAsync(assignmentId);
        }

        public async Task<List<StaffAssignmentDTO>> CreateBulkAssignmentsAsync(BulkAssignmentRequest request)
        {
            var assignments = new List<StaffAssignment>();
            var currentDate = request.StartDate.Date;

            while (currentDate <= request.EndDate.Date)
            {
                // Tạo hoặc lấy workday cho ngày này
                var workdays = await _workdayRepository.GetFilteredAsync(
                    request.StaffId, 
                    request.BranchId, 
                    currentDate, 
                    currentDate
                );

                Workday workday;
                if (!workdays.Any())
                {
                    // Tạo workday mới
                    workday = new Workday
                    {
                        WorkdayId = Guid.NewGuid(),
                        StaffId = request.StaffId,
                        BranchId = request.BranchId,
                        Date = currentDate
                    };
                    workday = await _workdayRepository.AddAsync(workday);
                }
                else
                {
                    workday = workdays.First();
                }

                // Tạo assignment cho mỗi shift
                foreach (var shiftId in request.ShiftIds)
                {
                    // Kiểm tra xem shift này đã được phân công chưa
                    var hasShift = await _assignmentRepository.WorkdayHasShiftAsync(workday.WorkdayId, shiftId);
                    if (!hasShift)
                    {
                        assignments.Add(new StaffAssignment
                        {
                            AssignmentId = Guid.NewGuid(),
                            WorkdayId = workday.WorkdayId,
                            ShiftId = shiftId,
                            Task = request.Task,
                            Status = "Assigned"
                        });
                    }
                }

                currentDate = currentDate.AddDays(1);
            }

            if (assignments.Any())
            {
                await _assignmentRepository.AddRangeAsync(assignments);
            }

            return _mapper.Map<List<StaffAssignmentDTO>>(assignments);
        }

        #endregion

        #region Special Queries

        public async Task<List<WorkdayDTO>> GetStaffScheduleAsync(Guid staffId, DateTime startDate, DateTime endDate, string? authToken = null)
        {
            var workdays = await _workdayRepository.GetFilteredAsync(
                staffId,
                null,
                startDate,
                endDate,
                includeAssignments: true
            );

            var workdayDtos = _mapper.Map<List<WorkdayDTO>>(workdays);

            // Lấy thông tin nhân viên
            if (authToken != null)
            {
                var userProfile = await _userMicroClient.GetUserProfileAsync(staffId.ToString(), authToken);
                foreach (var workdayDto in workdayDtos)
                {
                    workdayDto.StaffInfo = userProfile;
                }
            }

            return workdayDtos;
        }

        public async Task<List<WorkdayDTO>> GetBranchScheduleAsync(Guid branchId, DateTime startDate, DateTime endDate, string? authToken = null)
        {
            var workdays = await _workdayRepository.GetFilteredAsync(
                null,
                branchId,
                startDate,
                endDate,
                includeAssignments: true
            );

            var workdayDtos = _mapper.Map<List<WorkdayDTO>>(workdays);

            // Lấy thông tin tất cả nhân viên
            if (authToken != null && workdayDtos.Any())
            {
                var staffIds = workdayDtos.Select(w => w.StaffId.ToString()).Distinct().ToList();
                var userProfiles = await _userMicroClient.GetMultipleUserProfilesAsync(staffIds, authToken);

                foreach (var workdayDto in workdayDtos)
                {
                    if (userProfiles.TryGetValue(workdayDto.StaffId.ToString(), out var userProfile))
                    {
                        workdayDto.StaffInfo = userProfile;
                    }
                }
            }

            return workdayDtos;
        }

        #endregion
    }
}
