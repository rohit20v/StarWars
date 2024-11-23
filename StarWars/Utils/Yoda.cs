using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml;

namespace StarWars.Utils
{
    public class Yoda
    {
        private UIElement _customCursorElement;
        private DispatcherTimer _cursorTimer;
        private double _targetX;
        private double _targetY;
        private double _currentX;
        private double _currentY;

        public Yoda(Canvas canvas)
        {
            CreateCustomCursor(canvas);

            _cursorTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16) 
            };
            _cursorTimer.Tick += UpdateCursorPosition;
            _cursorTimer.Start();

            Window.Current.CoreWindow.PointerMoved += CoreWindow_PointerMoved;
        }

        private void CreateCustomCursor(Canvas canvas)
        {
            var customCursorImage = new Image
            {
                Width = 32,
                Height = 32,
                Source = new BitmapImage(new Uri("ms-appx:///Assets/yoda.png"))
            };

            _customCursorElement = customCursorImage;
            canvas.Children.Add(_customCursorElement);
            Canvas.SetZIndex(_customCursorElement, 1000); 
        }

        private void CoreWindow_PointerMoved(CoreWindow sender, PointerEventArgs args)
        {
            _targetX = args.CurrentPoint.Position.X + 5;
            _targetY = args.CurrentPoint.Position.Y + 5; 
        }

        private void UpdateCursorPosition(object sender, object e)
        {
            const double smoothingFactor = 0.25; 
            _currentX += (_targetX - _currentX) * smoothingFactor;
            _currentY += (_targetY - _currentY) * smoothingFactor;

            Canvas.SetLeft(_customCursorElement, _currentX);
            Canvas.SetTop(_customCursorElement, _currentY);
        }
    }
}
