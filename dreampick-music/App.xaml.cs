using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Formats.Tar;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using dreampick_music.Models;

namespace dreampick_music
{

    public partial class App : Application
    {
        
        
        private List<string> UriResourcesStrings = new List<string>()
        {
            "Resources/Theme/Theme.Default.xaml",
            "Resources/Localization/Local.En.xaml",
            "Resources/Style/Default.xaml",
            "Resources/Theme/Sizes.Default.xaml",
            "Resources/Style/TitleBar.xaml",
            "Resources/Controls/Icons.xaml",
            
        };

        public App()
        {
            new Posts();
        }
        
    }
}