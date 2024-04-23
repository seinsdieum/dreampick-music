using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace dreampick_music;

public partial class SingleChoiceGroup : UserControl
{

    [Bindable(true)]
    public ObservableCollection<SingleChoice> ChoiceList
    {
        get => (ObservableCollection<SingleChoice>)GetValue(ChoiceListProperty);
        set => SetValue(ChoiceListProperty, (object) value);
    }
    
    public static readonly DependencyProperty ChoiceListProperty = 
        DependencyProperty.Register(nameof(ChoiceList), 
            typeof(ObservableCollection<SingleChoice>), 
            typeof(SingleChoiceGroup),
            new UIPropertyMetadata(new ObservableCollection<SingleChoice>()
                {
                    new SingleChoice("LMissing", (() =>
                    {
                        MessageBox.Show("FUck");
                    })),
                },
                (o, args) =>
                {
                    var group = o as SingleChoiceGroup;
                    group.OnChoiceListChanged(args);
                }));

    private void OnChoiceListChanged(DependencyPropertyChangedEventArgs e)
    {
        ChoiceList = (ObservableCollection<SingleChoice>)e.NewValue;
    }
    
    
    
    
    
    
    public SingleChoiceGroup()
    {
        InitializeComponent();
        DataContext = this;
    }


    // TODO use this
    private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
    {
        var button = sender as ToggleButton;
        var obj = button.DataContext as SingleChoice;

        for (int i = 0; i < ToggleItems.Items.Count; i++)
        {
            ContentPresenter c =
                (ContentPresenter)ToggleItems.ItemContainerGenerator.ContainerFromItem(ToggleItems.Items[i]);
            ToggleButton tb = c.ContentTemplate.FindName("Toggle", c) as ToggleButton;
            if (tb != button) tb.IsChecked = false;
        }

        button.IsChecked = true;

        obj.ExecuteChoice();
    }
}