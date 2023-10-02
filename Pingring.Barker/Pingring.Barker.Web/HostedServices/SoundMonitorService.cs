namespace Pingring.Barker.Web.HostedServices;

public class SoundMonitorService : BackgroundService
{
    private readonly ILogger<SoundMonitorService> _logger;
    private readonly IGpioMonitor _gpioMonitor;

    public SoundMonitorService(ILogger<SoundMonitorService> logger, IGpioMonitor gpioMonitor)
    {
        _logger = logger;
        _gpioMonitor = gpioMonitor;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Monitor service starting up");
        _gpioMonitor.AddPin(15, true, (pin, state) => _logger.LogInformation("Pin {pin} went to state {state}", pin, (state ? "high" : "low")));

        _gpioMonitor.Start();

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Waiting for GPIO toggle");
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        _gpioMonitor.Stop();

        _logger.LogInformation("Shutting down ...");
    }
}