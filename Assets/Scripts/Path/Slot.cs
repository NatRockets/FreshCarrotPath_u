using System;
using DG.Tweening;
using UnityEngine;

[Serializable]
public struct SlotVisual
{
    public Sprite sprite;
    public Sprite alternateSprite;
    public int id;
}

public class Slot : MonoBehaviour
{
    [SerializeField] private bool isLocked;
    private Transform slotT;
    private SpriteRenderer spriteRenderer;
    private SlotVisual targetVisual;
    private Line parentLine;
    private Rabbit rabbit = null;
    
    private Vector3 slotScale;

    public void Init(Line parent)//one time init
    {
        parentLine = parent;
        slotT = transform;
        slotScale = slotT.localScale;
        spriteRenderer = slotT.GetChild(0).GetComponent<SpriteRenderer>();
    }

    public void SetSlot(SlotVisual slotVisual)//init game session
    {
        targetVisual = slotVisual;
        Unlock();
        rabbit = null;
    }

    public void MoveSlot(Vector3 targetPosition, Action callback = null, bool transitEdge = false)
    {
        //slotT.position = targetPosition;
        
        if (transitEdge)
        {
            slotT.DOScale(0f, 0.4f)
                .OnComplete(() =>
                {
                    slotT.position = targetPosition;
                    slotT.DOScale(slotScale, 0.4f)
                        .OnComplete(() => callback?.Invoke());
                });
            return;
        }

        slotT.DOMove(targetPosition, 0.8f);
    }

    public void BindRabbit(Rabbit r)
    {
        rabbit = r;
        rabbit.Move(slotT);
    }

    public Rabbit GetRabbit()
    {
        return rabbit;
    }

    public void UnBindRabbit()
    {
        rabbit = null;
    }
    
    public Line GetParentLine()
    {
        return parentLine;
    }

    public void Unlock()
    {
        isLocked = false;
        spriteRenderer.sprite = targetVisual.sprite;
    }

    public void Lock()
    {
        isLocked = true;
        spriteRenderer.sprite = targetVisual.alternateSprite;
    }

    public bool IsOccupied()
    {
        return rabbit;
    }

    public bool IsLocked()
    {
        return isLocked;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public int GetId()
    {
        return targetVisual.id;
    }
}
