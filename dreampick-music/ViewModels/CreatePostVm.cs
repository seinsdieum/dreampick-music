using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using dreampick_music.Models;

namespace dreampick_music;

public class CreatePostVm : HistoryVm
{
    public string fieldText = "";


    private NotifyTaskCompletion<bool> uploadSuccess;

    public NotifyTaskCompletion<bool> UploadSuccess
    {
        get
        {
            return uploadSuccess;
        }
        set
        {
            uploadSuccess = value;
            OnPropertyChanged(nameof(UploadSuccess));
        }
    }

    private ObservableCollection<string> tags = new ObservableCollection<string>()
    {
        "#music",
        "#text"
    };

    [UndoRedo]
    public ObservableCollection<string> Tags
    {
        get { return tags; }
        set { Set(ref tags, value); }
    }

    [UndoRedo]
    public string FieldText
    {
        get { return fieldText; }
        set { Set(ref fieldText, value); }
    }

    public ButtonCommand AddTagsCommand
    {
        get { return new ButtonCommand((o => { Tags.Add("lalala"); })); }
    }


    public ButtonCommand PublishCommand
    {
        get
        {
            return new ButtonCommand(o =>
            {
                var post = new Post(Utils.GenerateRandomString(5), FieldText);




                UploadSuccess = new NotifyTaskCompletion<bool>(PlatformDAO.Instance.AddPost(post));

                FieldText = "";
            });
        }
    }
    
    
    public ButtonCommand LostFocusCommand
    {
        get
        {
            return new ButtonCommand((o =>
            {
                if (FieldText == "")
                {
                    FieldText = Utils.GetLocalizedName("LWriteSometh");
                }
            }));
        }
    }
    
    
    public ButtonCommand GotFocusCommand
    {
        get
        {
            return new ButtonCommand((o =>
            {
                if (FieldText == Utils.GetLocalizedName("LWriteSometh"))
                {
                    FieldText = "";
                }

                
            }));
        }
    }
    

    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}

[AttributeUsage(AttributeTargets.Property)]
class UndoRedoAttribute : Attribute
{
}

public abstract class HistoryVm : INotifyPropertyChanged
{
    protected bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;
        // Сюда
        SaveHistory(this, propertyName, field);
        field = value;
        NotifyPropertyChanged(propertyName);
        return true;
    }

    static bool isUndoProcess = false;
    static bool isRedoProcess = false;

    // Пара стеков для хранения истории
    static Stack<(object Obj, string Prop, object OldValue)> undoHistory
        = new Stack<(object Obj, string Prop, object OldValue)>();

    static Stack<(object Obj, string Prop, object OldValue)> redoHistory
        = new Stack<(object Obj, string Prop, object OldValue)>();

    static void Undo()
    {
        if (undoHistory.Count == 0) return;
        var undo = undoHistory.Pop();
        // Обернуто для того чтобы в случае исключения флаг всё равно снимался
        try
        {
            isUndoProcess = true;
            undo.Obj.GetType().GetProperty(undo.Prop).SetValue(undo.Obj, undo.OldValue);
        }
        finally
        {
            isUndoProcess = false;
        }
    }

    static void Redo()
    {
        if (redoHistory.Count == 0) return;
        var redo = redoHistory.Pop();
        try
        {
            isRedoProcess = true;
            redo.Obj.GetType().GetProperty(redo.Prop).SetValue(redo.Obj, redo.OldValue);
        }
        finally
        {
            isRedoProcess = false;
        }
    }

    static void SaveHistory(object obj, string propertyName, object value)
    {
        if (obj.GetType()
                .GetProperty(propertyName)
                .GetCustomAttributes(typeof(UndoRedoAttribute), true)
                .Length == 0) return;

        if (isUndoProcess)
        {
            redoHistory.Push((obj, propertyName, value));
        }
        else if (isRedoProcess)
        {
            undoHistory.Push((obj, propertyName, value));
        }
        else
        {
            undoHistory.Push((obj, propertyName, value));
            redoHistory.Clear();
        }
    }

    static void ClearHistory()
    {
        undoHistory.Clear();
        redoHistory.Clear();
    }

    // Команды, которые можно выставлять в GUI
    public static ButtonCommand UndoCommand { get; }
        = new ButtonCommand(_ => Undo(), _ => undoHistory.Count > 0);

    public static ButtonCommand RedoCommand { get; }
        = new ButtonCommand(_ => Redo(), _ => redoHistory.Count > 0);

    public static ButtonCommand ClearHistoryCommand { get; }
        = new ButtonCommand(_ => ClearHistory());


    protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public event PropertyChangedEventHandler PropertyChanged;
}

public class HistoryCommand : ICommand
{
    private Action<object> execute;
    private Func<object, bool> canExecute;

    public event EventHandler CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public HistoryCommand(Action<object> execute, Func<object, bool> canExecute = null)
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