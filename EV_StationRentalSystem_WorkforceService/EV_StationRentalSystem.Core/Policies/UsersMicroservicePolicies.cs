using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;
using Polly.Wrap;
using System.Text;
using System.Text.Json;

namespace EV_StationRentalSystem.Core.Policies;

public class UsersMicroservicePolicies : IUsersMicroservicePolicies
{
  private readonly ILogger<UsersMicroservicePolicies> _logger;
  private readonly IPollyPolicies _pollyPolicies;

  public UsersMicroservicePolicies(ILogger<UsersMicroservicePolicies> logger, IPollyPolicies pollyPolicies)
  {
    _logger = logger;
    _pollyPolicies = pollyPolicies;
  }

  public IAsyncPolicy<HttpResponseMessage> GetCombinedPolicy()
  {
    var retryPolicy = _pollyPolicies.GetRetryPolicy(3);
    var circuitBreakerPolicy = _pollyPolicies.GetCircuitBreakerPolicy(5, TimeSpan.FromMinutes(1));
    var timeoutPolicy = _pollyPolicies.GetTimeoutPolicy(TimeSpan.FromSeconds(30)); // Tăng timeout lên 30 giây

    AsyncPolicyWrap<HttpResponseMessage> wrappedPolicy = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy, timeoutPolicy);
    return wrappedPolicy;
  }
}
