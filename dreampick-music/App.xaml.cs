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


// main
// todo user subscription
// todo recommendations 
// todo tracks sorting
// Errors
// TODO fix exception on going to account while not loaded
// TODO test for errors

// UI

// TODO BigScreen mode
// TODO editalbum page UI
// TODO replace all track listviews to TrackCollectionPage's one

// TODO localize app
// TODO beautiful UI/UX for pages
// TODO add adaptivity
// TODO fix elements and gaps,
// TODO set up styles,
// TODO replace the most properties values with equal DynamicResource,

// Person
// TODO add header to artist (if possible)

// Pages and Navigation
// TODO collection page: tracks, albums, recommended
// TODO create DisposablePageBase class, that is implemented for pages that would like to clear its content automatically
// TODO add refresh to each source we need

// Tracks
// TODO implement using tracks queue navigation
// TODO implement track playing modes: shuffle, repeat-queue, repeat-one, no-repeat
// TODO add genre and publish time
// TODO implement beuatiful lyrics sync (if possible)

// Playlists
// TODO add function
// TODO create some track picks
// TODO user_playlist (if possible)
// TODO create recommendations(if possible)

// Post
// TODO CreatePost: add playlist binding via dialog window etc
// TODO albums in feed binding

// Settings
// TODO things serialization and deserialization

// Auth 
// TODO auto-auth if have saved token that is not expired(if possible)
// TODO add full name field (if possible)


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
            "Resources/Converters/Converters.xaml"
            
        };

        public App()
        {
        }
        
    }
}