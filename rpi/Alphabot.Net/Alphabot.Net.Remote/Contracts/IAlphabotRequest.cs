namespace Alphabot.Net.Remote.Contracts
{
    public interface IAlphabotRequest
    {
        // string Command { get; }
        // string[] Args { get;}
        string ActionResult { get; }
        void PerformAction();
    }
}
