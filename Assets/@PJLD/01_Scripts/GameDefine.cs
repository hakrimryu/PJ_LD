/// <summary>
/// 게임 내에서 사용되는 상수들을 정의합니다.
/// ゲーム内で使用される定数を定義します。
/// Defines game-wide constants.
/// </summary>
public static class GameDefine
{
    #region Grid Settings
    
    /// <summary>
    /// 그리드 열 개수
    /// グリッドの列数
    /// Number of columns in the grid
    /// </summary>
    public const int GRID_COLUMN_COUNT = 6;

    /// <summary>
    /// 그리드 행 개수
    /// グリッドの行数
    /// Number of rows in the grid
    /// </summary>
    public const int GRID_ROW_COUNT = 3;
    
    #endregion
    
    #region Monster Settings
    
    /// <summary>
    /// 몬스터 생성 간격 (초)
    /// モンスターの生成間隔（秒）
    /// Monster spawn interval (seconds)
    /// </summary>
    public const float MONSTER_SPAWN_INTERVAL = 0.5f;
    
    /// <summary>
    /// 몬스터 이동 속도
    /// モンスターの移動速度
    /// Monster movement speed
    /// </summary>
    public const float MONSTER_MOVE_SPEED = 1f;
    
    /// <summary>
    /// 몬스터 도착 판정 거리
    /// モンスターの到着判定距離
    /// Distance threshold for monster arrival
    /// </summary>
    public const float MONSTER_ARRIVAL_DISTANCE = 0.1f;
    
    #endregion
}