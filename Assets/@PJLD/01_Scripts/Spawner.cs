using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// 부모 SpriteRenderer를 기준으로 6x3 셀 오브젝트의 위치를 계산합니다.
/// 親のSpriteRendererを基準に6x3のセルオブジェクトの位置を計算します。
/// Calculates 6x3 cell positions based on the parent SpriteRenderer.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class Spawner : NetworkBehaviour
{
    [SerializeField] private GameObject spawnPrefab;

    [SerializeField] private Monster spawnMonsterPrefab;


    private Vector2 _parentSize;
    private Vector2 _cellScale;

    /// <summary>
    /// 계산된 셀의 위치 리스트
    /// 計算されたセルの位置リスト
    /// List of calculated cell positions
    /// </summary>
    private readonly List<Vector2> _playerSpawnList = new();
    private readonly List<Vector2> _companionSpawnList = new();

    private readonly List<bool> _playerHasCharacter = new();
    private readonly List<bool> _companionHasCharacter = new();

    public static readonly List<Vector2> PlayerMonsterMovePosList = new();
    public static readonly List<Vector2> CompanionMonsterMovePosList = new();

    private void Start()
    {
        Initialize();
    }

    /// <summary>
    /// 필드 값 초기화
    /// フィールド値の初期化を行います。
    /// Initializes field values.
    /// </summary>
    private void Initialize()
    {
        SetGrid();
        StartCoroutine(SpawnMonsterCo());
    }

    private void SetGrid()
    {
        InitGrid(transform.GetChild(0), true);
        InitGrid(transform.GetChild(1), false);
        
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            PlayerMonsterMovePosList.Add(transform.GetChild(0).GetChild(i).position);
        }

        for (int i = 0; i < transform.GetChild(1).childCount; i++)
        {
            CompanionMonsterMovePosList.Add(transform.GetChild(1).GetChild(i).position);
        }
    }

    #region Grid

    
    private void InitGrid(Transform tt, bool player)
    { 
         SpriteRenderer parentRenderer = tt.GetComponent<SpriteRenderer>();
        _parentSize = parentRenderer.bounds.size;

        _cellScale = new Vector2(
            tt.localScale.x / GameDefine.GridColumnCount,
            tt.localScale.y / GameDefine.GridRowCount
        );
        
        for (int row = 0; row < GameDefine.GridRowCount; row++)
        {
            for (int col = 0; col < GameDefine.GridColumnCount; col++)
            {
                float xPos = (-_parentSize.x / 2f) + (col * _cellScale.x) + (_cellScale.x / 2f);
                float yPos = (_parentSize.y / 2f) - (row * _cellScale.y) + (_cellScale.y / 2f);
                float yAdjust = tt.position.y - _cellScale.y;
                
                switch (player)
                {
                    case true:
                        _playerSpawnList.Add(new Vector2(xPos, yPos + yAdjust));
                        _playerHasCharacter.Add(false);
                        break;
                    case false:
                        _companionSpawnList.Add(new Vector2(xPos, yPos + yAdjust));
                        _companionHasCharacter.Add(false);
                        break;
                }
            }
        }
    }


    #endregion

    #region Player Spawn

    /// <summary>
    /// 빈 위치에 캐릭터를 소환합니다. 꽉 찼으면 생성하지 않고 로그를 남깁니다.
    /// 空いているセルにキャラクターを召喚します。全て埋まっている場合はログを出力します。
    /// Spawns a character into an empty cell. If full, logs a message and does not spawn.
    /// </summary>
    public void Summon()
    {
        if (GameManager.Instance.Money < GameManager.Instance.SummonCount)
            return;
        
        GameManager.Instance.Money -= GameManager.Instance.SummonCount;
        GameManager.Instance.SummonCount += 2;
        
        int posIndex = _playerHasCharacter.FindIndex(occupied => occupied == false);

        if (posIndex == -1)
        {
            Debug.LogWarning("[CharacterSpawner] 모든 그리드가 이미 꽉 찼습니다. 소환할 수 없습니다.");
            return;
        }

        GameObject go = Instantiate(spawnPrefab);
        go.transform.position = _playerSpawnList[posIndex];
        _playerHasCharacter[posIndex] = true;
    }
    
    #endregion

    #region Monster Spawn

    private IEnumerator SpawnMonsterCo()
    {
        yield return new WaitForSeconds(1f);
        
        if (IsClient)
        {
            ServerMonsterSpawnServerRpc(NetworkManager.Singleton.LocalClientId);
        }
        else if (IsServer)
        {
            MonsterSpawn(NetworkManager.Singleton.LocalClientId);
        }
        
        StartCoroutine(SpawnMonsterCo());
    }

    [ServerRpc(RequireOwnership = false)]
    private void ServerMonsterSpawnServerRpc(ulong clientId)
    {
        MonsterSpawn(clientId);
    }

    private void MonsterSpawn(ulong clientId)
    {
        var go = Instantiate(spawnMonsterPrefab, PlayerMonsterMovePosList[0], Quaternion.identity);
        NetworkObject networkObject = go.GetComponent<NetworkObject>();
        networkObject.Spawn();
        //GameManager.Instance.AddMonster(go);
        ClientMonsterSpawnClientRpc(networkObject.NetworkObjectId, clientId);

    }

    [ClientRpc]
    private void ClientMonsterSpawnClientRpc(ulong networkObjId, ulong clientId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjId, out NetworkObject monsterNetworkObject))
        {
            if (clientId == NetworkManager.Singleton.LocalClientId)
            {
                monsterNetworkObject.transform.position = new Vector3(0, -3, 0);
            }
            else
            {
                monsterNetworkObject.transform.position = new Vector3(0, 3, 0);
            }
        }
    }

    #endregion
}
