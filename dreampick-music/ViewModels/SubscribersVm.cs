using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace dreampick_music;

public class SubscribersVm : INotifyPropertyChanged
{

    public ObservableCollection<Person> Subscribers { get; set; } = new ObservableCollection<Person>();

    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}