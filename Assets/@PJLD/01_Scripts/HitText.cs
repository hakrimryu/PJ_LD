using System.Collections;
using TMPro;
using UnityEngine;

public class HitText : MonoBehaviour
{
    [SerializeField] private float tweenSpeed;
    [SerializeField] private float riseDuration;
    [SerializeField] private float fadeDuration;
    public Vector3 offset = new Vector3(0, 2, 0);
    
    public TextMeshPro damageText;
    private Color _textColor;

    public void Init(int damage)
    {
        damageText.text = damage.ToString();
        _textColor = damageText.color;
        StartCoroutine(MoveAndFadeCo());
    }

    private IEnumerator MoveAndFadeCo()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + offset;
        
        float elapsedTime = 0;

        while (elapsedTime < riseDuration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / riseDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        elapsedTime = 0;

        while (elapsedTime < fadeDuration)
        {
            _textColor.a = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            damageText.color = _textColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        Destroy(gameObject);
    }
}
