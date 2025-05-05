using UnityEngine;

/// <summary>
/// 부모 SpriteRenderer를 기준으로 6x3 셀 오브젝트를 배치합니다.
/// 親のSpriteRendererを基準に6x3のセルオブジェクトを生成します。
/// Places 6x3 cell GameObjects based on the parent SpriteRenderer.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class CharacterSpawner : MonoBehaviour
{
    private SpriteRenderer _parentRenderer;
    private Vector2 _parentSize;
    private Vector2 _cellScale;

    private void Start()
    {
        Initialize();
        SpawnGrid();
    }

    /// <summary>
    /// 필드 값 초기화
    /// フィールド値の初期化を行います。
    /// Initializes field values.
    /// </summary>
    private void Initialize()
    {
        _parentRenderer = GetComponent<SpriteRenderer>();
        _parentSize = _parentRenderer.bounds.size;

        _cellScale = new Vector2(
            transform.localScale.x / GameDefine.GridColumnCount,
            transform.localScale.y / GameDefine.GridRowCount
        );
    }

    /// <summary>
    /// 전체 셀을 생성합니다.
    /// 全てのセルを生成します。
    /// Spawns all grid cells.
    /// </summary>
    private void SpawnGrid()
    {
        for (int row = 0; row < GameDefine.GridColumnCount; row++)
        {
            for (int col = 0; col < GameDefine.GridRowCount; col++)
            {
                CreateCell(row, col);
            }
        }
    }

    /// <summary>
    /// 하나의 셀을 생성하고 위치를 지정합니다.
    /// 単一のセルを生成し、位置を設定します。
    /// Creates a single cell and sets its position.
    /// </summary>
    private void CreateCell(int row, int col)
    {
        GameObject cell = new GameObject(GetCellName(row, col))
        {
            transform =
            {
                localScale = new Vector3(_cellScale.x, _cellScale.y, 1f)
            }
        };

        AddSpriteRenderer(cell);

        Vector2 localPos = CalculateLocalPosition(row, col, cell.transform.localScale.y);
        cell.transform.localPosition = localPos;
    }

    /// <summary>
    /// SpriteRenderer를 추가하고 설정합니다.
    /// SpriteRendererを追加し、設定します。
    /// Adds and configures a SpriteRenderer.
    /// </summary>
    private void AddSpriteRenderer(GameObject obj)
    {
        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = _parentRenderer.sprite;
        sr.color = Color.black;
    }

    /// <summary>
    /// 셀의 위치를 계산합니다 (원본 코드 위치 계산 유지).
    /// セルの位置を計算します（元のコードと同じ位置になるように）。
    /// Calculates the position of a cell (keeps the same logic as original code).
    /// </summary>
    private Vector2 CalculateLocalPosition(int row, int col, float cellHeight)
    {
        float x = (-_parentSize.x / 2f) + (col * _cellScale.x) + (_cellScale.x / 2f);
        float y = (_parentSize.y / 2f) - (row * _cellScale.y) + (_cellScale.y / 2f);
        float yAdjust = transform.position.y - cellHeight;

        return new Vector2(x, y + yAdjust);
    }

    /// <summary>
    /// 셀 이름 생성 ("Grid_0:1" 등)
    /// セルの名前を生成します（例："Grid_0:1"）。
    /// Generates the cell name (e.g., "Grid_0:1").
    /// </summary>
    private static string GetCellName(int row, int col)
    {
        return $"Grid_{row}:{col}";
    }
}
