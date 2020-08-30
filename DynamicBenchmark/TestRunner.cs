using System;
using System.Diagnostics;
using System.Reflection;

namespace DynamicBenchmark
{
    public class TestRunner
    {
        private const int NumberOfRuns = 1000000;
        private static readonly Item Item = new Item();
        private static readonly IHandler Handler = new Handler();

        public void Run()
        {
            RunReflection();
            Clear();
            
            RunReflectionWithCachedMethod();
            Clear();
            
            RunDynamic();
            Clear();
            
            RunDefault();
            Clear();
            
            RunDelegate();
            Clear();
        }
        
        public void RunWarmup()
        {
            RunReflectionWarmup();
            RunDynamicWarmup();
            RunDefaultWarmup();
            RunDelegateWarmup();
            Clear();
        }

        private static void Clear() => Item.Number = 0;

        private static void RunReflection()
        {
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < NumberOfRuns; i++)
            {
                var method = Handler.GetType()
                    .GetMethod(nameof(IHandler.Handle), BindingFlags.Public | BindingFlags.Instance);
                method.Invoke(Handler, new[] {Item});
            }
            sw.Stop();
            Console.WriteLine($"Reflection: {sw.ElapsedMilliseconds}ms");
        }
        
        private static void RunReflectionWarmup()
        {
            var method = Handler.GetType()
                .GetMethod(nameof(IHandler.Handle), BindingFlags.Public | BindingFlags.Instance);
            method.Invoke(Handler, new[] {Item});
        }

        private static void RunReflectionWithCachedMethod()
        {
            var method = Handler.GetType()
                .GetMethod(nameof(IHandler.Handle), BindingFlags.Public | BindingFlags.Instance);
            
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < NumberOfRuns; i++)
            {
                method.Invoke(Handler, new[] {Item});
            }
            sw.Stop();
            Console.WriteLine($"Reflection with cached method: {sw.ElapsedMilliseconds}ms");
        }
        
        private static void RunDynamic()
        {
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < NumberOfRuns; i++)
            {
                ((dynamic) Handler).Handle(Item);
            }
            sw.Stop();
            Console.WriteLine($"Dynamic: {sw.ElapsedMilliseconds}ms");
        }
        
        private static void RunDynamicWarmup()
        {
            ((dynamic) Handler).Handle(Item);
        }

        private static void RunDefault()
        {
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < NumberOfRuns; i++)
            {
                Handler.Handle(Item);
            }
            sw.Stop();
            Console.WriteLine($"Default: {sw.ElapsedMilliseconds}ms");
        }
        
        private static void RunDefaultWarmup()
        {
            Handler.Handle(Item);
        }

        private static void RunDelegate()
        {
            var method = Handler.GetType()
                .GetMethod(nameof(IHandler.Handle), BindingFlags.Public | BindingFlags.Instance);
            var delegateType = typeof(Action<,>).MakeGenericType(typeof(Handler), typeof(Item));
            var @delegate = method.CreateDelegate(delegateType);
            var helperMethod = typeof(TestRunner)
                .GetMethod(nameof(CallDelegate), BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(typeof(Handler), typeof(Item));
            var delegateCache = (Action<object, object>) helperMethod.Invoke(NumberOfRuns, new[] {@delegate});

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < NumberOfRuns; i++)
            {
                delegateCache(Handler, Item);
            }
            sw.Stop();
            Console.WriteLine($"Delegate: {sw.ElapsedMilliseconds}ms");
        }
        
        private static void RunDelegateWarmup()
        {
            var method = Handler.GetType()
                .GetMethod(nameof(IHandler.Handle), BindingFlags.Public | BindingFlags.Instance);
            var delegateType = typeof(Action<,>).MakeGenericType(typeof(Handler), typeof(Item));
            var @delegate = method.CreateDelegate(delegateType);
            var helperMethod = typeof(TestRunner)
                .GetMethod(nameof(CallDelegate), BindingFlags.Static | BindingFlags.NonPublic)
                .MakeGenericMethod(typeof(Handler), typeof(Item));
            var delegateCache = (Action<object, object>) helperMethod.Invoke(NumberOfRuns, new[] {@delegate});
            
            delegateCache(Handler, Item);
        }

        private static Action<object, object> CallDelegate<TClass, TItem>(Action<TClass, TItem> action) =>
            (instance, item) => action((TClass)instance, (TItem)item);

    }
}