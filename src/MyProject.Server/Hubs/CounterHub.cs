using Microsoft.AspNetCore.SignalR;
using MyProject.Common;
using System.Threading.Tasks;

namespace MyProject.Backend.Controller.Hubs
{
    public class CounterHubService
    {
        private readonly IHubContext<CounterHub> hubContext;

        public CounterHubService(IHubContext<CounterHub> hubContext, ICounterService counterService)
        {
            this.hubContext = hubContext;
            counterService.OnNewValue += OnNewValue;
        }

        private async void OnNewValue(object sender, int newValue)
        {
            await hubContext.Clients.All.SendAsync("NewValue", newValue);
        }
    }

    public class CounterHub : Hub
    {
        private readonly ICounterService counterService;

        public CounterHub( ICounterService counterService, CounterHubService _)
        {
            this.counterService = counterService;
        }

        public async Task Increment(int byHowMuch)
        {
            await counterService.Increment(byHowMuch);
        }
    }
}
