using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using UnityEngine;

public partial class NetManager
{
    public async void JoinGameWithCode(string inputJoinCode)
    {
        if (string.IsNullOrEmpty(inputJoinCode))
        {
            Debug.Log("Input JoinCode is null or empty.");
            return;
        }

        try
        {
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(inputJoinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData);
            
            StartClient();
            Debug.Log("Joining Game");
        }
        catch (RelayServiceException e)
        {
            Console.WriteLine(e);
        }
    }

    public async void StartMatchMaking()
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            Debug.Log("Not signed in");
            return;
        }

        _currentLobby = await FindAvailableLobbyAsync();

        if (_currentLobby == null)
        {
            await CreateNewLobbyAsync();
        }
        else
        {
            await JoinLobbyAsync(_currentLobby.Id);
        }

    }

    private async Task<Lobby> FindAvailableLobbyAsync()
    {
        try
        {
            var queryResponse = await LobbyService.Instance.QueryLobbiesAsync();
            if (queryResponse.Results.Count > 0)
            {
                return queryResponse.Results[0];
            }

        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
        return null;
    }

    private async Task CreateNewLobbyAsync()
    {
        try
        {
            _currentLobby = await LobbyService.Instance.CreateLobbyAsync("NewLobby", MaxPlayers);
            Debug.Log($"Created new lobby : {_currentLobby.Id}");

            await AllocateRelayServerAndJoin(_currentLobby);
            StartHost();

        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    private async Task AllocateRelayServerAndJoin(Lobby lobby)
    {
        try
        {
            var allocation = await RelayService.Instance.CreateAllocationAsync(lobby.MaxPlayers);
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            joinCodeText.text = joinCode;
            Debug.Log($"Relay server joined {joinCode}");
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
        }
    }

    private async Task JoinLobbyAsync(string lobbyId)
    {
        try
        {
            _currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
            Debug.Log($"Joined lobby : {_currentLobby.Id}");
            StartClient();
        }
        catch (LobbyServiceException e)
        {
            Console.WriteLine(e);
        }
    }

    private void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        Debug.Log("Host started");

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnHostDisConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        OnPlayerJoined();
    }

    private void OnHostDisConnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId && NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnHostDisConnected;
        }
    }

    private void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        Debug.Log("Client started");
    }
}
