using EV_StationRentalSystem.Core.DTO;
using EV_StationRentalSystem.Core.ServiceContracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateVehicle([FromBody] VehicleCreateRequest request)
        {
            try
            {
                var result = await _vehicleService.CreateVehicleAsync(request);
                return CreatedAtAction(nameof(GetVehicleById), new { id = result.VehicleId }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehicleById(Guid id)
        {
            var vehicle = await _vehicleService.GetVehicleByIdAsync(id);
            if (vehicle == null)
                return NotFound(new { message = "Vehicle not found" });

            return Ok(vehicle);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVehicles()
        {
            var vehicles = await _vehicleService.GetAllVehiclesAsync();
            return Ok(vehicles);
        }

        [HttpGet("type/{typeId}")]
        public async Task<IActionResult> GetVehiclesByType(Guid typeId)
        {
            var vehicles = await _vehicleService.GetVehiclesByTypeAsync(typeId);
            return Ok(vehicles);
        }

        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetVehiclesByStatus(string status)
        {
            var vehicles = await _vehicleService.GetVehiclesByStatusAsync(status);
            return Ok(vehicles);
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetVehicleStatusSummary()
        {
            var summary = await _vehicleService.GetVehicleStatusSummaryAsync();
            return Ok(summary);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVehicle(Guid id, [FromBody] VehicleUpdateRequest request)
        {
            try
            {
                var result = await _vehicleService.UpdateVehicleAsync(id, request);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Vehicle not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicle(Guid id)
        {
            var result = await _vehicleService.DeleteVehicleAsync(id);
            if (!result)
                return NotFound(new { message = "Vehicle not found" });

            return NoContent();
        }
    }
}

