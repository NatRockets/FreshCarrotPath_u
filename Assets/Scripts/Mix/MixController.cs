using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MixController : MonoBehaviour
{
    [FormerlySerializedAs("startupValues")] [SerializeField] private StoredValues storedValues;
    [SerializeField] private InputController inputController;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform[] slotPositions;
    [SerializeField] private Transform heightPosition;
    [SerializeField] private MixSlot[] slots;
    [SerializeField] private RewardSlot bonus1;
    [SerializeField] private RewardSlot bonus2;
    [SerializeField] private SpriteRenderer backRenderer;
    
    [SerializeField] private AudioSource audioSource;
    
    [SerializeField] private Button startButton;
    [SerializeField] private ScreenBlock winBlock;
    [SerializeField] private ScreenBlock loseBlock;
    [SerializeField] private Image winRes;
    [SerializeField] private Text hintText;
    
    private int mixCounter;
    
    private void Awake()
    {
        startButton.onClick.AddListener(StartMix);
        inputController.SetupTouch(Pick);

        foreach (MixSlot slot in slots)
        {
            slot.InitSlot();
        }
        
        bonus1.Bind();
        bonus2.Bind();
        
        winBlock.Bind();
        loseBlock.Bind();
    }
    
    private void OnEnable()
    {
        winBlock.HideForce();
        loseBlock.HideForce();
        InitMix();
    }
    
    private void OnDisable()
    {
        inputController.ActivateSwiping(false);
        DOTween.Kill("mix");
    }

    private void InitMix()
    {
        backRenderer.enabled = false;
        hintText.enabled = false;
        foreach (MixSlot slot in slots)
        {
            slot.Scale(false, null);
            slot.UnBind();
        }
        
        bonus1.GetSlot().position = heightPosition.position;
        bonus2.GetSlot().position = heightPosition.position;
        
        startButton.gameObject.SetActive(true);
        //startButton.interactable = storedValues.Carrots > 19;
        mixCounter = 0;
    }

    private void StartMix()
    {
        startButton.gameObject.SetActive(false);
        storedValues.Carrots -= 20;

        //init
        int tempInd = Random.Range(0, slotPositions.Length);
        Vector3 temp = bonus1.GetSlot().position;
        bonus1.GetSlot().position = new Vector3(slotPositions[tempInd].position.x, temp.y, temp.z);

        int tempInd2;
        do
        {
            tempInd2 = Random.Range(0, slotPositions.Length);
        }
        while(tempInd == tempInd2);
        bonus2.GetSlot().position = new Vector3(slotPositions[tempInd2].position.x, temp.y, temp.z);

        bonus1.GetSlot().DOMoveY(slotPositions[0].position.y, 1f).SetId("mix");
        bonus2.GetSlot().DOMoveY(slotPositions[0].position.y, 1f)
            .SetId("mix")
            .OnComplete(() =>
            {
                backRenderer.enabled = true;
                
                slots[tempInd].Bind(bonus1);
                slots[tempInd2].Bind(bonus2);
                
                slots[0].GetTargetSlot().position = slotPositions[0].position;
                slots[0].Scale(true, null);
                slots[1].GetTargetSlot().position = slotPositions[1].position;
                slots[1].Scale(true, null);
                slots[2].GetTargetSlot().position = slotPositions[2].position;
                slots[2].Scale(true, () =>
                {
                    bonus1.GetSlot().localScale = Vector3.zero;
                    bonus2.GetSlot().localScale = Vector3.zero;
                    Mix();
                });
                
                
            });
    }

    private void Mix()
    {
        if (mixCounter > 10)
        {
            bonus1.Scale(true, null);
            bonus2.Scale(true, null);
            hintText.enabled = true;
            ActivatePick();
            return;
        }
        
        mixCounter++;
        
        LineController.ShuffleDerange(slots);
        
        Move3(slots[0].GetTargetSlot(), slotPositions[0].position, null);
        Move3(slots[1].GetTargetSlot(), slotPositions[1].position, null);
        Move3(slots[2].GetTargetSlot(), slotPositions[2].position, Mix);
    }

    private void ActivatePick()
    {
        inputController.ActivateSwiping(true);
    }

    private void Pick(Vector3 pos)
    {
        Ray ray = mainCamera.ScreenPointToRay(pos);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.TryGetComponent<MixSlot>(out var slot))
        {
            hintText.enabled = false;
            inputController.ActivateSwiping(false);
            var reward = slot.GetTarget();
            int id = 0;
            Sprite s = null;
            if (reward != null)
            {
                id = reward.GetId();
                s = reward.GetSprite();
                reward.GetSlot().position = slot.GetTargetSlot().position;
            }
            slot.Scale(false, () => DoResult(id, s));
        }
    }

    private void DoResult(int result, Sprite sprite)
    {
        if(result > 0)
        {
            audioSource.Play();
            winRes.sprite = sprite;
            winBlock.Show(false, false, () =>
            {
                switch (result)
                {
                    case 1: storedValues.Locks++; break;
                    case 2: storedValues.Swipes++; break;
                }
                DOTween.Sequence()
                    .AppendInterval(1f)
                    .OnComplete(() => winBlock.Hide(false, false, InitMix));
            });
        }
        else
        {
            loseBlock.Show(false, false, () =>
            {
                
                DOTween.Sequence()
                    .AppendInterval(1f)
                    .OnComplete(() => loseBlock.Hide(false, false, InitMix));
            });
        }
    }

    /*private void Move1(Transform target, Vector3 pos)
    {
        target.DOMove(pos, 1f);
    }
    
    private void Move2(Transform target, Vector3 pos)
    {
        target.DOMove(pos, 1f);
    }*/
    
    private void Move3(Transform target, Vector3 pos, Action callback)
    {
        target.DOMove(pos, 1f)
            .SetId("mix")
            .OnComplete(() => callback?.Invoke());
    }
}
