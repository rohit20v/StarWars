using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using StarWars.Utils;
using StarWars.Views;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace StarWars
{
    public sealed partial class HomePage : Page
    {
        private Yoda _yodaInstance;

        public HomePage()
        {
            this.InitializeComponent();
            _yodaInstance = new Yoda(YodaCursorCanvas);
            PersonFrame.Navigate(typeof(DataContainer), "people");
            PlanetFrame.Navigate(typeof(DataContainer), "planets");

        }


    }
}
