using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;

namespace dreampick_music;

public class Config
{
    private App app = (App)Application.Current;
    public static Config Instance = new Config();

    private string dbString = "Data Source=MOONFADE;Initial Catalog=MUSIC_PLATFORM;Integrated Security=True;";
    private void BuildSqlString()
    {
        var builder = new SqlConnectionStringBuilder();
        builder.DataSource = "MOONFADE";
        builder.TrustServerCertificate = true;
        builder.IntegratedSecurity = true;
        builder.InitialCatalog = "MUSIC_PLATFORM";
        dbString = builder.ToString();
    }

    public string DbString => dbString;

    public List<string> DictionarySources = new List<string>()
    {
        "Resources/Theme/Theme.Default.xaml",
        "Resources/Localization/Local.En.xaml",
        "Resources/Style/Default.xaml",
        "Resources/Theme/Sizes.Default.xaml",
        "Resources/Style/TitleBar.xaml",
        "Resources/Controls/Icons.xaml",
        "Resources/Converters/Converters.xaml",
    };

    
    
    

    public Config()
    {
        BuildSqlString();
        
    }

    public void InitializeSettings()
    {
        
    }
    
    public void ChangeTheme(string str)
    {
        DictionarySources[0] = str;
        RefreshDictionaries();
    }
        
    public void ChangeLocalization(string str)
    {
        DictionarySources[1] = str;
        RefreshDictionaries();
    }
        
    public void SwitchTitleBar(bool defaultTitleBar)
    {
        if(defaultTitleBar) DictionarySources[4] = "Resources/Style/TitleBar.xaml";
        else DictionarySources[4] = "Resources/Style/TitleBar.New.xaml";
        RefreshDictionaries();
    }
    

    private void RefreshDictionaries()
    {
        app.Resources.MergedDictionaries.Clear();
        foreach (var source in DictionarySources)
        {
            app.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(source, UriKind.Relative) });
        }
    }
}