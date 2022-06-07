using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace GettingStarted.Diagnostic;

public class MassTransitDiagnosticObserver : IObserver<KeyValuePair<string, object>>
{
       
    private readonly Dictionary<string, Action<object>> _handlerSelectorDict;

    public MassTransitDiagnosticObserver()
    {
        _handlerSelectorDict = new Dictionary<string, Action<object>>()
        {
            {"MassTransit.Transport.Send.Start", TestAction},
            {"MassTransit.Transport.Send.Stop", TestAction},
            {"MassTransit.Consumer.Consume.Start", TestAction},
            {"MassTransit.Consumer.Consume.Stop", TestAction},
            {"MassTransit.Consumer.Handle.Start", TestAction},
            {"MassTransit.Consumer.Handle.Stop", TestAction}
        };
    }
    
    
    private void TestAction(object value)
    {
        Console.WriteLine($"Action ...{value}");
    }

    public void OnCompleted()
    {
        throw new NotImplementedException();
    }

    public void OnError(Exception error)
    {
        throw new NotImplementedException();
    }

    public void OnNext(KeyValuePair<string, object> value)
    {
        if (_handlerSelectorDict.TryGetValue(value.Key, out var action))
        {
            action.Invoke(value.Value);
        }
    }
}