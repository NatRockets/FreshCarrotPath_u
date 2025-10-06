using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Rabbit : MonoBehaviour
{
    private Transform rabbitTransform;

    public void Init()
    {
        rabbitTransform = transform;
    }
    
    public void Move(Transform t)
    {
        rabbitTransform.DOMove(t.position, 0.8f)
            .OnComplete(() => rabbitTransform.parent = t);
        rabbitTransform.DOScaleY(1.2f, 0.4f)
            .OnComplete(() => rabbitTransform.DOScaleY(1f, 0.4f));
    }
}
