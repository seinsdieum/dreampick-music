using System.ComponentModel;

namespace dreampick_music;

public class AccountVm : INotifyPropertyChanged
{
    
    
    
    
    
    
    public event PropertyChangedEventHandler PropertyChanged = delegate { };

    public void OnPropertyChanged(string prop)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}