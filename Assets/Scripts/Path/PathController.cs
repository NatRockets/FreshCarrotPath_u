using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PathController : MonoBehaviour
{
    [SerializeField] private LineController lineController;
    [SerializeField] private InputController inputController;
    [SerializeField] private SceneRayCaster sceneRaycaster;
    [SerializeField] private Rabbit rabbit;
    [SerializeField] private StoredValues values;
    
    [SerializeField] private AudioSource audioSource;
    
    [SerializeField] private Button resetButton;
    [SerializeField] private ScreenBlock winBlock;
    [SerializeField] private ScreenBlock loseBlock;
    [SerializeField] private Text swipesText;
    [SerializeField] private Image freeSwipesImage;
    [SerializeField] private Text finishText;

    private int swipes;

    private void Awake()
    {
        lineController.InitLines(OnPathCompleted);
        inputController.SetupSwipe(OnSwiped);
        inputController.SetupTouch(OnTouched);
        sceneRaycaster.SetCallbacks(OnLineSwiped, UpdateLocks);
        rabbit.Init();
        
        resetButton.onClick.AddListener(ResetPath);
        winBlock.Bind();
        loseBlock.Bind();
    }
    
    private void OnEnable()
    {
        ResetPath();
    }

    private void OnDisable()
    {
        inputController.ActivateSwiping(false);
    }

    private void ResetPath()
    {
        lineController.SetLines(rabbit);
        
        inputController.ResetSwipe();
        inputController.ActivateSwiping(true);

        swipes = Random.Range(5, 10);
        swipesText.DOText(swipes.ToString(), 0.5f);
        
        freeSwipesImage.DOFade(0f, 0.5f);
    }
    
    private void OnSwiped(SwipeDirection direction, Vector3 pos)
    {
        sceneRaycaster.Cast(pos, true, direction);
    }

    private void OnTouched(Vector3 pos)
    {
        sceneRaycaster.Cast(pos);
    }

    private void OnPathCompleted()
    {
        audioSource.Play();
        int res = lineController.CalculateSlots() - 2;
        values.Carrots += res;
        
        finishText.text = $"Successfully harvested from {res} beds!";
        winBlock.Show(false, false, () =>
        {
            values.Carrots++;
            DOTween.Sequence()
                .AppendInterval(1f)
                .OnComplete(() => winBlock.Hide(false, false, ResetPath));
        });
    }

    private bool OnLineSwiped()
    {
        bool res = swipes > 0;
        if (res)
        {
            swipes--;
            swipesText.DOText(swipes.ToString(), 0.5f);
            
            if (swipes < 1)
            {
                OnSwipesOut();
            }

            return true;
        }
        
        res = values.Swipes > 0;
        if (res)
        {
            freeSwipesImage.DOFade(1f, 0.5f);
            values.Swipes--;
        }
        return res;
    }

    private bool UpdateLocks()
    {
        bool res = values.Locks > 0;
        if (res)
        {
            values.Locks--;
        }
        return res;
    }

    private void OnSwipesOut()
    {
        loseBlock.Show(false, false, () =>
        {
            DOTween.Sequence()
                .AppendInterval(0.8f)
                .OnComplete(() => loseBlock.Hide());
        });
    }
}
