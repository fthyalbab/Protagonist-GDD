///dialogueStart(?label, ?display)

// get jump point to start at
var jump = "";
if (argument_count >= 1 && is_string(argument[0]))
{
    jump = argument[0];
}

// change display object if necessary
var displayObj = obj_dialogueBoxSpecial;
if (argument_count >= 2)
{
    displayObj = argument[1];
}
if (obj_dialogue.display.object_index != displayObj)
{
    obj_dialogue.display = instance_create(0, 0, displayObj);
}

// stops dialogue if already started
if (checkDialogueActive())
{
    dialogueStop();
}
if (jump != "")
{
    // if it doesn't exist, throw error
    if (!ds_map_exists(obj_dialogue.labels, jump))
    {
        show_error("The label " + string(jump) + " is not defined.", true);
    }
    var label = obj_dialogue.labels[? jump];
    // goto the specified line
    obj_dialogue.line = label[| LABEL_LINE];
}
// prepare dialogue display for start
obj_dialogue.display.status = START;
// start
obj_dialogue.active = true;
dialogueAdvance();
// set flags to indicate that this dialogue was run
var fileSplit = string_split(obj_dialogue.file, ".", ds_list_create());
ds_list_delete(fileSplit, ds_list_size(fileSplit) - 1);
var dialogueFilename = string_join(fileSplit, ".");
ds_list_destroy(fileSplit);
setFlag(dialogueFilename, true);
