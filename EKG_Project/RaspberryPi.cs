using System.Device.Gpio;
using System.Device.Spi;
using Iot.Device.Mcp3xxx;
using Iot.Device.Mcp3xxx.Mcp32080;

namespace DefaultNamespace;

public class RaspberryPi
{
	private readonly Mcp3208 _adc;
    
	public RaspberryPi()
	{
		var settings = new SpiConnectionSettings(0, 0) // Hvad vil vælger på GPIO og MCP3208
        {
            ClockFrequency = 1_000_000, // 1 MHz
            Mode = SpiMode.Mode0
        };

        var spi = SpiDevice.Create(settings);
        _adc = new Mcp3208(spi);
	}
	
	public double RawVoltage()
	{
		var channel = _adc.createSingleEndedChannel(0);
		int raw = channel.ReadValue();
		double voltage = channel.ReadVoltage();
    	Console.WriteLine($"Raw: {raw}, Voltage: {voltage:F4} V");
    	return voltage;
	}

}