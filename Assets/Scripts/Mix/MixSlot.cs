using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MixSlot : MonoBehaviour
{
    private Transform targetSlot;
    private RewardSlot target;

    public void InitSlot()
    {
        targetSlot = transform;
    }

    public Transform GetTargetSlot()
    {
        return targetSlot;
    }

    public void Bind(RewardSlot t)
    {
        target = t;
    }

    public RewardSlot GetTarget()
    {
        return target;
    }

    public void UnBind()
    {
        target = null;
    }

    public void Scale(bool up, Action callback)
    {
        targetSlot.DOScale(up ? 1f : 0f, 1f)
            .SetId("mix")
            .OnComplete(() => callback?.Invoke());
    }
}
