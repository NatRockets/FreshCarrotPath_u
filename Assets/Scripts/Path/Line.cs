using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Line : MonoBehaviour
{
    [SerializeField] private int lockChanceBase;
    [SerializeField] private Text lockChanceUI;
    [SerializeField] private bool isMovable;

    private int lineId;
    private int lockChance;
    
    private List<Slot> slots;

    private Action<int> ShiftCallback;

    public void InitLine(int id, Action<int> shiftCallback)
    {
        lineId = id;
        ShiftCallback = shiftCallback;
        
        slots = new List<Slot>();
        int childCount = transform.childCount;
        Slot temp;
        for (int i = 0; i < childCount; i++)
        {
            temp = transform.GetChild(i).GetComponent<Slot>();
            temp.Init(this);
            slots.Add(temp);
        }
    }
    
    public void SetLine(SlotVisual[] targetSlots)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].SetSlot(targetSlots[i]);
        }

        lockChance = lockChanceBase;
        lockChanceUI.DOText(lockChance + "%", 0.5f);
    }

    private void CalculateLock()
    {
        if (Random.Range(0, 100) <= lockChance)
        {
            int i = 0;
            int ind;
            do
            {
                ind = Random.Range(0, slots.Count);
                i++;
            }
            while (slots[ind].IsOccupied() || i < 10);
            
            slots[ind].Lock();
            
            Color col = lockChanceUI.color;
            lockChanceUI.DOColor(Color.red, 0.5f)
                .OnComplete(() => lockChanceUI.DOColor(col, 0.5f));
        }
        
        lockChance += 5;
        lockChanceUI.DOText(lockChance + "%", 0.5f);
    }

    public void SetRabbit(Rabbit rabbit)
    {
        slots[slots.Count/2].BindRabbit(rabbit);
    }
    
    public int CalculateSlots()
    {
        int sum = 0;
        foreach (Slot slot in slots)
        {
            if (!slot.IsLocked())
            {
                sum++;
            }
        }
        
        return sum;
    }

    public void CompareLine(Line previous, Action<int> rabbitCallback, Action<int> callback)
    {
        Slot temp;
        bool rabbitMoved = false;
        for (int i = 0; i < slots.Count; i++)
        {
            temp = previous.GetSlotByIndex(i);
            if (temp.GetId() == slots[i].GetId())
            {
                if (slots[i].IsLocked())
                {
                    continue;
                }
                
                if (temp.IsOccupied())
                {
                    slots[i].BindRabbit(temp.GetRabbit());
                    temp.UnBindRabbit();
                    rabbitMoved = true;
                }
                else if (!slots[i].IsOccupied())
                {
                    slots[i].Lock();
                }
            }
        }

        if (rabbitMoved)
        {
            rabbitCallback?.Invoke(lineId);
        }
        else
        {
            callback?.Invoke(lineId);
        }
    }

    public Slot GetSlotByIndex(int index)
    {
        return slots[index];
    }
    
    public void ShiftLine(SwipeDirection direction)
    {
        if (!isMovable)
        {
            return;
        }

        Slot temp;
        Vector3 secondEdge;
        
        switch (direction)
        {
            case SwipeDirection.Left:
                temp = slots[0];
                //shift visuals
                secondEdge = slots[^1].GetPosition();
                for (int i = slots.Count - 1; i > 0; i--)
                {
                    slots[i].MoveSlot(slots[i - 1].GetPosition());
                }
                slots[0].MoveSlot(secondEdge, () =>ShiftCallback?.Invoke(lineId),true);

                //shift list
                for (int i = 1; i < slots.Count; i++)
                {
                    slots[i - 1] = slots[i];
                }
                slots[^1] = temp;
                
                CalculateLock();
                break;
            case SwipeDirection.Right: 
                temp = slots[^1];
                //shift visuals
                secondEdge = slots[0].GetPosition();
                for (int i = 0; i < slots.Count - 1; i++)
                {
                    slots[i].MoveSlot(slots[i + 1].GetPosition());
                }
                slots[^1].MoveSlot(secondEdge, () =>ShiftCallback?.Invoke(lineId),true);

                //shift list
                for (int i = slots.Count - 1; i > 0; i--)
                {
                    slots[i] = slots[i - 1];
                }
                slots[0] = temp;
                
                CalculateLock();
                break;
            default: ShiftCallback?.Invoke(lineId); break; 
        }
    }
}
