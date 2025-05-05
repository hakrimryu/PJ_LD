using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

/// <summary>
/// 부모 SpriteRenderer를 기준으로 6x3 셀 오브젝트의 위치를 계산합니다.
/// 親のSpriteRendererを基準に6x3のセルオブジェクトの位置を計算します。
/// Calculates 6x3 cell positions based on the parent SpriteRenderer.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class Spawner : MonoBehaviour
{
    #region Fields
    
    [SerializeField] private GameObject _characterPrefab;
    [SerializeField] private Monster _monsterPrefab;
    
    private SpriteRenderer _parentRenderer;
    private Vector2 _parentSize;
    private Vector2 _cellScale;
    private CancellationTokenSource _cancellationTokenSource;
    
    /// <summary>
    /// 계산된 셀의 위치 리스트
    /// 計算されたセルの位置リスト
    /// List of calculated cell positions
    /// </summary>
    private readonly List<Vector2> _spawnPositionList = new();
    private readonly List<bool> _cellOccupied = new();

    /// <summary>
    /// 몬스터가 이동할 수 있는 위치 목록
    /// モンスターが移動できる位置のリスト
    /// List of positions where monsters can move
    /// </summary>
    public static readonly List<Vector2> MonsterMovePosList = new();
    
    #endregion

    #region Unity Lifecycle
    
    private void Awake()
    {
        _cancellationTokenSource = new CancellationTokenSource();
    }

    private void Start()
    {
        Initialize();
        SpawnMonstersAsync(_cancellationTokenSource.Token).Forget();
    }
    
    private void OnDestroy()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
    }
    
    #endregion

    #region Initialization
    
    /// <summary>
    /// 스포너를 초기화합니다.
    /// スポナーを初期化します。
    /// Initializes the spawner.
    /// </summary>
    private void Initialize()
    {
        InitializeGrid();
        CreateSpawnPositions();
        InitializeMonsterMovePositions();
    }

    /// <summary>
    /// 그리드 관련 필드 값을 초기화합니다.
    /// グリッド関連のフィールド値を初期化します。
    /// Initializes the grid-related field values.
    /// </summary>
    private void InitializeGrid()
    {
        _parentRenderer = GetComponent<SpriteRenderer>();
        _parentSize = _parentRenderer.bounds.size;

        _cellScale = new Vector2(
            transform.localScale.x / GameDefine.GRID_COLUMN_COUNT,
            transform.localScale.y / GameDefine.GRID_ROW_COUNT
        );
    }

    /// <summary>
    /// 전체 셀 위치를 계산하여 리스트에 저장합니다.
    /// 全てのセルの位置を計算してリストに保存します。
    /// Calculates all cell positions and stores them in the list.
    /// </summary>
    private void CreateSpawnPositions()
    {
        _spawnPositionList.Clear();
        _cellOccupied.Clear();
        
        for (int row = 0; row < GameDefine.GRID_ROW_COUNT; row++)
        {
            for (int col = 0; col < GameDefine.GRID_COLUMN_COUNT; col++)
            {
                _spawnPositionList.Add(CalculateLocalPosition(row, col));
                _cellOccupied.Add(false);
            }
        }
    }

    /// <summary>
    /// 셀의 위치를 계산합니다.
    /// セルの位置を計算します。
    /// Calculates the position of a cell.
    /// </summary>
    private Vector2 CalculateLocalPosition(int row, int col)
    {
        float xPos = (-_parentSize.x / 2f) + (col * _cellScale.x) + (_cellScale.x / 2f);
        float yPos = (_parentSize.y / 2f) - (row * _cellScale.y) + (_cellScale.y / 2f);
        float yAdjust = transform.position.y - _cellScale.y;

        return new Vector2(xPos, yPos + yAdjust);
    }

    /// <summary>
    /// 몬스터 이동 위치를 초기화합니다.
    /// モンスターの移動位置を初期化します。
    /// Initializes monster move positions.
    /// </summary>
    private void InitializeMonsterMovePositions()
    {
        MonsterMovePosList.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            MonsterMovePosList.Add(transform.GetChild(i).position);
        }
    }
    
    #endregion

    #region Character Spawn
    
    /// <summary>
    /// 빈 위치에 캐릭터를 소환합니다. 꽉 찼으면 생성하지 않고 로그를 남깁니다.
    /// 空いているセルにキャラクターを召喚します。全て埋まっている場合はログを出力します。
    /// Spawns a character into an empty cell. If full, logs a message and does not spawn.
    /// </summary>
    public void SummonCharacter()
    {
        if (!TryFindEmptyCell(out int posIndex))
        {
            Debug.LogWarning("[CharacterSpawner] 모든 그리드가 이미 꽉 찼습니다. 소환할 수 없습니다.");
            return;
        }

        SpawnCharacterAtPosition(posIndex);
    }

    /// <summary>
    /// 빈 셀을 찾습니다.
    /// 空いているセルを探します。
    /// Finds an empty cell.
    /// </summary>
    private bool TryFindEmptyCell(out int posIndex)
    {
        posIndex = _cellOccupied.FindIndex(occupied => !occupied);
        return posIndex != -1;
    }

    /// <summary>
    /// 지정된 위치에 캐릭터를 생성합니다.
    /// 指定された位置にキャラクターを生成します。
    /// Spawns a character at the specified position.
    /// </summary>
    private void SpawnCharacterAtPosition(int posIndex)
    {
        GameObject character = Instantiate(_characterPrefab);
        character.transform.position = _spawnPositionList[posIndex];
        _cellOccupied[posIndex] = true;
    }
    
    #endregion

    #region Monster Spawn
    
    /// <summary>
    /// 지정된 간격으로 몬스터를 생성합니다.
    /// 指定された間隔でモンスターを生成します。
    /// Spawns monsters at the specified interval.
    /// </summary>
    private async UniTask SpawnMonstersAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            SpawnMonster();
            
            await UniTask.Delay(
                TimeSpan.FromSeconds(GameDefine.MONSTER_SPAWN_INTERVAL), 
                cancellationToken: cancellationToken
            );
        }
    }

    /// <summary>
    /// 몬스터를 생성합니다.
    /// モンスターを生成します。
    /// Spawns a monster.
    /// </summary>
    private void SpawnMonster()
    {
        Instantiate(_monsterPrefab, MonsterMovePosList[0], Quaternion.identity);
    }
    
    #endregion
}
