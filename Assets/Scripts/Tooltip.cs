using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tooltip : MonoBehaviour
{
    Camera cam;
    TextMeshProUGUI text;
    RectTransform background;

    private void Awake() 
    {
        text = GetComponentInChildren<TextMeshProUGUI>();    
        background = transform.Find("background").GetComponent<RectTransform>();
        cam = Camera.main;
    }

    private void Update() 
    {
        Debug.Log("Update being called");
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), Input.mousePosition, cam, out localPoint);    
        transform.localPosition = localPoint;
    }
    
    public void SetText(string newText)
    {
        text.text = newText;
        // Vector2 backgroundSize = new Vector2(text.preferredWidth + 8f, text.preferredHeight + 8f);
        // background.sizeDelta = backgroundSize;
    }
}
