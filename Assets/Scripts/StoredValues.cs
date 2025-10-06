using System;
using DG.Tweening;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CustomContainer
{
    public int carrots;
    public int locks;
    public int freeSwipes;
}

public class StoredValues : MonoBehaviour
{
    private CustomContainer container = new CustomContainer();

    [SerializeField] private Text scoreText;
    [SerializeField] private Text locksText;
    [SerializeField] private Text freeSwipesText;
    
    public int Carrots
    {
        get => container.carrots;
        set
        {
            container.carrots = value;
            scoreText.DOText(value.ToString(), 0.4f);
        }
    }
    
    public int Locks
    {
        get => container.locks;
        set
        {
            container.locks = value;
            locksText.DOText(value.ToString(), 0.4f);
        }
    }
    
    public int Swipes
    {
        get => container.freeSwipes;
        set
        {
            container.freeSwipes = value;
            freeSwipesText.DOText(value.ToString(), 0.4f);
        }
    }
    
    private void OnEnable()
    {
        if (!PlayerPrefs.HasKey("CarrotsV"))
        {
            container.carrots = 40;
            container.locks = 5;
            container.freeSwipes = 5;
        }
        else
        {
            string saveData = PlayerPrefs.GetString("CarrotsV");
            container = JsonConvert.DeserializeObject<CustomContainer>(saveData);
        }
        
        Carrots = container.carrots;
        Locks = container.locks;
        Swipes = container.freeSwipes;
    }

    private void OnDisable()
    {
        PlayerPrefs.SetString("CarrotsV", JsonConvert.SerializeObject(container));
    }
}
