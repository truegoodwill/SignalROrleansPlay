namespace SignalROrleansPlay
{
  internal interface ISampleGrain : IGrainWithStringKey
  {
    Task SayHello(string fromWhom);
  }
}
