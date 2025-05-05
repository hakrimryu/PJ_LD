using System;
using UnityEngine;

public class Character : MonoBehaviour
{
    protected Animator Animator;
    protected SpriteRenderer SpriteRenderer;
    
    private void Start()
    {
        Init();
    }

    protected void AnimatorChange(string temp, bool trigger)
    {
        if (trigger)
        {
            Animator.SetTrigger(temp);
        }
        else
        {
            Animator.SetBool(temp, true);
        }
    }

    public virtual void Init()
    {
        Animator = transform.GetChild(0).GetComponent<Animator>();
        SpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }
}
