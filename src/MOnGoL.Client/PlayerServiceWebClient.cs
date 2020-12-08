﻿using Microsoft.AspNetCore.SignalR.Client;
using MOnGoL.Common;
using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace MOnGoL.Backend.Client
{

    public class PlayerServiceWebClient : IPlayerService
    {
        public PlayerServiceWebClient(SignalRConnection signalR)
        {
            SignalR = signalR;
            SignalR.HubConnection.On<IImmutableList<PlayerState>>("PlayerlistChanged", newValue =>
            {
                lastPlayerlist = newValue;
                OnPlayerlistChanged?.Invoke(this, newValue);
            });
        }

        private EventHandler<IImmutableList<PlayerState>> onPlayerlistChanged;
        private IImmutableList<PlayerState> lastPlayerlist;

        public EventHandler<IImmutableList<PlayerState>> OnPlayerlistChanged
        {
            get => onPlayerlistChanged;
            set
            {
                onPlayerlistChanged = value;
                _ = SignalR.Connect();
                if (lastPlayerlist is not null)
                    onPlayerlistChanged?.Invoke(this, lastPlayerlist);
            }
        }

        public async Task<PlayerInfo?> GetMyInfo()
        {
            await SignalR.Connect();
            return await HubConnection.InvokeAsync<PlayerInfo?>("GetMyInfo");
        }

        public async Task<IImmutableList<PlayerState>> GetPlayerlist()
        {
            await SignalR.Connect();
            return await HubConnection.InvokeAsync<IImmutableList<PlayerState>>("GetPlayerlist");
        }

        public async Task Leave()
        {
            await SignalR.Connect();
            await HubConnection.SendAsync("Leave");
            await HubConnection.StopAsync();
        }

        public async Task<PlayerInfo?> Register(PlayerInfo myInfo)
        {
            await Connect();
            return await HubConnection.InvokeAsync<PlayerInfo?>("Register", myInfo);
        }

        public SignalRConnection SignalR { get; }
        private HubConnection HubConnection => SignalR.HubConnection;
        private Task Connect()
        {
            return SignalR.Connect();
        }
    }
}
