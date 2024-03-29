using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine.Events;

public class ActionDropdown : AdvancedDropdown
{
    public Dictionary<string, System.Type> Actions = new Dictionary<string, System.Type>();

    private Rhythm.State _state = null;
    protected System.Action<System.Type, Rhythm.State> _onActionSelected;

    public ActionDropdown(AdvancedDropdownState state, System.Action<System.Type, Rhythm.State> onActionSelected, Rhythm.State rhythmState) : base(state)
    {
        _onActionSelected = onActionSelected;
        _state = rhythmState;
    }

    protected override AdvancedDropdownItem BuildRoot()
    {
        var root = new AdvancedDropdownItem("Action");

        List<System.Type> actions = RhythmEditor.Utility.GetAllSubclassesOf(typeof(Rhythm.Action));

        foreach (System.Type type in actions)
        {
            Actions.Add(type.Name, type);

            var item = new AdvancedDropdownItem(type.Name);
            root.AddChild(item);
        }

        return root;
    }

    protected override void ItemSelected(AdvancedDropdownItem item)
    {
        base.ItemSelected(item);

        _onActionSelected?.Invoke(Actions[item.name], _state);

        Debug.Log(item.name);
    }
}
