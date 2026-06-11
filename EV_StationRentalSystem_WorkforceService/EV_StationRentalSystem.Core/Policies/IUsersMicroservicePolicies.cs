using Polly;

namespace EV_StationRentalSystem.Core.Policies;

public interface IUsersMicroservicePolicies
{
  IAsyncPolicy<HttpResponseMessage> GetCombinedPolicy();
}
