using System;
using System.Collections.Concurrent;
using System.Device.Spi;
using System.Threading.Channels;
using EKG_Project;
using Iot.Device.Adc;


namespace EKG_Project;

public class ECGSensor : IECGSensor
{
	private readonly Mcp3208 _adc;
	private readonly SpiDevice _spi;

	public bool IsFinished => false;
	
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

	public double ReadRawSample()
	{
		int raw = _adc.Read(channel: 0); 
		
		// Konverter til volt
		double voltage = (raw / 4095.0) * 5.0;
		return voltage;
	}
	
	

	
	
	
	
	

}