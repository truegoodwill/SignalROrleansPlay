using Microsoft.AspNetCore.SignalR;

namespace SignalROrleansPlay
{
  internal class SampleGrain : Grain, ISampleGrain
  {
    private readonly IHubContext<MyHub> _hub;

    public SampleGrain(IHubContext<MyHub> hub)
    {
      _hub = hub;
    }

    public async Task SayHello(string fromWhom)
    {
      await _hub.Clients.All.SendAsync("SayHello", fromWhom);
    }
  }
}
