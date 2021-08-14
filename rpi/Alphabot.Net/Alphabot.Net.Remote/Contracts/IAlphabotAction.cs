namespace Alphabot.Net.Remote.Contracts
{
    public interface IAlphabotAction
    {
        string Command { get; }
        string[] Args { get;}
        string ActionResult { get; }
        void Perform();
    }
}
