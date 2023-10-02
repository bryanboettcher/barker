using System.Device.Gpio;

namespace Pingring.Barker.Web.HostedServices;

public class RpiGpioMonitor : IGpioMonitor, IDisposable
{
    private readonly ILogger<RpiGpioMonitor> _logger;
    private readonly GpioController _controller;
    private readonly Dictionary<int, (bool usePullup, Action<int, bool> callback)> _listeningPins = new();
    private readonly CancellationTokenSource _cts = new();

    private Task? _monitorTask;

    public RpiGpioMonitor(ILogger<RpiGpioMonitor> logger)
    {
        _logger = logger;
        _controller = new GpioController();

        logger.LogInformation("Using RpiGpioMonitor");
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

        foreach (var kvp in _listeningPins)
        {
            _controller.OpenPin(
                kvp.Key, 
                kvp.Value.usePullup
                    ? PinMode.InputPullUp
                    : PinMode.Input
            );

            _controller.RegisterCallbackForPinValueChangedEvent(
                kvp.Key,
                PinEventTypes.Rising,
                (s, a) =>
                {
                    _logger.LogInformation("Pin rise detected on {pin}", a.PinNumber);
                    kvp.Value.callback(a.PinNumber, (a.ChangeType & PinEventTypes.Rising) != 0);
                });
        }

        _monitorTask = Task.Run(async () =>
        {
            await Task.Delay(Timeout.Infinite, token);
            _logger.LogInformation("Shutting down GPIO monitor");
            foreach(var (k, v) in _listeningPins)
                _controller.ClosePin(k);
        }, CancellationToken.None);
    }

    public void Stop() => _cts.Cancel();

    public void Dispose() => _controller.Dispose();
}