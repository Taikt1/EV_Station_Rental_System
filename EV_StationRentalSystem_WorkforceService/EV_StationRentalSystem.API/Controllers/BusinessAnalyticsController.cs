using EV_StationRentalSystem.Core.DTO;
using EV_StationRentalSystem.Core.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.API.Controllers
{
    [Route("Workforce/BusinessAnalytics")]
    [ApiController]
    public class BusinessAnalyticsController : ControllerBase
    {
        private readonly IBusinessAnalyticsService _analyticsService;

        public BusinessAnalyticsController(IBusinessAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        /// <summary>
        /// Get comprehensive revenue analytics
        /// GET: api/BusinessAnalytics/revenue
        /// </summary>
        [HttpGet("revenue")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRevenueAnalytics([FromQuery] AnalyticsFilterRequest filter)
        {
            try
            {
                var gatewayHeaders = ExtractGatewayHeaders();
                var userRole = gatewayHeaders.ContainsKey("X-User-Role") ? gatewayHeaders["X-User-Role"] : null;

                // Only Manager can access analytics
                if (userRole?.ToLower() != "manager")
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Only Manager can access analytics"
                    });
                }

                var result = await _analyticsService.GetRevenueAnalyticsAsync(filter);

                return Ok(new
                {
                    success = true,
                    message = "Revenue analytics retrieved successfully",
                    filters = filter,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error retrieving revenue analytics",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Get vehicle utilization analytics
        /// GET: api/BusinessAnalytics/vehicle-utilization
        /// </summary>
        [HttpGet("vehicle-utilization")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetVehicleUtilizationAnalytics([FromQuery] AnalyticsFilterRequest filter)
        {
            try
            {
                var gatewayHeaders = ExtractGatewayHeaders();
                var userRole = gatewayHeaders.ContainsKey("X-User-Role") ? gatewayHeaders["X-User-Role"] : null;

                // Only Manager can access analytics
                if (userRole?.ToLower() != "manager")
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Only Manager can access analytics"
                    });
                }

                var result = await _analyticsService.GetVehicleUtilizationAnalyticsAsync(filter);

                return Ok(new
                {
                    success = true,
                    message = "Vehicle utilization analytics retrieved successfully",
                    filters = filter,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error retrieving vehicle utilization analytics",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Get customer analytics
        /// GET: api/BusinessAnalytics/customer
        /// </summary>
        [HttpGet("customer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCustomerAnalytics([FromQuery] AnalyticsFilterRequest filter)
        {
            try
            {
                var gatewayHeaders = ExtractGatewayHeaders();
                var userRole = gatewayHeaders.ContainsKey("X-User-Role") ? gatewayHeaders["X-User-Role"] : null;

                // Only Manager can access analytics
                if (userRole?.ToLower() != "manager")
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Only Manager can access analytics"
                    });
                }

                var result = await _analyticsService.GetCustomerAnalyticsAsync(filter);

                return Ok(new
                {
                    success = true,
                    message = "Customer analytics retrieved successfully",
                    filters = filter,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error retrieving customer analytics",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Get operational metrics
        /// GET: api/BusinessAnalytics/operational
        /// </summary>
        [HttpGet("operational")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetOperationalMetrics([FromQuery] AnalyticsFilterRequest filter)
        {
            try
            {
                var gatewayHeaders = ExtractGatewayHeaders();
                var userRole = gatewayHeaders.ContainsKey("X-User-Role") ? gatewayHeaders["X-User-Role"] : null;

                // Only Manager can access analytics
                if (userRole?.ToLower() != "manager")
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Only Manager can access analytics"
                    });
                }

                var result = await _analyticsService.GetOperationalMetricsAsync(filter);

                return Ok(new
                {
                    success = true,
                    message = "Operational metrics retrieved successfully",
                    filters = filter,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error retrieving operational metrics",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Get complete business dashboard with all analytics
        /// GET: api/BusinessAnalytics/dashboard
        /// </summary>
        [HttpGet("dashboard")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBusinessDashboard([FromQuery] AnalyticsFilterRequest filter)
        {
            try
            {
                var gatewayHeaders = ExtractGatewayHeaders();
                var userRole = gatewayHeaders.ContainsKey("X-User-Role") ? gatewayHeaders["X-User-Role"] : null;
                var userName = gatewayHeaders.ContainsKey("X-User-Name") ? gatewayHeaders["X-User-Name"] : "Unknown";

                // Only Manager can access analytics
                if (userRole?.ToLower() != "manager")
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Only Manager can access business dashboard"
                    });
                }

                var result = await _analyticsService.GetBusinessDashboardAsync(filter);

                return Ok(new
                {
                    success = true,
                    message = $"Business dashboard retrieved successfully for {userName}",
                    filters = filter,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error retrieving business dashboard",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Get period comparison analytics
        /// GET: api/BusinessAnalytics/comparison
        /// </summary>
        [HttpGet("comparison")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPeriodComparison(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var gatewayHeaders = ExtractGatewayHeaders();
                var userRole = gatewayHeaders.ContainsKey("X-User-Role") ? gatewayHeaders["X-User-Role"] : null;

                // Only Manager can access analytics
                if (userRole?.ToLower() != "manager")
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Only Manager can access analytics"
                    });
                }

                if (!startDate.HasValue || !endDate.HasValue)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Start date and end date are required"
                    });
                }

                if (startDate.Value >= endDate.Value)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Start date must be before end date"
                    });
                }

                var result = await _analyticsService.GetPeriodComparisonAsync(startDate.Value, endDate.Value);

                return Ok(new
                {
                    success = true,
                    message = "Period comparison retrieved successfully",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error retrieving period comparison",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Get quick summary metrics for dashboard cards
        /// GET: api/BusinessAnalytics/summary
        /// </summary>
        [HttpGet("summary")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetQuickSummary([FromQuery] AnalyticsFilterRequest filter)
        {
            try
            {
                var gatewayHeaders = ExtractGatewayHeaders();
                var userRole = gatewayHeaders.ContainsKey("X-User-Role") ? gatewayHeaders["X-User-Role"] : null;

                // Only Manager and Staff can access summary
                if (userRole?.ToLower() != "manager" && userRole?.ToLower() != "staff")
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Only Manager and Staff can access summary"
                    });
                }

                var revenue = await _analyticsService.GetRevenueAnalyticsAsync(filter);
                var utilization = await _analyticsService.GetVehicleUtilizationAnalyticsAsync(filter);
                var customer = await _analyticsService.GetCustomerAnalyticsAsync(filter);

                var summary = new
                {
                    totalRevenue = revenue.TotalRevenue,
                    totalRentals = revenue.TotalRentals,
                    activeRentals = revenue.ActiveRentals,
                    completedRentals = revenue.CompletedRentals,
                    averageRevenuePerRental = revenue.AverageRevenuePerRental,
                    totalVehicles = utilization.TotalVehicles,
                    availableVehicles = utilization.AvailableVehicles,
                    utilizationRate = utilization.OverallUtilizationRate,
                    totalCustomers = customer.TotalCustomers,
                    activeCustomers = customer.ActiveCustomers,
                    newCustomers = customer.NewCustomersInPeriod
                };

                return Ok(new
                {
                    success = true,
                    message = "Quick summary retrieved successfully",
                    filters = filter,
                    data = summary
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Error retrieving quick summary",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Helper method to extract Gateway headers
        /// </summary>
        private Dictionary<string, string> ExtractGatewayHeaders()
        {
            var headers = new Dictionary<string, string>();

            if (Request.Headers.ContainsKey("X-User-Id"))
                headers["X-User-Id"] = Request.Headers["X-User-Id"].ToString();

            if (Request.Headers.ContainsKey("X-User-Email"))
                headers["X-User-Email"] = Request.Headers["X-User-Email"].ToString();

            if (Request.Headers.ContainsKey("X-User-Role"))
                headers["X-User-Role"] = Request.Headers["X-User-Role"].ToString();

            if (Request.Headers.ContainsKey("X-User-Name"))
                headers["X-User-Name"] = Request.Headers["X-User-Name"].ToString();

            return headers;
        }
    }
}
