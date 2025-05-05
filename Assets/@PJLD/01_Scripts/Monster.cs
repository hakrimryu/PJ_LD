using UnityEngine;

public class Monster : Character
{
    private int targetValue;
    
    public Vector2 _target;
    private float _speed = 1f;
    
    public override void Init()
    {
        base.Init();
    }

    private void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, Spawner.MonsterMovePosList[targetValue], Time.deltaTime * _speed);
        if (Vector2.Distance(transform.position, Spawner.MonsterMovePosList[targetValue]) <= 0.0f)
        {
            targetValue++;
            SpriteRenderer.flipX = targetValue >= 3;

            if (targetValue >= 4)
            {
                targetValue = 0;
            }
        }
    }
}
