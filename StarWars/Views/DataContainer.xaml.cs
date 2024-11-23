using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using StarWars.Models;
using StarWars.Utils;
using System.Threading.Tasks;
using Windows.UI.Text;
using Windows.UI;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StarWars.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DataContainer : Page
    {
        private string _dataType;
        private List<Character> _people = [];
        private List<Planet> _planets = [];

        public DataContainer()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _dataType = e.Parameter as string;
            InitialFetch();
        }

        private void InitialFetch()
        {
            if (_dataType.Equals("people"))
            {
                DataList.ItemTemplate = this.Resources["CharacterTemplate"] as DataTemplate;

                _people = HttpFetch.FetchApi<Character>($"https://swapi.dev/api/{_dataType}");
 
                DataList.ItemsSource = _people;
                Debug.WriteLine(_people[0].VehiclesDataUrl.Count);
            }

            else
            {
                DataList.ItemTemplate = this.Resources["PlanetTemplate"] as DataTemplate;

                _planets = HttpFetch.FetchApi<Planet>($"https://swapi.dev/api/{_dataType}");
                DataList.ItemsSource = _planets;
                Debug.WriteLine(_planets[0].Name);
            }
        }


        private async void ShowVehiclesButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var character = button?.Tag as Character;

            if (character != null && character.VehiclesDataUrl != null)
            {
                // Fetch vehicle data for the selected character
                var vehicles = new List<Vehicle>();
                foreach (var url in character.VehiclesDataUrl)
                {
                    try
                    {
                        var vehicle = await HttpFetch.FetchSingleItemAsync<Vehicle>(url); // Use async call
                        if (vehicle != null) vehicles.Add(vehicle);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error fetching vehicle data: {ex.Message}");
                    }
                }

                // Show vehicle data in a popup
                ShowPopup("Vehicles", vehicles.Select(v => v.Name).ToList());
            }
        }

        private async void ShowStarshipsButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var character = button?.Tag as Character;

            if (character != null && character.StarshipsDataUrl != null)
            {
                // Fetch starship data for the selected character
                var starships = new List<Starship>();
                foreach (var url in character.StarshipsDataUrl)
                {
                    try
                    {
                        var starship = await HttpFetch.FetchSingleItemAsync<Starship>(url); // Use async call
                        if (starship != null) starships.Add(starship);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error fetching starship data: {ex.Message}");
                    }
                }

                // Show starship data in a popup
                ShowPopup("Starships", starships.Select(s => s.Name).ToList());
            }
        }


        private void ShowPopup(string title, List<string> items)
        {
            // Create a simple Popup with a List of items
            var popup = new Popup
            {
                IsLightDismissEnabled = true,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            var stackPanel = new StackPanel
            {
                Background = new SolidColorBrush(Colors.Black),
                Padding = new Thickness(20),
                CornerRadius = new CornerRadius(10),
                MaxWidth = 300,
                BorderBrush = new SolidColorBrush(Colors.Yellow),
                BorderThickness = new Thickness(2)
            };

            // Title
            stackPanel.Children.Add(new TextBlock
            {
                Text = title,
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.Yellow),
                Margin = new Thickness(0, 0, 0, 10)
            });

            // Items
            foreach (var item in items)
            {
                stackPanel.Children.Add(new TextBlock
                {
                    Text = "• " + item,
                    Foreground = new SolidColorBrush(Colors.White),
                    Margin = new Thickness(0, 5, 0, 5)
                });
            }

            // Close Button
            var closeButton = new Button
            {
                Content = "Close",
                Background = new SolidColorBrush(Colors.Gray),
                Foreground = new SolidColorBrush(Colors.White),
                Margin = new Thickness(0, 10, 0, 0)
            };

            closeButton.Click += (s, e) => popup.IsOpen = false;
            stackPanel.Children.Add(closeButton);

            popup.Child = stackPanel;
            popup.IsOpen = true;
        }


    }
}