using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

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
            
        };
        public ResourceDictionary ThemeDictionary
        {
            // You could probably get it via its name with some query logic as well.
            get { return Resources.MergedDictionaries[1]; }
        }
        
        public ResourceDictionary LocalizationDictionary
        {
            // You could probably get it via its name with some query logic as well.
            get { return Resources.MergedDictionaries[0]; }
        }
        

        public void ChangeTheme(string str)
        {
            UriResourcesStrings[0] = str;
            RefreshDictionaries();
        }
        
        public void ChangeLocalization(string str)
        {
            UriResourcesStrings[1] = str;
            RefreshDictionaries();
        }

        public void ChangeStyle(string str)
        {
            
        }
        

        private void RefreshDictionaries()
        {
            Resources.MergedDictionaries.Clear();
            Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(UriResourcesStrings[0], UriKind.Relative) });
            Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(UriResourcesStrings[1], UriKind.Relative) });
            Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(UriResourcesStrings[2], UriKind.Relative) });
            Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(UriResourcesStrings[3], UriKind.Relative) });
        }
    }
}