using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// 몬스터 캐릭터 클래스입니다.
/// モンスターキャラクタークラスです。
/// Monster character class.
/// </summary>
public class Monster : Character
{
    /// <summary>
    /// 현재 타겟 위치의 인덱스
    /// 現在のターゲット位置のインデックス
    /// Current target position index
    /// </summary>
    private int _targetPositionIndex;
    
    /// <summary>
    /// 몬스터를 초기화합니다.
    /// モンスターを初期化します。
    /// Initialize the monster.
    /// </summary>
    protected override void Initialize()
    {
        base.Initialize();
        MoveToNextPositionAsync(_cancellationTokenSource.Token).Forget();
    }

    /// <summary>
    /// 몬스터를 다음 위치로 이동시킵니다.
    /// モンスターを次の位置に移動させます。
    /// Move monster to the next position.
    /// </summary>
    private async UniTask MoveToNextPositionAsync(System.Threading.CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            Vector2 targetPosition = Spawner.MonsterMovePosList[_targetPositionIndex];
            
            while (Vector2.Distance(transform.position, targetPosition) > GameDefine.MONSTER_ARRIVAL_DISTANCE)
            {
                if (cancellationToken.IsCancellationRequested) return;
                
                transform.position = Vector2.MoveTowards(
                    transform.position, 
                    targetPosition, 
                    Time.deltaTime * GameDefine.MONSTER_MOVE_SPEED
                );
                
                await UniTask.Yield(cancellationToken);
            }
            
            _targetPositionIndex = (_targetPositionIndex + 1) % Spawner.MonsterMovePosList.Count;
        }
    }
}
