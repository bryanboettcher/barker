using Microsoft.Extensions.Logging;

namespace Pingring.Barker.Web.HostedServices;

public class FakeGpioMonitor : IGpioMonitor
{
    private readonly ILogger<FakeGpioMonitor> _logger;
    private Task? _monitorTask;
    private readonly Dictionary<int, (bool usePullup, Action<int, bool> callback)> _listeningPins = new();
    private readonly CancellationTokenSource _cts = new();

    public FakeGpioMonitor(ILogger<FakeGpioMonitor> logger)
    {
        _logger = logger;
        logger.LogInformation("Using FakeGpioMonitor");
    }

    public void AddPin(int pin, bool usePullup, Action<int, bool> gpioDigitalEvent)
    {
        if (_monitorTask is not null)
            throw new InvalidOperationException("Cannot add pins to monitor after monitoring has started");

        _listeningPins.Add(pin, (usePullup, gpioDigitalEvent));
    }

    public void Start()
    {
        var token = _cts.Token;
        _monitorTask = Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(2500 + Random.Shared.Next(5000), token);
                var triggeredPin = _listeningPins.MinBy(_ => Random.Shared.NextInt64());
                triggeredPin.Value.callback.Invoke(triggeredPin.Key, Random.Shared.Next(0, 10) < 5);
            }
        }, token);
    }

    public void Stop() => _cts.Cancel();
}