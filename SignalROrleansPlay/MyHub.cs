using Microsoft.AspNetCore.SignalR;

namespace SignalROrleansPlay
{
  public class MyHub : Hub
  {
    public async Task<string> Hello(string message)
    {
      return "Hello " + message;
    }
  }
}
