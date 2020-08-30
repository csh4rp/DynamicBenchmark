namespace DynamicBenchmark
{
    public class Handler : IHandler
    {
        public void Handle(Item item)
        {
            item.Number++;
        }
    }
}