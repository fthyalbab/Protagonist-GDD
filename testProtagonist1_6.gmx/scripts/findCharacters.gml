///findCharacters(list)
// finds AND REMOVES characters from the list.
var list = argument0;
var characterMap = ds_map_create();
var split = ds_list_create();
var nameSplit = ds_list_create();
var character = noone;
var bracketCounter = 0;
for (var i = 0; i < ds_list_size(list); i++)
{
    var line = list[| i];
    // if on a character line, remove it
    if (character > 0)
    {
        ds_list_delete(list, i);
        i--;
    }
    // make sure the string is a character definition
    string_split(line, " ", split);
    if (split[| 0] == "character")
    {
        if (ds_list_size(split) >= 2)
        {
            // get the rest of the label
            var name = split[| 1];
            // create character marker and add to map
            if (character > 0)
            {
                show_error("Character " + name + " cannot be nested in " + character[| CHARACTER_NAME] + ".", true);
            }
            // if character already exists
            if (ds_map_exists(characterMap, name))
            {
                show_error("Character " + name + " is defined twice.", true);
            }
            character = createCharacter(name, name, noone, RIGHT);
            characterMap[? name] = character;
            // now remove this line
            ds_list_delete(list, i);
            i--;
            // if next line isn't "{" then we're done
            if (i < ds_list_size(list) - 1)
            {
                if (list[| i + 1] != "{")
                {
                    character = noone;
                }
            }
        }
        else
        {
            show_error("Character declaration is not followed by a character name.", true);
        }
    }
    // if we're currently on a character
    else if (character > 0)
    {
        // one layer deeper
        if (line == "{")
        {
            bracketCounter += 1;
        }
        // one layer shallower
        if (line == "}")
        {
            bracketCounter -= 1;
        }
        // if brackets unbalanced...
        if (bracketCounter < 0)
        {
            show_error("Unexpected symbol: }", true);
        }
        // if we finished our bracket sequence
        if (bracketCounter == 0)
        {
            character = noone;
        }
        // if display name declaration on depth 1
        if (split[| 1] == "=" && bracketCounter == 1)
        {
            // remove the command name from the display name we're going to use
            string_split(line, "=", nameSplit);
            if (split[| 0] == "name")
            {
                // set display name
                var displayName = unquote(string_trim(nameSplit[| 1]));
                character[| CHARACTER_DISPLAYNAME] = displayName;
            }
            if (split[| 0] == "speed")
            {
                // set textspeed of character
                var textspeed = string_trim(nameSplit[| 1]);
                character[| CHARACTER_TEXTSPEED] = textspeed;
            }
            if (split[| 0] == "side")
            {
                // set side of character
                var side = string_lower(string_trim(nameSplit[| 1]));
                var sideConst = noone;
                switch (side)
                {
                    case "left":
                        sideConst = LEFT;
                        break;
                    case "right":
                        sideConst = RIGHT;
                        break;
                    default:
                        show_error('Error in line "' + string(line) + '": Invalid side "' + side + '" is not "right" or "left"', true);
                        break;
                }
                character[| CHARACTER_SIDE] = sideConst;
            }
        }
    }
}
// free the split lists
ds_list_destroy(split);
ds_list_destroy(nameSplit);
// output
return characterMap;
