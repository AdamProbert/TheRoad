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
    bool active;

    private void Awake() 
    {
        text = GetComponentInChildren<TextMeshProUGUI>();    
        background = transform.Find("background").GetComponent<RectTransform>();
        cam = Camera.main;
        Deactivate();
    }

    private void LateUpdate()
    {
        if(active)
        {
            Debug.Log("Update being called");
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), Input.mousePosition, cam, out localPoint);    
            transform.localPosition = localPoint;
        }
    }
    
    public void Activate(string newText)
    {
        active = true;
        text.text = newText;
        text.gameObject.SetActive(true);
        background.gameObject.SetActive(true);
        Vector2 backgroundSize = new Vector2(text.preferredWidth + 8f, text.preferredHeight + 8f);
        background.sizeDelta = backgroundSize;
        text.GetComponent<RectTransform>().sizeDelta = backgroundSize;
    }

    public void Deactivate()
    {
        active = false;
        text.gameObject.SetActive(false);
        background.gameObject.SetActive(false);
    }
}
