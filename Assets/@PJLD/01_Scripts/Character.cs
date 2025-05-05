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

    public virtual void Init()
    {
        Animator = transform.GetChild(0).GetComponent<Animator>();
        SpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }
}
