using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TestUtilities.RabbitMqPingPong
{
    public static class Tools
    {
        private const long DefaultTimeoutMs = 5000;
        private const long DefaultCycleDelayMs = 100;
        
        public static readonly TimeSpan DefaultTimeout = TimeSpan.FromMilliseconds(DefaultTimeoutMs);
        
        public static async Task WaitUntilSuccess(Func<Task> successFunc, 
            TimeSpan timeout = default(TimeSpan), 
            TimeSpan cycleDelay = default(TimeSpan))
        {
            timeout = timeout == default(TimeSpan) ? DefaultTimeout : timeout;
            cycleDelay = cycleDelay == default(TimeSpan) ? TimeSpan.FromMilliseconds(DefaultCycleDelayMs) : cycleDelay;
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            while (true)
            {
                try
                {
                    await successFunc.Invoke();
                    break;
                }
                catch (Exception)
                {
                    if (stopWatch.Elapsed > timeout)
                    {
                        throw;
                    }
                }
                await Task.Delay(cycleDelay);
            }
        }
    }
}