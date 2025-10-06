using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class LineController : MonoBehaviour
{
    [SerializeField] private Sprite[] slotSprites;
    [SerializeField] private Sprite[] slotSpritesAlt;
    [SerializeField] private Line[] lines;

    private SlotVisual[] visuals;
    
    private Action winCallback;

    public void InitLines(Action wCallback)
    {
        winCallback = wCallback;
        
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i].InitLine(i, OnLineShifted);
        }
        
        //init visuals
        visuals = new SlotVisual[slotSprites.Length];
        for (int i = 0; i < visuals.Length; i++)
        {
            visuals[i].id = i;
            visuals[i].sprite = slotSprites[i];
            visuals[i].alternateSprite = slotSpritesAlt[i];
        }
    }

    public int CalculateSlots()
    {
        int sum = 0;
        foreach (Line line in lines)
        {
            sum += line.CalculateSlots();
        }
        
        return sum;
    }

    public void SetLines(Rabbit rabbit)
    {
        MixLine(visuals);
        lines[^1].SetLine(visuals);
        
        for (int i = 0; i < lines.Length - 1; i++)
        {
            lines[i].SetLine(visuals);
            ShuffleDerange(visuals);
        }
        
        lines[0].SetRabbit(rabbit);
    }
    
    private void OnLineShifted(int lineId)
    {
        lines[lineId].CompareLine(lines[lineId - 1], OnRabbitMoved, null);
        OnLineCompared(lineId);
    }

    private void OnRabbitMoved(int lineId)
    {
        if (lineId >= lines.Length - 1)//final line
        {
            winCallback?.Invoke();
        }
        else
        {
            OnLineCompared(lineId);
        }
    }

    private void OnLineCompared(int lineId)
    {
        lines[lineId + 1].CompareLine(lines[lineId], OnRabbitMoved, null);
    }

    private void MixLine<T>(T[] array)
    {
        int n = array.Length;
        while (n > 1) 
        {
            int k = Random.Range(0, n--);
            (array[n], array[k]) = (array[k], array[n]);
        }
    }
    
    public static void ShuffleDerange<T>(T[] a)
    {
        if (a.Length < 2) return;

        // remember original positions
        var original = a.ToArray();          // shallow copy is enough for structs
        int tries = 0;

        do
        {
            // Fisher–Yates
            for (int i = a.Length - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (a[i], a[j]) = (a[j], a[i]);
            }

            // derangement test: no element equals the one that was originally here
            bool ok = true;
            for (int i = 0; i < a.Length && ok; i++)
                ok = !EqualityComparer<T>.Default.Equals(a[i], original[i]);

            if (ok) return;          // success

        } while (++tries < 100);     // safety valve

        // fallback: guaranteed derangement – rotate left by 1
        var tmp = a[0];
        Array.Copy(a, 1, a, 0, a.Length - 1);
        a[^1] = tmp;
    }
}
