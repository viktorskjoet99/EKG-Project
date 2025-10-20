using System;
using System.Device.Spi;
using Iot.Device.Adc;
using Iot.Device.Adc;

namespace DefaultNamespace;

public class RaspberryPi
{
	private readonly Mcp3208 _adc;
    
	public RaspberryPi()
	{
		SpiDevice device;
		Mcp3208 adc = new Mcp3208(device);
		
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
		var channel = _adc.CreateSingleEndedChannel(0);
		int raw = channel.ReadValue();
		double voltage = channel.ReadVoltage();
    	Console.WriteLine($"Raw: {raw}, Voltage: {voltage:F4} V");
    	return voltage;
	}

}