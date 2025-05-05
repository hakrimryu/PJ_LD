using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Monster : Character
{
    [SerializeField] private HitText hitText;
    [SerializeField] private Image hpSlider;
    [SerializeField] private Image hpSliderDeco;
    
    private int _targetValue;
    public int hp;
    public int maxHp;
    private bool _isDead = false;
    
    private float _speed = 1f;
    
    public override void Init()
    {
        base.Init();
        hp = maxHp;
    }

    private void Update()
    {
        hpSliderDeco.fillAmount = Mathf.Lerp(hpSliderDeco.fillAmount, hpSlider.fillAmount, Time.deltaTime * 1.5f);
        
        if (_isDead) return;

        transform.position = Vector2.MoveTowards(transform.position, Spawner.MonsterMovePosList[_targetValue], Time.deltaTime * _speed);
        if (Vector2.Distance(transform.position, Spawner.MonsterMovePosList[_targetValue]) <= 0.0f)
        {
            _targetValue++;
            SpriteRenderer.flipX = _targetValue >= 3;

            if (_targetValue >= 4)
            {
                _targetValue = 0;
            }
        }
    }

    public void GetDamage(int damage)
    {
        if (_isDead) return;
        
        hp -= damage;
        hpSlider.fillAmount = (float)hp / (float)maxHp;
        
        Instantiate(hitText, transform.position, Quaternion.identity).Init(damage);
        
        if (hp <= 0)
        {
            hp = 0;
            _isDead = true;
            gameObject.layer = LayerMask.NameToLayer("Default");
            StartCoroutine(DieCo());
            AnimatorChange("Die", true);
        }
    }

    private IEnumerator DieCo()
    {
        float alpha = 1.0f;
        
        while (SpriteRenderer.color.a > 0.0f)
        {
            alpha -= Time.deltaTime;
            SpriteRenderer.color = new Color(SpriteRenderer.color.r, SpriteRenderer.color.g, SpriteRenderer.color.b, alpha);
            
            yield return null;
        }
        
        Destroy(gameObject);
    }
}
