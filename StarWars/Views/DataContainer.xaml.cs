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
using System.Runtime.InteropServices;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StarWars.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DataContainer : Page
    {
        private string _dataType;
        private ApiResponse<Character> _characterResponse = new();
        private ApiResponse<Planet> _planetResponse = new();
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

                _characterResponse = HttpFetch.FetchApi<Character>($"https://swapi.dev/api/{_dataType}");
                FetchCharacterData($"https://swapi.dev/api/{_dataType}");
            }

            else
            {
                DataList.ItemTemplate = this.Resources["PlanetTemplate"] as DataTemplate;

                FetchPlanetData($"https://swapi.dev/api/{_dataType}");
            }
        }

        private void FetchCharacterData(string url)
        {
            _characterResponse = HttpFetch.FetchApi<Character>(url);
            if (_characterResponse != null)
            {
                _people = _characterResponse.Results;
                UpdateNavigationButtons(_characterResponse.Next, _characterResponse.Previous);
                DataList.ItemsSource = _people;
            }
        }

        private void FetchPlanetData(string url)
        {
            _planetResponse = HttpFetch.FetchApi<Planet>(url);
            if (_planetResponse != null)
            {
                _planets = _planetResponse.Results;
                UpdateNavigationButtons(_planetResponse.Next, _planetResponse.Previous);

                DataList.ItemsSource = _planets;
            }
        }

        private async void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.Trim();

            if (!string.IsNullOrEmpty(searchText))
            {
                if (_dataType.Equals("people"))
                {
                    string searchUrl = $"https://swapi.dev/api/people/?search={searchText}";
                    FetchCharacterData(searchUrl);
                }
                else if (_dataType.Equals("planets"))
                {
                    string searchUrl = $"https://swapi.dev/api/planets/?search={searchText}";
                    FetchPlanetData(searchUrl);
                }
            }
            else
            {
                InitialFetch();
            }
        }

        private async void ShowVehiclesButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var character = button?.Tag as Character;

            if (character != null && character.VehiclesDataUrl != null)
            {
                var vehicles = new List<Vehicle>();
                foreach (var url in character.VehiclesDataUrl)
                {
                    try
                    {
                        var vehicle = await HttpFetch.FetchSingleItemAsync<Vehicle>(url);
                        if (vehicle != null) vehicles.Add(vehicle);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error fetching vehicle data: {ex.Message}");
                    }
                }

                ShowPopup("Vehicles", vehicles.Select(v => v.Name).ToList());
            }
        }

        private async void ShowStarshipsButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var character = button?.Tag as Character;

            if (character != null && character.StarshipsDataUrl != null)
            {
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

                ShowPopup("Starships", starships.Select(s => s.Name).ToList());
            }
        }


        private void ShowPopup(string title, List<string> items)
        {
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

            stackPanel.Children.Add(new TextBlock
            {
                Text = title,
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.Yellow),
                Margin = new Thickness(0, 0, 0, 10)
            });

            foreach (var item in items)
            {
                stackPanel.Children.Add(new TextBlock
                {
                    Text = "• " + item,
                    Foreground = new SolidColorBrush(Colors.White),
                    Margin = new Thickness(0, 5, 0, 5)
                });
            }

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

        private void UpdateNavigationButtons(string next, string previous)
        {
            Next.IsEnabled = !string.IsNullOrEmpty(next);
            Prev.IsEnabled = !string.IsNullOrEmpty(previous);

            Next.Tag = next;
            Prev.Tag = previous;
        }

        private async void PreviousPageButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            string previousUrl = button?.Tag?.ToString();

            if (!string.IsNullOrEmpty(previousUrl))
            {
                if (_dataType.Equals("people"))
                {
                    FetchCharacterData(previousUrl);
                }
                else
                {
                    FetchPlanetData(previousUrl);
                }
            }
        }

        private async void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            string nextUrl = button?.Tag?.ToString();

            if (!string.IsNullOrEmpty(nextUrl))
            {
                if (_dataType.Equals("people"))
                {
                    FetchCharacterData(nextUrl);
                }
                else
                {
                    FetchPlanetData(nextUrl);
                }
            }
        }

        private void SaveXml(object sender, RoutedEventArgs e)
        {
            if (_dataType.Equals("people"))
            {
                XmlWriter.WriteXmlFile<Character>(_people, "characters.xml");
            }
            else if (_dataType.Equals("planets"))
            {
                XmlWriter.WriteXmlFile<Planet>(_planets, "planets.xml");

            }
        }

        private void SaveJson(object sender, RoutedEventArgs e)
        {

            if (_dataType.Equals("people"))
            {
                JsonWriter.WriteJsonFile<Character>(_people, "characters.json");
            }
            else if (_dataType.Equals("planets"))
            {
                JsonWriter.WriteJsonFile(_planets, "planets.json");

            }
        }
    }
}