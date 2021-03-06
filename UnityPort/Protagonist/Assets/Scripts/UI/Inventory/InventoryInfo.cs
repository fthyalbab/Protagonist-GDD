﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Panel that contains the text display of the Inventory Item's information,
 * along with the eat/discard buttons.
 * Handles display a given item's data, as well as the eat/discard button presses.
 */
public class InventoryInfo : MonoBehaviour
{
    InventoryDisplay display;
    Inventory inventory;

    UIPanel panel;
    UIPanel imagePanel;
    UIPanel textPanel;
    SpriteRenderer image;
    Text itemName;
    Text itemDesc;
    UIPanel eatButton;
    UIPanel discardButton;

    // Use this for initialization
    void Start ()
    {
        panel = GetComponent<UIPanel>();
        inventory = transform.parent.GetComponent<Inventory>();
        display = transform.parent.GetComponent<InventoryDisplay>();
        // item image
        imagePanel = transform.Find("ImagePanel").GetComponent<UIPanel>();
        image = imagePanel.transform.Find("ImageContainer").Find("Image").GetComponent<SpriteRenderer>();
        // name/desc text
        textPanel = transform.Find("TextPanel").GetComponent<UIPanel>();
        itemName = textPanel.transform.Find("Name").GetComponent<Text>();
        itemDesc = textPanel.transform.Find("Description").GetComponent<Text>();
        // buttons
        eatButton = textPanel.transform.Find("EatButton").GetComponent<UIPanel>();
        discardButton = textPanel.transform.Find("DiscardButton").GetComponent<UIPanel>();
    }

    void Update()
    {
        // handle button clicks
        if (display.selectedItem != null && Input.GetMouseButtonDown(0))
        {
            if (eatButton.rect.Contains(Input.mousePosition))
            {
                EatItem(display.selectedItem);
            }
            if (discardButton.rect.Contains(Input.mousePosition))
            {
                DiscardItem(display.selectedItem);
            }
        }
    }

    // set info display attributes
    public void SetEatButton(bool visible)
    {
        eatButton.alpha = visible ? 1 : 0;
    }
    public void SetDiscardButton(bool visible)
    {
        eatButton.alpha = visible ? 1 : 0;
    }
    public void SetImage(Sprite s)
    {
        image.sprite = s;
    }
    public void SetName(string s)
    {
        itemName.text = s;
    }
    public void SetDesc(string s)
    {
        itemDesc.text = s;
    }
    public void SetAlpha(float alpha)
    {
        panel.alpha = alpha;
        imagePanel.alpha = alpha;
        image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
        textPanel.alpha = alpha;
        itemName.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
        itemDesc.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
    }

    // button click actions
    private void EatItem(InventoryItem item)
    {
        item.Eat();
        if (item.type.edible)
        {
            SetImage(null);
            SetEatButton(false);
            SetDiscardButton(false);
            inventory.RemoveItem(item.item);
        }
        SetDesc(item.type.eatText);
    }
    private void DiscardItem(InventoryItem item)
    {
        if (!item.type.edible)
        {
            return;
        }
        display.selectedItem = null;
        DisplayItem(null);
        inventory.RemoveItem(item.item);
    }

    // set item display info
    public void DisplayItem(InventoryItem item)
    {
        if (item != null)
        {
            SetImage(item.sr.sprite);
            SetName(item.type.name);
            SetDesc(item.type.text);
            SetEatButton(true);
            SetDiscardButton(true);
            SetDiscardButton(item.type.edible);
        }
        else
        {
            SetImage(null);
            SetName("");
            SetDesc("");
            SetEatButton(false);
            SetDiscardButton(false);
        }
    }
}
