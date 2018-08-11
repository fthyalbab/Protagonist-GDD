﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

// helper class used to wrap setting alpha/y/text/size into one object per GameObject.
// Then, this behavior wraps it into one method call.
public class UIPanel : MonoBehaviour
{
    bool initialized = false;
    InputField textbox;
    Text text;
    Image frame;
    Image fill;
    [HideInInspector] public RectTransform rect;

    public bool rootPanel = true;
    public float left = 0f;
    public float right = 1f;

    void Awake()
    {
        if (ResolutionHandler.Ready)
        {
            Initialize();
        }
    }

    void Start()
    {
        if (!initialized)
        {
            Initialize();
        }
    }

    void Initialize()
    {
        textbox = gameObject.GetComponentInChildren<InputField>();
        text = gameObject.GetComponentInChildren<Text>();
        frame = transform.Find("Frame").gameObject.GetComponent<Image>();
        fill = transform.Find("Fill").gameObject.GetComponent<Image>();
        rect = GetComponent<RectTransform>();
        UpdateAnchors();
        initialized = true;
    }

    public string GetText()
    {
        return textbox.text;
    }
    public void SetText(string message)
    {
        textbox.text = message;
    }

    public void SetAlpha(float alpha)
    {
        if (text != null)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
        }
        if (frame != null)
        {
            frame.color = new Color(frame.color.r, frame.color.g, frame.color.b, alpha);
        }
        if (fill != null)
        {
            fill.color = new Color(fill.color.r, fill.color.g, fill.color.b, alpha);
        }
    }

    // y position control
    public float GetLocalY()
    {
        return rect.anchoredPosition.y;
    }
    public void SetLocalY(float height)
    {
        rect.anchoredPosition = new Vector3(rect.anchoredPosition.x, height, rect.position.z);
    }
    public float GetScreenY()
    {
        return ResolutionHandler.RectToScreenPoint(rect, new Vector2(0, 0)).y;
    }
    public void SetScreenY(float screenY)
    {
        SetLocalY(rect.anchoredPosition.y + ResolutionHandler.ScreenToRectPoint(rect, new Vector2(0, screenY)).y);
    }

    public float GetSize()
    {
        return rect.sizeDelta.y;
    }
    public void SetSize(float size, float pivot = 0)
    {
        var resize = size - GetSize();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, size);
        SetScreenY(GetScreenY() + resize * pivot);
    }

    // resets anchor position to those of the left/right fields
    public void UpdateAnchors()
    {
        UpdateAnchors(left, right);
    }
    // temporarily sets anchor position to the given left/right, does not set the object's left/right fields
    public void UpdateAnchors(float left, float right)
    {
        // set anchor to resolution size
        if (rect != null)
        {
            if (rootPanel)
            {
                rect.anchorMin = ResolutionHandler.MapViewToScreenPoint(new Vector2(left, 0)) / new Vector2(Screen.width, Screen.height);
                rect.anchorMax = ResolutionHandler.MapViewToScreenPoint(new Vector2(right, 0)) / new Vector2(Screen.width, Screen.height);
            }
            else
            {
                rect.anchorMin = new Vector2(left, rect.anchorMin.y);
                rect.anchorMax = new Vector2(right, rect.anchorMax.y);
            }
        }
    }
}