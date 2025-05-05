using UnityEngine;
using System.Threading;

/// <summary>
/// 모든 캐릭터의 기본 클래스입니다.
/// すべてのキャラクターの基本クラスです。
/// Base class for all characters.
/// </summary>
public class Character : MonoBehaviour
{
    /// <summary>
    /// 캐릭터의 애니메이터 컴포넌트
    /// キャラクターのアニメーターコンポーネント
    /// Animator component of the character
    /// </summary>
    protected Animator _animator;
    
    /// <summary>
    /// 캐릭터의 취소 토큰
    /// キャラクターのキャンセルトークン
    /// Cancellation token for the character
    /// </summary>
    protected CancellationTokenSource _cancellationTokenSource;
    
    private void Awake()
    {
        _cancellationTokenSource = new CancellationTokenSource();
    }

    private void Start()
    {
        Initialize();
    }
    
    /// <summary>
    /// 캐릭터를 초기화합니다.
    /// キャラクターを初期化します。
    /// Initialize the character.
    /// </summary>
    protected virtual void Initialize()
    {
        _animator = transform.GetChild(0).GetComponent<Animator>();
    }
    
    private void OnDestroy()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
    }
}
