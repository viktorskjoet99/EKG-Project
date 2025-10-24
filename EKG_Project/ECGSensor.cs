using System;
using System.Collections.Concurrent;
using System.Device.Spi;
using System.Threading.Channels;
using EKG_Project;
using Iot.Device.Adc;


namespace DefaultNamespace;

public class ECGSensor
{
	private readonly Mcp3208 _adc;
	private readonly SpiDevice _spi;
	
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

	public short ReadRawSample()
	{
		Span<byte> buffer = stackalloc byte[3] {0x06, 0x00, 0x00};
		Span<byte> buffer2 = stackalloc byte[3];
		_spi.TransferFullDuplex(buffer, buffer2);
		
		int value = ((buffer2[1] & 0x0F) << 8 | buffer2[2]);
		return (short)(value - 2048);
	}
	
	

	
	
	
	
	

}