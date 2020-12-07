using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MOnGoL.Common;
using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace MOnGoL.Backend.Controller.Hubs
{
    public class SignalRScopeService
    {
        ILogger<SignalRScopeService> Logger { get; set; }

        public SignalRScopeService(IServiceProvider serviceProvider, ILogger<SignalRScopeService> logger)
        {
            this.serviceProvider = serviceProvider;
            Logger = logger;
        }

        private ImmutableDictionary<string, IServiceScope> scopeDictionay = ImmutableDictionary<string, IServiceScope>.Empty;
        private SemaphoreSlim theLock = new SemaphoreSlim(1);
        private IServiceProvider serviceProvider;

        public async Task<IServiceProvider> GetScope(HubCallerContext context)
        {
            using var _ = await theLock.DisposableEnter();
            var connectionId = context.ConnectionId;
            if (scopeDictionay.TryGetValue(connectionId, out var scope))
            {
                Logger.LogDebug($"Re-using scope for {connectionId}");
                return scope.ServiceProvider;
            }

            Logger.LogDebug($"Creating scope for {connectionId}");
            scope = serviceProvider.CreateScope();
            context.ConnectionAborted.Register(() => DisposeServiceForConnection(context.ConnectionId));
            scopeDictionay = scopeDictionay.Add(connectionId, scope);
            return scope.ServiceProvider;
        }

        private async void DisposeServiceForConnection(string connectionId)
        {
            Logger.LogDebug($"Ditching scope for {connectionId}");
            using var _ = await theLock.DisposableEnter();
            if (!scopeDictionay.TryGetValue(connectionId, out var scope))
                return;
            scopeDictionay = scopeDictionay.Remove(connectionId);
            if (scope is IAsyncDisposable asyncScope)
                await asyncScope.DisposeAsync();
            else
                scope.Dispose();
        }
    }
}
