using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;

namespace dreampick_music;

public class SettingsVm: INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged = delegate { };


    private ObservableCollection<SingleChoice> _themeTabs;
    public ObservableCollection<SingleChoice> ThemeTabs
    {
        get
        {
            return _themeTabs =  new ObservableCollection<SingleChoice>()
            {
                new SingleChoice("LLight", () =>
                {
                    Config.Instance.ChangeTheme("Resources/Theme/Theme.Light.xaml");
                }),
                new SingleChoice("LDark", () =>
                {
                    Config.Instance.ChangeTheme("Resources/Theme/Theme.Default.xaml");
                }),
                new SingleChoice("LPurple", () =>
                {
                    Config.Instance.ChangeTheme("Resources/Theme/Theme.Purple.xaml");
                }),
                new SingleChoice("LGreen", () =>
                {
                    Config.Instance.ChangeTheme("Resources/Theme/Theme.Green.xaml");
                }),
            } ;
        }
        set
        {
            _themeTabs = value;
            OnPropertyChanged("ThemeTabs");
        }
    }

    public ObservableCollection<SingleChoice> TitlebarTabs
    {
        get
        {
            return new ObservableCollection<SingleChoice>()
            {
                new SingleChoice("LDefault", () =>
                {
                    Config.Instance.SwitchTitleBar(true);
                }),
                new SingleChoice("LNew", () =>
                {
                    Config.Instance.SwitchTitleBar(false);
                })
            };
        }
    }


    private List<string> someList;

    public List<string> SomeList
    {
        get
        {
            return someList ??= new List<string>()
            {
                "lalkaa",
                "NOOO",
            };
        }
        set
        {
            someList = value; OnPropertyChanged("SomeList");
        }
    }

    public ButtonCommand SomeCommand
    {
       get
       {
           return new ButtonCommand((o =>
           {
               SomeList = new List<string>()
               {
                   "Refreshed Successfully!",
                   "Refreshed Yeah",
               };
           }));
       }
    }


    public ObservableCollection<SingleChoice> LocalizationTabs
    {
        get
        {
            return new ObservableCollection<SingleChoice>()
            {
                new SingleChoice("LRussian", () =>
                {
                    Config.Instance.ChangeLocalization("Resources/Localization/Local.Ru.xaml");
                }),
                new SingleChoice("LEnglish", () =>
                {
                    Config.Instance.ChangeLocalization("Resources/Localization/Local.En.xaml");

                })
            };
        }
    }

    public SettingsVm()
    {
        OnPropertyChanged(nameof(ThemeTabs));
    }
    
    
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    

    private string localThemeName;
    public string LocalThemeName
    {
        get { return localThemeName; }
        set { localThemeName = value; OnPropertyChanged("ThemeChoice"); }
    }
    
    private string localTitlebar;
    public string LocalTitlebar
    {
        get { return localTitlebar; }
        set { localTitlebar = value; OnPropertyChanged("TitlebarChoice"); }
    }
    
    
    
}