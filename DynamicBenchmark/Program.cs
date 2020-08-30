using System;

namespace DynamicBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting benchmark...");
            Console.WriteLine("Type 'w' - to run warmup, 't' - to tun test, 'e' - to exit");
            Console.Write("Press key... ");
            
            var testRunner = new TestRunner();

            char key;
            while ((key = Console.ReadKey().KeyChar) != 'e')
            {
                switch (key)
                {
                    case 'w':
                        Console.Write("\n");
                        testRunner.RunWarmup();
                        break;
                    case 't':
                        Console.Write("\n");
                        testRunner.Run();
                        break;
                }
                
                Console.WriteLine("\nType 'w' - to run warmup, 't' - to tun test, 'e' - to exit");
                Console.Write("Press key... ");
            }
            
            Console.WriteLine("Finished");
        }
    }
}