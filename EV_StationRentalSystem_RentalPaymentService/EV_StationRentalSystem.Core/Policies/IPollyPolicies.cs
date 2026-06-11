using Polly;

namespace EV_StationRentalSystem.Core.Policies;

public interface IPollyPolicies
{
  IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount);
  IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(int handledEventsAllowedBeforeBreaking, TimeSpan durationOfBreak);
  IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy(TimeSpan timeout);
}
