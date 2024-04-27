using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Xaml.Behaviors;
using Microsoft.Xaml.Behaviors.Core;

namespace dreampick_music
{
    /// <summary>
    /// Window resizing interaction
    /// </summary>
    public partial class MainWindow : Window
    {

        #region WindowHandlers

                private const int WmSyscommand = 0x112;
        private HwndSource _hwndSource;

        private enum ResizeDirection
        {
            Left = 61441,
            Right = 61442,
            Top = 61443,
            TopLeft = 61444,
            TopRight = 61445,
            Bottom = 61446,
            BottomLeft = 61447,
            BottomRight = 61448,
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        private void Window1_SourceInitialized(object sender, EventArgs e)
        {
            _hwndSource = PresentationSource.FromVisual((Visual)sender) as HwndSource;
        }

        private void ResizeWindow(ResizeDirection direction)
        {
            SendMessage(_hwndSource.Handle, WmSyscommand, (IntPtr)direction, IntPtr.Zero);
        }

        protected void ResetCursor(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
            {
                Cursor = Cursors.Arrow;
            }
        }

        protected void Resize(object sender, MouseButtonEventArgs e)
        {
            var clickedShape = sender as Shape;

            if (clickedShape == null) return;
            switch (clickedShape.Name)
            {
                case "ResizeN":
                    Cursor = Cursors.SizeNS;
                    ResizeWindow(ResizeDirection.Top);
                    break;
                case "ResizeE":
                    Cursor = Cursors.SizeWE;
                    ResizeWindow(ResizeDirection.Right);
                    break;
                case "ResizeS":
                    Cursor = Cursors.SizeNS;
                    ResizeWindow(ResizeDirection.Bottom);
                    break;
                case "ResizeW":
                    Cursor = Cursors.SizeWE;
                    ResizeWindow(ResizeDirection.Left);
                    break;
                case "ResizeNW":
                    Cursor = Cursors.SizeNWSE;
                    ResizeWindow(ResizeDirection.TopLeft);
                    break;
                case "ResizeNE":
                    Cursor = Cursors.SizeNESW;
                    ResizeWindow(ResizeDirection.TopRight);
                    break;
                case "ResizeSE":
                    Cursor = Cursors.SizeNWSE;
                    ResizeWindow(ResizeDirection.BottomRight);
                    break;
                case "ResizeSW":
                    Cursor = Cursors.SizeNESW;
                    ResizeWindow(ResizeDirection.BottomLeft);
                    break;
            }
        }

        protected void DisplayResizeCursor(object sender, MouseEventArgs e)
        {
            var clickedShape = sender as Shape;

            if (clickedShape == null) return;
            switch (clickedShape.Name)
            {
                case "ResizeN":
                case "ResizeS":
                    Cursor = Cursors.SizeNS;
                    break;
                case "ResizeE":
                case "ResizeW":
                    Cursor = Cursors.SizeWE;
                    break;
                case "ResizeNW":
                case "ResizeSE":
                    Cursor = Cursors.SizeNWSE;
                    break;
                case "ResizeNE":
                case "ResizeSW":
                    Cursor = Cursors.SizeNESW;
                    break;
            }
        }

        #endregion
        
        
        public MainWindow()
        {
            SourceInitialized += Window1_SourceInitialized;

            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 0, 0,100);
            
            
            if(AudioPlayer is not null) AudioPlayer.MediaOpened += ((o, args) =>
            {
                if(TrackSlider is not null) TrackSlider.Maximum = AudioPlayer.NaturalDuration.TimeSpan.TotalSeconds;
            }) ;
        }



        #region MediaPlayerEvents

        private DispatcherTimer timer;

        private void timer_Tick(object sender, EventArgs e)
        {
            codeFactor = 0;
            ChangeTrackPositionSelection();
            if (System.Windows.Input.Mouse.LeftButton != MouseButtonState.Pressed &&
                TrackSlider != null && AudioPlayer != null) TrackSlider.Value = AudioPlayer.Position.TotalSeconds;

        }

        private TimeSpan trackPosition = TimeSpan.FromSeconds(0);

        public TimeSpan TrackPosition
        {
            get { return trackPosition; }
        }

        public void SongValue_OnChange(object? sender, EventArgs e)
        {
            if (AudioPlayer is { LoadedBehavior: MediaState.Play })
            {

                if (timer != null) timer.Start();
                else
                {
                    timer = new DispatcherTimer();
                    timer.Tick += new EventHandler(timer_Tick);
                    timer.Interval = new TimeSpan(0, 0, 0, 1);
                    timer.Start();
                }
            }
            else
            {
                if (timer != null) timer.Stop();
            }
        }


        private double codeFactor = 0.0;

        public double CodeFactor
        {
            get => codeFactor;
            set => codeFactor = value;
        }

        private double TrackSliderLastValue = 0;

        private void TrackSlider_OnLastValueChanged(object sender, DragCompletedEventArgs dragCompletedEventArgs)
        {
            if (TrackSlider != null && AudioPlayer != null)
                AudioPlayer.Position = TimeSpan.FromSeconds(TrackSliderLastValue);
            CodeFactor = 1.0;
        }

        private void ChangeTrackPositionSelection()
        {
            TrackSlider.SelectionStart = 0;

            if (!(TrackSlider.Value > 0)) return;
            if (System.Windows.Input.Mouse.LeftButton != MouseButtonState.Pressed &&
                TrackSlider != null && AudioPlayer != null)
            {
                TrackSlider.SelectionEnd = AudioPlayer.Position.TotalSeconds;  
            }
        }

        private void TrackSlider_OnValueChanged(object? sender, EventArgs eventArgs)
        {
            if (TrackSlider != null && AudioPlayer != null && CodeFactor == 1.0 &&
                Mouse.LeftButton == MouseButtonState.Pressed)
            {
                TrackSliderLastValue = TrackSlider.Value;
            }
            

            CodeFactor = 1.0;
        }
        private void TrackSlider_OnMouseReleased(object? sender, EventArgs eventArgs)
        {
            if (TrackSlider != null && AudioPlayer != null)
                AudioPlayer.Position = TimeSpan.FromSeconds(TrackSlider.Value);

            CodeFactor = 1.0;
        }

        #endregion

        private void PageFrameLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is Frame frame)
            {
                NavigationVm.Instance.Navigation = frame.NavigationService;
            }
        }
    }
}