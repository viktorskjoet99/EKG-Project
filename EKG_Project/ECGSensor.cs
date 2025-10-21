using System;
using System.Collections.Concurrent;
using System.Device.Spi;
using System.Threading.Channels;
using Iot.Device.Adc;
using Iot.Device.Adc;

namespace DefaultNamespace;

public class ECGSensor
{
	private readonly Mcp3208 _adc;
	private readonly BlockingCollection<DataChunks> _dataqueue;
	private readonly int _samplerateHz;
	private readonly int _chunksecond;
	private Thread _thread;
	private CancellationTokenSource _cts;
	
    
	public ECGSensor(BlockingCollection<DataChunks> dataqueue, int samplerateHz, int chunksecond)
	{
		_dataqueue = dataqueue;
		_samplerateHz = samplerateHz;
		_chunksecond = chunksecond;
		
		var settings = new SpiConnectionSettings(0, 0) // Hvad vil vælger på GPIO og MCP3208
        {
            ClockFrequency = 1_000_000, // 1 MHz
            Mode = SpiMode.Mode0
        }; 
		var spi = SpiDevice.Create(settings);
		_adc = new Mcp3208(spi);
	}

	public void Start()
	{
		if (_thread != null) return;
		_thread = new Thread(() => Run())
		{
			IsBackground = true,
			Name = "ECGSensorProducer"
		};
		_thread.Start();
	}

	public void Stop()
	{
		if (_thread == null) return;
		_cts.Cancel();
		_thread.Join();
		
		_thread = null;
		_cts.Dispose();
		_cts = null;
	}
	
	public void Run()
	{
		while (true)
		{
			
		}
	}

}