using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging.Abstractions;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;

namespace SignalROrleansPlay
{
  public class OrleansHubLifetimeManager<THub> : DefaultHubLifetimeManager<THub> where THub : Hub
  {
    private readonly IClusterClient _clusterClient;
    private readonly SemaphoreSlim _streamSetup = new(1);

    private IAsyncStream<OrleansSignalRMessage> _stream = default!;

    public OrleansHubLifetimeManager(IClusterClient clusterClient, ILogger<DefaultHubLifetimeManager<THub>> logger) : base(logger)
    {
      _clusterClient = clusterClient;
    }

    private async Task SetupStream()
    {
      if (_stream is not null)
        return;

      await _streamSetup.WaitAsync();

      if (_stream is not null)
        return;

      try
      {
        var hubName = typeof(THub).Name;
        if (typeof(THub).IsInterface && hubName[0] == 'I')
          hubName = hubName.Substring(1);

        _stream = _clusterClient.GetStreamProvider("OrleansSignalR").GetStream<OrleansSignalRMessage>(hubName);

        await _stream.SubscribeAsync(async (msg, _) =>
        {
          if (msg.IsToAllConnections)
          {
            if (msg.ExcludedConnectionIds is not null)
            {
              await base.SendAllExceptAsync(msg.Method, msg.Args, msg.ExcludedConnectionIds);
            }
            else
            {
              await base.SendAllAsync(msg.Method, msg.Args);
            }
          }
          else if (msg.ConnectionId is not null)
          {
            await base.SendConnectionAsync(msg.ConnectionId, msg.Method, msg.Args);
          }
          else if (msg.ConnectionIds is not null)
          {
            await base.SendConnectionsAsync(msg.ConnectionIds, msg.Method, msg.Args);
          }
          else if (msg.GroupName is not null)
          {
            if (msg.ExcludedConnectionIds is not null)
            {
              await base.SendGroupExceptAsync(msg.GroupName, msg.Method, msg.Args, msg.ExcludedConnectionIds);
            }
            else
            {
              await base.SendGroupAsync(msg.GroupName, msg.Method, msg.Args);
            }
          }
          else if (msg.GroupNames is not null)
          {
            await base.SendGroupsAsync(msg.GroupNames, msg.Method, msg.Args);
          }
          else if (msg.UserId is not null)
          {
            await base.SendUserAsync(msg.UserId, msg.Method, msg.Args);
          }
          else if (msg.UserIds is not null)
          {
            await base.SendUsersAsync(msg.UserIds, msg.Method, msg.Args);
          }
        });
      }
      finally
      {
        _streamSetup.Release();
      }
    }

    public override async Task OnConnectedAsync(HubConnectionContext connection)
    {
      await SetupStream();
      await base.OnConnectedAsync(connection);
    }

    public override Task SendAllAsync(string methodName, object?[] args, CancellationToken cancellationToken = default)
      => _stream.OnNextAsync(OrleansSignalRMessage.Create_AllConnections(methodName, args));

    public override Task SendAllExceptAsync(string methodName, object?[] args, IReadOnlyList<string> excludedConnectionIds, CancellationToken cancellationToken = default)
      => _stream.OnNextAsync(OrleansSignalRMessage.Create_AllConnections(methodName, args, excludedConnectionIds));

    public override Task SendConnectionAsync(string connectionId, string methodName, object?[] args, CancellationToken cancellationToken = default)
      => _stream.OnNextAsync(OrleansSignalRMessage.Create_ConnectionId(connectionId, methodName, args));

    public override Task SendConnectionsAsync(IReadOnlyList<string> connectionIds, string methodName, object?[] args, CancellationToken cancellationToken = default)
      => _stream.OnNextAsync(OrleansSignalRMessage.Create_ConnectionIds(connectionIds, methodName, args));

    public override Task SendGroupAsync(string groupName, string methodName, object?[] args, CancellationToken cancellationToken = default)
      => _stream.OnNextAsync(OrleansSignalRMessage.Create_Group(groupName, methodName, args));

    public override Task SendGroupExceptAsync(string groupName, string methodName, object?[] args, IReadOnlyList<string> excludedConnectionIds, CancellationToken cancellationToken = default)
      => _stream.OnNextAsync(OrleansSignalRMessage.Create_Group(groupName, methodName, args, excludedConnectionIds));

    public override Task SendGroupsAsync(IReadOnlyList<string> groupNames, string methodName, object?[] args, CancellationToken cancellationToken = default)
      => _stream.OnNextAsync(OrleansSignalRMessage.Create_Groups(groupNames, methodName, args));

    public override Task SendUserAsync(string userId, string methodName, object?[] args, CancellationToken cancellationToken = default)
      => _stream.OnNextAsync(OrleansSignalRMessage.Create_UserId(userId, methodName, args));

    public override Task SendUsersAsync(IReadOnlyList<string> userIds, string methodName, object?[] args, CancellationToken cancellationToken = default)
      => _stream.OnNextAsync(OrleansSignalRMessage.Create_UserIds(userIds, methodName, args));

    // This base class of the base class default implementation throws NotImplementedException
    public override Task<T> InvokeConnectionAsync<T>(string connectionId, string methodName, object?[] args, CancellationToken cancellationToken)
      => ((HubLifetimeManager<THub>)this).InvokeConnectionAsync<T>(connectionId, methodName, args, cancellationToken);
  }
}
