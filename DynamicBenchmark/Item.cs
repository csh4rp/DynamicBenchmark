using System;

namespace DynamicBenchmark
{
    public class Item
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int Number { get; set; } 
    }
}