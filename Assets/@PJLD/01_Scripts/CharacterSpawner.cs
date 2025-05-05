using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 부모 SpriteRenderer를 기준으로 6x3 셀 오브젝트의 위치를 계산합니다.
/// 親のSpriteRendererを基準に6x3のセルオブジェクトの位置を計算します。
/// Calculates 6x3 cell positions based on the parent SpriteRenderer.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class CharacterSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject spawnPrefab;

    private SpriteRenderer _parentRenderer;
    private Vector2 _parentSize;
    private Vector2 _cellScale;

    /// <summary>
    /// 계산된 셀의 위치 리스트
    /// 計算されたセルの位置リスト
    /// List of calculated cell positions
    /// </summary>
    private readonly List<Vector2> _spawnList = new();
    private readonly List<bool> _hasCharacter = new();

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
        InitGrid();
        SpawnGrid();
    }

    #region Grid

    private void InitGrid()
    {
        _parentRenderer = GetComponent<SpriteRenderer>();
        _parentSize = _parentRenderer.bounds.size;

        _cellScale = new Vector2(
            transform.localScale.x / GameDefine.GridColumnCount,
            transform.localScale.y / GameDefine.GridRowCount
        );
    }

    /// <summary>
    /// 전체 셀 위치를 계산하여 리스트에 저장합니다.
    /// 全てのセルの位置を計算してリストに保存します。
    /// Calculates all cell positions and stores them in the list.
    /// </summary>
    private void SpawnGrid()
    {
        for (int row = 0; row < GameDefine.GridRowCount; row++)
        {
            for (int col = 0; col < GameDefine.GridColumnCount; col++)
            {
                _spawnList.Add(CalculateLocalPosition(row, col));
                _hasCharacter.Add(false);
            }
        }
    }

    /// <summary>
    /// 셀의 위치를 계산합니다 (원본 코드 위치 계산 유지).
    /// セルの位置を計算します（元のコードと同じ位置になるように）。
    /// Calculates the position of a cell (keeps the same logic as original code).
    /// </summary>
    private Vector2 CalculateLocalPosition(int row, int col)
    {
        float xPos = (-_parentSize.x / 2f) + (col * _cellScale.x) + (_cellScale.x / 2f);
        float yPos = (_parentSize.y / 2f) - (row * _cellScale.y) + (_cellScale.y / 2f);
        float yAdjust = transform.position.y - _cellScale.y;

        return new Vector2(xPos, yPos + yAdjust);
    }

    #endregion

    /// <summary>
    /// 빈 위치에 캐릭터를 소환합니다. 꽉 찼으면 생성하지 않고 로그를 남깁니다.
    /// 空いているセルにキャラクターを召喚します。全て埋まっている場合はログを出力します。
    /// Spawns a character into an empty cell. If full, logs a message and does not spawn.
    /// </summary>
    public void Summon()
    {
        int posIndex = _hasCharacter.FindIndex(occupied => occupied == false);

        if (posIndex == -1)
        {
            Debug.LogWarning("[CharacterSpawner] 모든 그리드가 이미 꽉 찼습니다. 소환할 수 없습니다.");
            return;
        }

        GameObject go = Instantiate(spawnPrefab);
        go.transform.position = _spawnList[posIndex];
        _hasCharacter[posIndex] = true;
    }
}
