using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace SmartHome.WebSite.Models;

public class CustomTimeModule
{
    public int SecondsDelta { get; set; } = 0;

    private DateTime _endTime;

    private bool _isCountingDown = false;
    private bool _timerExecutable = true;

		public async Task<bool> WaitAmountOfSeconds(double seconds)
		{
			DateTime startTime = DateTime.Now;
			startTime = startTime.AddSeconds(seconds);
			while (DateTime.Compare(DateTime.Now, startTime) < 0)
			{
				await Task.Delay(100);
			}
			return true;
		}

    public void StartCountDown(int seconds)
    {
        _endTime = DateTime.Now.AddSeconds(seconds);
        _isCountingDown = true;
    }

    public void StopCountDown()
    {
        _isCountingDown = false;
    }

		public async Task StartTimerExecutable(Func<Task> reset, Delegate StateHasChanged)
		{
        _timerExecutable = true;
        while (_timerExecutable)
        {
            if (_isCountingDown)
            {
                SecondsDelta = (int)_endTime.Subtract(DateTime.Now).TotalSeconds;
                if (SecondsDelta <= 0)
                {
                    await reset();
                    StopCountDown();
                    SecondsDelta = 0;
                }
                StateHasChanged.DynamicInvoke();
            }
            await Task.Delay(500);
        }
    }

    public void StopTimerExecutable()
    {
        _timerExecutable = false;
    }
	}