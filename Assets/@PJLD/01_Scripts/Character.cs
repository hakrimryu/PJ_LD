using System;
using UnityEngine;

public class Character : MonoBehaviour
{
    Animator _animator;
    private void Start()
    {
        Init();
    }

    public virtual void Init()
    {
        _animator = transform.GetChild(0).GetComponent<Animator>();
    }
}
