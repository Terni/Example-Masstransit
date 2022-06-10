using System;
using System.Threading.Tasks;
using MassTransit;

namespace GettingStarted.Diagnostic;

public class ConsumeObserver : IConsumeObserver
{    
    Task IConsumeObserver.PreConsume<T>(ConsumeContext<T> context)
    {
        // called before the consumer's Consume method is called
        Console.WriteLine($"Pre Consume ...{context}");

        return Task.CompletedTask;
    }

    Task IConsumeObserver.PostConsume<T>(ConsumeContext<T> context)
    {
        // called after the consumer's Consume method is called
        // if an exception was thrown, the ConsumeFault method is called instead
        
        Console.WriteLine($"Post Consume ...{context}");
        
        return Task.CompletedTask;
    }

    Task IConsumeObserver.ConsumeFault<T>(ConsumeContext<T> context, Exception exception)
    {
        // called if the consumer's Consume method throws an exception
        
        return Task.CompletedTask;
    }
}