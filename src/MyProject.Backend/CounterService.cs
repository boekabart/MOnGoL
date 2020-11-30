using MyProject.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyProject.Backend
{
    public class CounterService : ICounterService
    {
        private int counter;
        private EventHandler<int> onNewValue;

        public EventHandler<int> OnNewValue
        {
            get => onNewValue; set
            {
                onNewValue = value;
                OnNewValue?.Invoke(this, counter);
            }
        }

        public Task Increment(int byHowMuch)
        {
            var newValue = Interlocked.Add(ref counter, byHowMuch);
            OnNewValue?.Invoke(this, newValue);
            return Task.CompletedTask;
        }
    }
}
