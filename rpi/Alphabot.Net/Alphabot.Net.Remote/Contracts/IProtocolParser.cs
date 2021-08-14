namespace Alphabot.Net.Remote.Contracts
{
    public interface IProtocolParser
    {
        string Request { get; }
        string GetCommand();
        string[] GetArgs();

    }
}
