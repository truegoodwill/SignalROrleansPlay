namespace SignalROrleansPlay
{
  [Immutable, GenerateSerializer]
  internal struct OrleansSignalRMessage
  {
    [Id(0)]
    public string Method { get; private set; }

    [Id(1)]
    public object?[] Args { get; private set; }

    [Id(2)]
    public bool IsToAllConnections { get; private set; }

    [Id(3)]
    public string? ConnectionId { get; private set; }

    [Id(4)]
    public IReadOnlyList<string>? ConnectionIds { get; private set; }

    [Id(5)]
    public string? GroupName { get; private set; }

    [Id(6)]
    public IReadOnlyList<string>? GroupNames { get; private set; }

    [Id(7)]
    public string? UserId { get; private set; }

    [Id(8)]
    public IReadOnlyList<string>? UserIds { get; private set; }

    [Id(9)]
    public IReadOnlyList<string>? ExcludedConnectionIds { get; private set; }

    internal static OrleansSignalRMessage Create_AllConnections(string method, object?[] args, IReadOnlyList<string>? excludedConnectionIds = null) => new()
    {
      Method = method,
      Args = args,
      IsToAllConnections = true,
      ExcludedConnectionIds = excludedConnectionIds,
    };

    internal static OrleansSignalRMessage Create_ConnectionId(string connectionId, string method, object?[] args) => new()
    {
      Method = method,
      Args = args,
      ConnectionId = connectionId,
    };

    internal static OrleansSignalRMessage Create_ConnectionIds(IReadOnlyList<string> connectionIds, string method, object?[] args) => new()
    {
      Method = method,
      Args = args,
      ConnectionIds = connectionIds,
    };

    internal static OrleansSignalRMessage Create_UserId(string userId, string method, object?[] args) => new()
    {
      Method = method,
      Args = args,
      UserId = userId,
    };

    internal static OrleansSignalRMessage Create_UserIds(IReadOnlyList<string> userIds, string method, object?[] args) => new()
    {
      Method = method,
      Args = args,
      UserIds = userIds,
    };

    internal static OrleansSignalRMessage Create_Group(string groupName, string method, object?[] args, IReadOnlyList<string>? excludedConnectionIds = null) => new()
    {
      Method = method,
      Args = args,
      GroupName = groupName,
      ExcludedConnectionIds = excludedConnectionIds,
    };

    internal static OrleansSignalRMessage Create_Groups(IReadOnlyList<string> groupNames, string method, object?[] args) => new()
    {
      Method = method,
      Args = args,
      GroupNames = groupNames,
    };
  }
}
