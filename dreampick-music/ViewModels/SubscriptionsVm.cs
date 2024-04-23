using System.Collections.ObjectModel;
using System.ComponentModel;

namespace dreampick_music;

public class SubscriptionsVm : INotifyPropertyChanged
{
    public ObservableCollection<Person> Subscriptions { get; set; } = new ObservableCollection<Person>();
    
    
    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}