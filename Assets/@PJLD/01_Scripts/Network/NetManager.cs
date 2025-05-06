using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public partial class NetManager : MonoBehaviour
{
    private Lobby _currentLobby;
    private const int MaxPlayers = 2;
    private string gameSceneName = "GamePlayScene";
    
    public Button startMatchButton;
    public InputField inputField;
    public Button joinButton;
    public Text joinCodeText;
    
    private async void Start()
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        
        startMatchButton.onClick.AddListener(() => StartMatchMaking());
        joinButton.onClick.AddListener(() => JoinGameWithCode(inputField.text));
    }
}
