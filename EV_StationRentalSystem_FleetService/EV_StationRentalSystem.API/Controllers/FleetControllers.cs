using EV_StationRentalSystem.Core.DTO;
using EV_StationRentalSystem.Core.ServiceContracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TypeVehicleController : ControllerBase
    {
        private readonly ITypeVehicleService _typeVehicleService;

        public TypeVehicleController(ITypeVehicleService typeVehicleService)
        {
            _typeVehicleService = typeVehicleService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTypeVehicle([FromBody] TypeVehicleCreateRequest request)
        {
            try
            {
                var result = await _typeVehicleService.CreateTypeVehicleAsync(request);
                return CreatedAtAction(nameof(GetTypeVehicleById), new { id = result.TypeId }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTypeVehicleById(Guid id)
        {
            var typeVehicle = await _typeVehicleService.GetTypeVehicleByIdAsync(id);
            if (typeVehicle == null)
                return NotFound(new { message = "Type Vehicle not found" });

            return Ok(typeVehicle);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTypeVehicles()
        {
            var types = await _typeVehicleService.GetAllTypeVehiclesAsync();
            return Ok(types);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTypeVehicle(Guid id, [FromBody] TypeVehicleUpdateRequest request)
        {
            try
            {
                var result = await _typeVehicleService.UpdateTypeVehicleAsync(id, request);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Type Vehicle not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTypeVehicle(Guid id)
        {
            var result = await _typeVehicleService.DeleteTypeVehicleAsync(id);
            if (!result)
                return NotFound(new { message = "Type Vehicle not found" });

            return NoContent();
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        private readonly IBranchDestinationService _branchService;

        public BranchController(IBranchDestinationService branchService)
        {
            _branchService = branchService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBranch([FromBody] BranchDestinationCreateRequest request)
        {
            try
            {
                var result = await _branchService.CreateBranchAsync(request);
                return CreatedAtAction(nameof(GetBranchById), new { id = result.BranchId }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBranchById(Guid id)
        {
            var branch = await _branchService.GetBranchByIdAsync(id);
            if (branch == null)
                return NotFound(new { message = "Branch not found" });

            return Ok(branch);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBranches()
        {
            var branches = await _branchService.GetAllBranchesAsync();
            return Ok(branches);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBranch(Guid id, [FromBody] BranchDestinationUpdateRequest request)
        {
            try
            {
                var result = await _branchService.UpdateBranchAsync(id, request);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Branch not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBranch(Guid id)
        {
            var result = await _branchService.DeleteBranchAsync(id);
            if (!result)
                return NotFound(new { message = "Branch not found" });

            return NoContent();
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class MaintenanceController : ControllerBase
    {
        private readonly IMaintenanceRecordService _maintenanceService;

        public MaintenanceController(IMaintenanceRecordService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMaintenanceRecord([FromBody] MaintenanceRecordCreateRequest request)
        {
            try
            {
                var result = await _maintenanceService.CreateMaintenanceRecordAsync(request);
                return CreatedAtAction(nameof(GetMaintenanceRecordById), new { id = result.MaintenanceId }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMaintenanceRecordById(Guid id)
        {
            var maintenance = await _maintenanceService.GetMaintenanceRecordByIdAsync(id);
            if (maintenance == null)
                return NotFound(new { message = "Maintenance record not found" });

            return Ok(maintenance);
        }

        [HttpGet("vehicle/{vehicleId}")]
        public async Task<IActionResult> GetMaintenanceByVehicle(Guid vehicleId)
        {
            var records = await _maintenanceService.GetMaintenanceByVehicleAsync(vehicleId);
            return Ok(records);
        }

        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcomingMaintenance()
        {
            var records = await _maintenanceService.GetUpcomingMaintenanceAsync();
            return Ok(records);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMaintenanceRecord(Guid id, [FromBody] MaintenanceRecordUpdateRequest request)
        {
            try
            {
                var result = await _maintenanceService.UpdateMaintenanceRecordAsync(id, request);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Maintenance record not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaintenanceRecord(Guid id)
        {
            var result = await _maintenanceService.DeleteMaintenanceRecordAsync(id);
            if (!result)
                return NotFound(new { message = "Maintenance record not found" });

            return NoContent();
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class RelocationController : ControllerBase
    {
        private readonly IVehicleRelocationService _relocationService;

        public RelocationController(IVehicleRelocationService relocationService)
        {
            _relocationService = relocationService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRelocation([FromBody] VehicleRelocationCreateRequest request)
        {
            try
            {
                var result = await _relocationService.CreateRelocationAsync(request);
                return CreatedAtAction(nameof(GetRelocationById), new { id = result.RelocationId }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRelocationById(Guid id)
        {
            var relocation = await _relocationService.GetRelocationByIdAsync(id);
            if (relocation == null)
                return NotFound(new { message = "Relocation not found" });

            return Ok(relocation);
        }

        [HttpGet("vehicle/{vehicleId}")]
        public async Task<IActionResult> GetRelocationsByVehicle(Guid vehicleId)
        {
            var relocations = await _relocationService.GetRelocationsByVehicleAsync(vehicleId);
            return Ok(relocations);
        }

        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetRelocationsByStatus(string status)
        {
            var relocations = await _relocationService.GetRelocationsByStatusAsync(status);
            return Ok(relocations);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRelocation(Guid id, [FromBody] VehicleRelocationUpdateRequest request)
        {
            try
            {
                var result = await _relocationService.UpdateRelocationAsync(id, request);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Relocation not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteRelocation(Guid id)
        {
            try
            {
                var result = await _relocationService.CompleteRelocationAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Relocation not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRelocation(Guid id)
        {
            var result = await _relocationService.DeleteRelocationAsync(id);
            if (!result)
                return NotFound(new { message = "Relocation not found" });

            return NoContent();
        }
    }
}

