using System;
using Unity.Netcode;

public partial class NetManager
{
    private void OnPlayerJoined()
    {
        if (NetworkManager.Singleton.ConnectedClients.Count >= MaxPlayers)
        {
            ChangeSceneForAllPlayers();
        }
    }

    private void ChangeSceneForAllPlayers()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(gameSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }
}
