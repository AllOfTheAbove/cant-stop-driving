using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChangeTextColorOnHover : MonoBehaviour {

    private bool hover = false;
    private float startTime = 0f;
    private Color defaultColor;

    public Color hoverColor;
    public float transitionSpeed;

    public void MouseEnter()
    {
        hover = true;
        startTime = Time.time;
    }

    public void MouseLeave()
    {
        hover = false;
        startTime = Time.time;
    }

    private void Start()
    {
        defaultColor = this.GetComponentInChildren<TextMeshProUGUI>().color;
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
            this.GetComponentInChildren<TextMeshProUGUI>().color = Color.Lerp(defaultColor, hoverColor, t);
        } else
        {
            this.GetComponentInChildren<TextMeshProUGUI>().color = Color.Lerp(hoverColor, defaultColor, t);
        }
    }
}
