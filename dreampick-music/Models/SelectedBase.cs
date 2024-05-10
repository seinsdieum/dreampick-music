using System;

namespace dreampick_music.Models;

public class SelectedBase
{
    

    public bool IsSelection = false;
    private Action selectedHandler = (() => {});

    private Action<object> objectSelected = ((o) => { });
    
    public void AddOnSelected(params Action[] onSelected)
    {
        foreach (var s in onSelected)
        {
            selectedHandler += s;
        }
    }

    public void AddOnObjectSelected(params Action<object>[] onSelected)
    {
        foreach (var s in onSelected)
        {
            objectSelected += s;
        }
    }
    

    public ButtonCommand SelectionCommand => new ButtonCommand(o =>
    {
        if (o is not null)
        {
            objectSelected.Invoke(o);
        }
        selectedHandler.Invoke();


    });

}