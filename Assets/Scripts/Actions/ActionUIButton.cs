using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ActionUIButton : MonoBehaviour, IPointerClickHandler,IPointerExitHandler, IPointerEnterHandler    
{

    [SerializeField] Color activeColor;
    [SerializeField] Color inActiveColor;
    PlayerInputManager playerInputManager;
    Image image;
    int actionIndex;

    private void Awake() 
    {
        playerInputManager = GetComponentInParent<PlayerInputManager>();
        image = GetComponent<Image>();
        image.color = inActiveColor;
    }

    public void Initialize(Sprite icon, int actionIndex)
    {
        image.sprite = icon;
        this.actionIndex = actionIndex;
        GetComponentInChildren<TextMeshProUGUI>().SetText(actionIndex.ToString());
    }

    public void SetImageActive(bool shouldBeActive)
    {
        if(shouldBeActive)
        {
            image.color = activeColor;
        }
        else
        {
            image.color = inActiveColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlayerEventManager.Instance.OnPlayerClickedAction(actionIndex);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }
}