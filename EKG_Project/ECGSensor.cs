using System;
using System.Collections.Concurrent;
using System.Device.Spi;
using System.Threading.Channels;
using EKG_Project;
using Iot.Device.Adc;
using Iot.Device.Adc;

namespace DefaultNamespace;

public class ECGSensor
{
	private readonly Mcp3208 _adc;
	
	public ECGSensor()
	{
		var settings = new SpiConnectionSettings(0, 0) // Hvad vil vælger på GPIO og MCP3208
        {
            ClockFrequency = 1_000_000, // 1 MHz
            Mode = SpiMode.Mode0
        }; 
		var spi = SpiDevice.Create(settings);
		_adc = new Mcp3208(spi);
	}
	
	

	
	
	
	
	

}