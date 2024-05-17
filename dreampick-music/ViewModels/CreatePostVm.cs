using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using dreampick_music.DbRepositories;
using dreampick_music.Models;
using dreampick_music.Views;

namespace dreampick_music;

public class CreatePostVm : HistoryVm
{
    private PostRepository postRepo = new();
    private PlaylistRepository playlistRepo = new();


    private string fieldText = "";


    private NotifyTaskCompletion<bool> uploadSuccess;

    private NotifyTaskCompletion<DbContexts.Playlist> selectedPlaylist;

    [UndoRedo]
    public NotifyTaskCompletion<DbContexts.Playlist> SelectedPlaylist
    {
        get => selectedPlaylist;
        set => Set(ref selectedPlaylist, value);
    }

    public NotifyTaskCompletion<bool> UploadSuccess
    {
        get { return uploadSuccess; }
        set
        {
            uploadSuccess = value;
            NotifyPropertyChanged();
        }
    }


    [UndoRedo]
    public string FieldText
    {
        get { return fieldText; }
        set => Set(ref fieldText, value);
    }


    private async Task<bool> PublishPost()
    {
        postRepo.Add(new DbContexts.Post()
        {
            Id = Utils.GenerateRandomString(10),
            
            PlaylistId = SelectedPlaylist is not null && SelectedPlaylist.Result is not null
                ? SelectedPlaylist.Result.Id
                : null,
            Text = FieldText,
            CreatedOn = DateTime.Now,
            UserId = AccountVm.Instance.AccountPerson.Id,
            
        });


        return true;
    }

    public ButtonCommand PublishCommand => new ButtonCommand(o =>
    {
        if (string.IsNullOrEmpty(FieldText)) return;

        UploadSuccess = new NotifyTaskCompletion<bool>(PublishPost());

        FieldText = "";
    }, o => !string.IsNullOrEmpty(FieldText));

    private void SelectPlaylist(string id)
    {
        SelectedPlaylist = new NotifyTaskCompletion<DbContexts.Playlist>(playlistRepo.GetById(id));
    }

    public ButtonCommand NavigatePlaylistSelection => new ButtonCommand(o =>
    {
        
        WindowModel.OpenRelatedPlaylistsSelectionDialog(new PlaylistCollection(PlaylistCollectionType.Related, "", (o1 =>
        {
            if (o1 is not string id) return;
            SelectPlaylist(id);
        })));
    });

    public ButtonCommand RemovePlaylistCommand => new ButtonCommand(o => { SelectedPlaylist = null; });

    public ButtonCommand BackCommand => new ButtonCommand(o =>
    {
        NavigationVm.Instance.ClearNavigateBack(DestroyObjects);
    });

    private void DestroyObjects()
    {
        FieldText = "";
        UploadSuccess = null;
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