using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace GettingStarted.Diagnostic;

public class DiagnosticObserver : IHostedService, IObserver<DiagnosticListener>
{
    private readonly MassTransitDiagnosticObserver _massTransitDiagnosticObserver;

    private IDisposable _mainSubscription = null;
    private IDisposable _massTransitSubscription = null;
    
    public DiagnosticObserver(MassTransitDiagnosticObserver massTransitDiagnosticObserver)
    {
        _massTransitDiagnosticObserver = massTransitDiagnosticObserver ?? throw new ArgumentNullException(nameof(massTransitDiagnosticObserver));
    }
    
    public void OnCompleted() { }

    public void OnError(Exception error) { }

    public void OnNext(DiagnosticListener value)
    {
        if (value.Name == "MassTransit")
        {
            // subscribe to the listener with your monitoring tool, etc.
            _massTransitSubscription = value.Subscribe(_massTransitDiagnosticObserver);
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _mainSubscription = DiagnosticListener.AllListeners.Subscribe(this);
        //_mainSubscription = DiagnosticListener.AllListeners.Subscribe(new DiagnosticObserver(_massTransitDiagnosticObserver));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _massTransitSubscription?.Dispose();
        _mainSubscription?.Dispose();

        return Task.CompletedTask;
    }
}