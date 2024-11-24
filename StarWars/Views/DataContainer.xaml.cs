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
            HomeworldSearchToggle.Visibility = _dataType.Equals("people") ? Visibility.Visible : Visibility.Collapsed;
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
            string searchText = SearchBox.Text.Trim().ToLower();

            if (!string.IsNullOrEmpty(searchText))
            {
                if (_dataType.Equals("people"))
                {
                    bool searchByHomeworld = HomeworldSearchToggle.IsOn;

                    if (searchByHomeworld)
                    {
                        var allCharacters = new List<Character>();
                        string url = "https://swapi.dev/api/people/";

                        while (!string.IsNullOrEmpty(url))
                        {
                            _characterResponse = HttpFetch.FetchApi<Character>(url);
                            if (_characterResponse != null && _characterResponse.Results != null)
                            {
                                foreach (var character in _characterResponse.Results)
                                {
                                    if (!string.IsNullOrEmpty(character.homeworld))
                                    {
                                        var planet = await HttpFetch.FetchSingleItemAsync<Planet>(character.homeworld);
                                        if (planet != null && planet.Name.ToLower().Contains(searchText))
                                        {
                                            allCharacters.Add(character);
                                        }
                                    }
                                }
                            }

                            url = _characterResponse?.Next;
                        }

                        _people = allCharacters;
                        DataList.ItemsSource = _people;
                        UpdateNavigationButtons(null, null);
                    }
                    else
                    {
                        string searchUrl = $"https://swapi.dev/api/people/?search={searchText}";
                        FetchCharacterData(searchUrl);
                    }
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

                ShowPopup("Vehicles", vehicles);
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
                        var starship = await HttpFetch.FetchSingleItemAsync<Starship>(url);
                        if (starship != null) starships.Add(starship);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Error fetching starship data: {ex.Message}");
                    }
                }

                ShowPopup("Starships", starships);
            }
        }

        private void ShowPopup<T>(string title, List<T> items)
        {
            var popup = new Popup
            {
                IsLightDismissEnabled = true,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            var rootGrid = new Grid
            {
                Width = Window.Current.Bounds.Width,
                Height = Window.Current.Bounds.Height
            };

            var semiTransparentBackdrop = new Grid
            {
                Width = Window.Current.Bounds.Width,
                Height = Window.Current.Bounds.Height,
                Background = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0))
            };

            semiTransparentBackdrop.Tapped += (s, e) => popup.IsOpen = false;

            var mainContainer = new Grid
            {
                MaxHeight = Window.Current.Bounds.Height * 0.8,
                MaxWidth = 400,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Background = new SolidColorBrush(Colors.Black),
                BorderBrush = new SolidColorBrush(Colors.Yellow),
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(20)
            };

            var contentPanel = new StackPanel
            {
                Spacing = 10
            };

            contentPanel.Children.Add(new TextBlock
            {
                Text = title,
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.Yellow),
                Margin = new Thickness(0, 0, 0, 10),
                HorizontalAlignment = HorizontalAlignment.Center
            });

            var scrollViewer = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                Padding = new Thickness(0, 0, 10, 0),
                MaxHeight = Window.Current.Bounds.Height * 0.6
            };

            var itemsPanel = new StackPanel
            {
                Spacing = 10
            };

            if (items == null || !items.Any())
            {
                itemsPanel.Children.Add(new TextBlock
                {
                    Text = "No Data Found",
                    Foreground = new SolidColorBrush(Colors.White),
                    Margin = new Thickness(0, 10, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Center
                });
            }
            else
            {
                foreach (var item in items)
                {
                    var properties = item.GetType().GetProperties();
                    var itemStack = new StackPanel
                    {
                        Margin = new Thickness(0, 5, 0, 5),
                        BorderBrush = new SolidColorBrush(Colors.Yellow),
                        BorderThickness = new Thickness(2),
                        Padding = new Thickness(10),
                        CornerRadius = new CornerRadius(5)
                    };

                    foreach (var prop in properties)
                    {
                        var value = prop.GetValue(item)?.ToString() ?? "N/A";

                        itemStack.Children.Add(new TextBlock
                        {
                            Text = $"{prop.Name}: {value}",
                            Foreground = new SolidColorBrush(Colors.White),
                            Margin = new Thickness(0, 2, 0, 2),
                            TextWrapping = TextWrapping.Wrap
                        });
                    }

                    itemsPanel.Children.Add(itemStack);
                }
            }

            scrollViewer.Content = itemsPanel;
            contentPanel.Children.Add(scrollViewer);

            var closeButton = new Button
            {
                Content = "Close",
                Background = new SolidColorBrush(Colors.Goldenrod),
                Foreground = new SolidColorBrush(Colors.White),
                BorderBrush = new SolidColorBrush(Colors.White),
                Padding = new Thickness(8),
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(4),
                Margin = new Thickness(0, 10, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            closeButton.Click += (s, e) => popup.IsOpen = false;
            contentPanel.Children.Add(closeButton);

            mainContainer.Children.Add(contentPanel);
            rootGrid.Children.Add(semiTransparentBackdrop);
            rootGrid.Children.Add(mainContainer);

            popup.Child = rootGrid;
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

        private async void ShowHomeworldButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var homeworldUrl = button?.Tag as string;

            if (!string.IsNullOrEmpty(homeworldUrl))
            {
                var planet = await HttpFetch.FetchSingleItemAsync<Planet>(homeworldUrl);
                if (planet != null)
                {
                    var planets = new List<Planet> { planet };
                    ShowPopup("Homeworld", planets);
                }
            }
        }
    }
}