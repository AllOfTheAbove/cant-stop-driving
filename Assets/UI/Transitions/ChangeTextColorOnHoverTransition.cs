using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChangeTextColorOnHoverTransition : MonoBehaviour {

    private bool hover = false;
    private float startTime = 0f;
    private Color defaultColor;

    public Color hoverColor;
    public float transitionSpeed;

    public void MouseClick()
    {
        hover = false;
        GetComponentInChildren<TextMeshProUGUI>().color = defaultColor;
    }

    public void MouseEnter()
    {
        hover = true;
        startTime = Time.time;
    }

    public void MouseLeave()
    {
        hover = false;
        startTime = Time.time;
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void Start()
    {
        defaultColor = GetComponentInChildren<TextMeshProUGUI>().color;
    }

    private void Update()
    {
        if(startTime == 0f)
        {
            return;
        }

        float t = (Time.time - startTime) * transitionSpeed;
        if(hover)
        {
            GetComponentInChildren<TextMeshProUGUI>().color = Color.Lerp(defaultColor, hoverColor, t);
        } else
        {
            GetComponentInChildren<TextMeshProUGUI>().color = Color.Lerp(hoverColor, defaultColor, t);
        }
    }
}
