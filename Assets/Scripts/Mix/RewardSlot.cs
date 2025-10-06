using System;
using DG.Tweening;
using UnityEngine;

public class RewardSlot : MonoBehaviour
{
    private Transform slot;
    [SerializeField] private int id; //1 - lock 2 - swipe
    [SerializeField] private Sprite sprite;

    public void Bind()
    {
        slot = transform;
    }
    
    public Transform GetSlot()
    {
        return slot;
    }

    public int GetId()
    {
        return id;
    }

    public Sprite GetSprite()
    {
        return sprite;
    }
    
    public void Scale(bool up, Action callback)
    {
        slot.DOScale(up ? 1f : 0f, 1f)
            .SetId("mix")
            .OnComplete(() => callback?.Invoke());
    }
}
