namespace Pingring.Barker.Web.HostedServices;

public interface IGpioMonitor
{
    void AddPin(int pin, bool usePullup, Action<int, bool> gpioDigitalEvent);
    void Start();
    void Stop();
}