﻿using Assets.Scripts.Libraries.ProtagonistDialog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/**
 * This is a partial class to Dialog. A partial class allows you to spread code for the same class across multiple files.
 * This contains methods that handle statements like "show" and "hide".
 * This is so that the statement-handling methods are not in the same file as the logic.
 * This also houses some important enums and static dictionaries.
 * For the main thing, see Dialog.cs
 */
public partial class Dialog
{
    // sides and positions
    public static readonly Dictionary<string, Vector2> sides = new Dictionary<string, Vector2>()
    {
        { "Center", new Vector2(0.5f, 0) },
        { "Left", new Vector2(0.2f, 0) },
        { "Front Left", new Vector2(0.25f, -0.1f) },
        { "Back Left", new Vector2(0.2f, 0) },
        { "Alt Left", new Vector2(0.35f, -0.05f) },
        { "Right", new Vector2(1 - 0.2f, 0) },
        { "Front Right", new Vector2(1 - 0.25f, -0.1f) },
        { "Back Right", new Vector2(1 - 0.2f, 0) },
        { "Alt Right", new Vector2(1 - 0.35f, -0.05f) }
    };

    // image states
    public enum DialogImage
    {
        Normal, Happy, Glitch
    }
    static Dictionary<string, DialogImage> images = new Dictionary<string, DialogImage>();
    static Dialog() {
        foreach (DialogImage img in Enum.GetValues(typeof(DialogImage)))
        {
            images[img.ToString()] = img;
        }
    }

    public GameObject ShowAction(Dictionary<string, object> show)
    {
        var chr = UpdateCharacter(show);
        // make sure sprite is filled in
        bool newShow = chr.gameObject == null;
        if (newShow)
        {
            if (!DialogPrefabs.Prefabs.ContainsKey(chr.sprite))
            {
                throw new ParseError("No DialogPrefab with name '" + chr.sprite + "' is registered. See the DialogPrefabs script on the DialogSystem GameObject.");
            }
            chr.gameObject = Instantiate(DialogPrefabs.Prefabs[chr.sprite]);
            // transition in
            chr.gameObject.GetComponent<DialogAnimationBase>().SetTransition(chr.transition, false, show);
        }
        UpdateCharacterAfter(show, chr, newShow);
        return chr.gameObject;
    }

    public GameObject HideAction(Dictionary<string, object> show)
    {
        var chr = UpdateCharacter(show);
        // make sure game obj exists
        if (chr.gameObject == null)
        {
            throw new ParseError("Invalid 'hide' statement, character '" + chr.abbrev + "' is not being shown.");
        }
        UpdateCharacterAfter(show, chr);
        // transition out
        chr.gameObject.GetComponent<DialogAnimationBase>().SetTransition(chr.transition, true, show);
        // set as hidden
        chr.gameObject = null;
        return chr.gameObject;
    }

    private DialogCharacter UpdateCharacter(Dictionary<string, object> show)
    {
        // name is required
        var name = GetValue(show, "name");
        // find the character obj with this name
        if (!parser.characters.ContainsKey(name))
        {
            throw new ParseError("A character with name '" + name + "' does not exist.");
        }
        var chr = parser.characters[name];
        // set transition if necessary
        var transition = GetValue(show, "transition", true);
        if (transition != null)
        {
            chr.transition = transition;
        }
        // set sprite if necessary
        if (chr.sprite == null)
        {
            chr.sprite = GetValue(show, "sprite");
        }
        return chr;
    }
    private void UpdateCharacterAfter(Dictionary<string, object> show, DialogCharacter chr, bool setPos = false)
    {
        // set image if necessary
        var image = GetValue(show, "image", true);
        if (image != null)
        {
            if (!images.ContainsKey(image))
            {
                throw new ParseError("The image tag '" + image + "' is not registered in the DialogImage enum.");
            }
            chr.gameObject.GetComponent<DialogAnimationBase>().SetImage(images[image]);
        }
        // set side if necessary
        var side = GetValue(show, "side", true);
        if (side != null)
        {
            chr.position = side;
        }
        Vector2 pos = Vector2.zero;
        if (sides.ContainsKey(chr.position))
        {
            pos = sides[chr.position];
        }
        else
        {
            pos = ParseVector2(chr.position);
        }
        chr.gameObject.GetComponent<DialogAnimationBase>().SetPosition(pos, setPos);
    }

    private Vector2 ParseVector2(string str)
    {
        str = str.Trim();
        // first and last chars must be ( and )
        if (str[0] != '(' || str[str.Length - 1] != ')')
        {
            throw new ParseError("Position '" + str + "' is not a valid (x, y) coordinate or registered side.");
        }
        str = str.Substring(1, str.Length - 2);
        // split by ,
        string[] xyStr = str.Split(',');
        if (xyStr.Length != 2)
        {
            throw new ParseError("Position '" + str + "' is not a valid (x, y) coordinate or registered side.");
        }
        float[] nums = new float[2];
        for (int i = 0; i < xyStr.Length; i++)
        {
            if (!float.TryParse(xyStr[i], out nums[i]))
            {
                new ParseError("Position '" + str + "' is not a valid (x, y) coordinate or registered side.");
            }
        }
        return new Vector2(nums[0], nums[1]);
    }

    public string GetValue(Dictionary<string, object> show, string name, bool optional = false)
    {
        // see if there is a key with the given name
        if (show.ContainsKey(name) && show[name] is string)
        {
            return (string)show[name];
        }
        else if (!optional)
        {
            throw new ParseError("Statement element 'show' must have a field name '" + name + "'.");
        }
        return null;
    }
}
