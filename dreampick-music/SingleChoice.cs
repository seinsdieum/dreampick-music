using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace dreampick_music;



public class ButtonCommand : ICommand
{
    private Action<object> execute;
    private Func<object, bool> canExecute;
    public event EventHandler CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value;}
        remove { CommandManager.RequerySuggested -= value; }
    }

    public ButtonCommand(Action<object> execute, Func<object, bool> canExecute = null)
    {
        this.execute = execute;
        this.canExecute = canExecute;
    }

    public bool CanExecute(object param)
    {
        return this.canExecute == null || canExecute(param);
    }

    public void Execute(object param)
    {
        this.execute(param);
    }
}

public class SingleChoice
{


    private Uri iconSource;
    private Visibility userAccessibility;
    private string localName;
    private Action choice;
    
    public string Name
    {
        get
        {
            string local = Utils.GetLocalizedName(localName);
            return local == null ? localName : local;
        }
    }

    public ButtonCommand ChoiceAction
    {
        get
        {
            return new ButtonCommand((o) =>
            {
                ExecuteChoice();
            });
        }
    }

    public Uri IconSource => iconSource;


    public SingleChoice(string localName, Action choiceAction, Uri iconSource = null, Visibility userAccessibility = Visibility.Visible)
    {
        this.localName = localName;
        this.iconSource = iconSource;
        this.userAccessibility = userAccessibility;
        choice = choiceAction;
    }


    public void ExecuteChoice()
    {
        choice.Invoke();
    }
}