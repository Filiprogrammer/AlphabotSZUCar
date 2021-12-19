namespace Alphabot.Net.Remote.Contracts
{
    public interface IAlphabotAction
    {
        int[] Args { get; set; }
        void Perform();
    }
}
