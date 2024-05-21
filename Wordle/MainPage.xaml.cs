using Microsoft.Maui.Controls.Shapes;
using Syncfusion.Maui.Popup;

namespace Wordle
{
    public partial class MainPage : ContentPage
    {
        bool GameActive;
        int x;
        int y;
        HashSet<string> wordList = new HashSet<string>();
        string word = "";
        Random rand = new Random();
        int streak = 0;
        List<int> statsList = new List<int>() { 0, 0, 0, 0, 0, 0 };

        public MainPage()
        {
            InitializeComponent();
            // Load the word list and start a new game
            Application.Current.Dispatcher.Dispatch(async () =>
            {
                await InitAsync();
            });

        }

        private void closeClicked(object? sender, EventArgs e) => popup.Dismiss();
        private void helpClicked(object? sender, EventArgs e) => popup.Show();
        private void focusEntry(object? sender, EventArgs e) => TextEntry.Focus();

        private async Task InitAsync()
        {
            var stream = await FileSystem.OpenAppPackageFileAsync("sgb-words.txt");

            if (stream != null)
            {
                StreamReader sr = new StreamReader(stream);
                string line = sr.ReadLine();
                //Continue to read until you reach end of file
                while (line != null)
                {
                    wordList.Add(line);
                    //Read the next line
                    line = sr.ReadLine();
                }
                sr.Close();
            }
            // Display instructions
            popup.Show();
            // Start the game by setting default values and picking a new word
            startGame();

            string targetFile = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, "filesave.txt");
            try
            {
                using (StreamReader stats = new StreamReader(targetFile))
                {
                    streak = int.Parse(stats.ReadLine());
                    for (int i = 0; i < 6; i++)
                    {
                        statsList[i] = int.Parse(stats.ReadLine());
                    }
                    stats.Close();
                }
            }
            catch
            {
                // Do nothing
            }
        }

        private async void waitRefocus(object? sender, EventArgs e)
        {
            await Task.Delay(100);
            TextEntry.Focus();
        }

        private void startGame()
        {
            GameActive = true;
            x = 0;
            y = 0;
            word = wordList.ElementAt(rand.Next(wordList.Count()));
            TextEntry.Focus();
            wordDisplay.Text = word.ToUpper();
        }

        private void RestartGame(object sender, EventArgs e)
        {
            // Restart Game
            for(int i = 0; i < 5; i++)
            {
                for(int j = 0; j < 6; j++)
                {
                    Label currBox = (Label)this.FindByName("Box" + j + i);
                    currBox.Text = "";
                    Border currBorder = (Border)this.FindByName("Border" + j + i);
                    currBorder.Stroke = Color.FromArgb("#59595a");
                    currBorder.BackgroundColor = Color.FromArgb("#121213");
                }
            }
            for(int i = 0; i < 26; i++)
            {
                Button currButton = (Button)this.FindByName((char)(i+65) + "Key");
                currButton.BackgroundColor = Color.FromArgb("#818384");
            }
            wordDisplayBorder.IsVisible = false;
            Restart.IsVisible = false;
            TextEntry.Text = "";
            startGame();
        }

        private void KeyClicked(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            TextEntry.Text += button.Text;
            TextEntry.Focus();
        }

        private void DelClicked(object sender, EventArgs e)
        {
            if (TextEntry.Text.Length > 0)
            {
                TextEntry.Text = TextEntry.Text.Substring(0, TextEntry.Text.Length - 1);
            }
        }

        private async void OnTextChanged(object sender, EventArgs e)
        {
            if (GameActive)
            {
                // Delete button clicked
                if(x > TextEntry.Text.Length)
                {
                    if (x != 0)
                    {
                        x--;
                    }
                    Label currBox = (Label)this.FindByName("Box" + y + x);
                    currBox.Text = "";
                    Border currBorder = (Border)this.FindByName("Border" + y + x);
                    currBorder.Stroke = Color.FromArgb("#59595a");
                }
                else
                {
                    if(TextEntry.Text.Length == 0)
                    {
                        return;
                    }
                    if (x < 5)
                    {
                        Label currBox = (Label)this.FindByName("Box" + y + x);
                        currBox.Text = TextEntry.Text[TextEntry.Text.Length - 1].ToString().ToUpper();
                        Border currBorder = (Border)this.FindByName("Border" + y + x);
                        x++;
                        await currBorder.ScaleTo(1.15, 50);
                        currBorder.Stroke = Color.FromArgb("#afafaf");
                        await currBorder.ScaleTo(1, 50);
                    }
                    else
                    {
                        TextEntry.Text = TextEntry.Text.Substring(0, 5);
                    }
                }
            }
        }

        private void OnEnter(object sender, EventArgs e)
        {
            if(x == 5)
            {
                if (wordList.Contains(TextEntry.Text.ToLower()))
                {
                    if (y < 6)
                    {
                        wordCheck();
                        y++;
                        x = 0;
                        TextEntry.Text = "";
                    }
                }
            }
        }

        private void wordCheck()
        {
            Dictionary<char, int> letterCount = new Dictionary<char, int>();
            List<(char, int)> yellowChars = new List<(char, int)>();
            for (int i = 0; i < 5; i++)
            {
                if (!letterCount.ContainsKey(word[i]))
                {
                    letterCount[word[i]] = 0;
                }
                letterCount[word[i]]++; ;
            }
            int correct = 0;
            string text = TextEntry.Text.ToLower();
            Dictionary<int, (Border, Color)> dict = new Dictionary<int, (Border, Color)>();
            for(int i = 0; i < 5; i++)
            {
                Border currBorder = (Border)this.FindByName("Border" + y + i);
                Button currButton = (Button)this.FindByName(text[i].ToString().ToUpper() + "Key");
                if (word[i] == text[i])
                {
                    dict[i] = (currBorder, Color.FromArgb("#6aaa64"));
                    //currBorder.BackgroundColor = Color.FromArgb("#6aaa64");
                    currButton.BackgroundColor = Color.FromArgb("#6aaa64");
                    correct++;
                    letterCount[word[i]]--;
                }
                else if (word.Contains(text[i]))
                {
                    yellowChars.Add((text[i], i));
                    currButton.BackgroundColor = Color.FromArgb("#c9b458");
                }
                else
                {
                    dict[i] = (currBorder, Color.FromArgb("#3a3a3c"));
                    //currBorder.BackgroundColor = Color.FromArgb("#3a3a3c");
                    currButton.BackgroundColor = Color.FromArgb("#3a3a3c");
                }
            }
            foreach((char, int) c in yellowChars)
            {
                Border currBorder = (Border)this.FindByName("Border" + y + c.Item2);
                if (letterCount[c.Item1] != 0)
                {
                    dict[c.Item2] = (currBorder, Color.FromArgb("#c9b458"));
                    //currBorder.BackgroundColor = Color.FromArgb("#c9b458");
                    letterCount[c.Item1]--;
                }
                else
                {
                    dict[c.Item2] = (currBorder, Color.FromArgb("#3a3a3c"));
                    //currBorder.BackgroundColor = Color.FromArgb("#3a3a3c");
                }
            }
            flipLetter(dict, correct);
        }

        private async void flipLetter(Dictionary<int, (Border, Color)> dict, int correct)
        {
            for (int i = 0; i < 5; i++)
            {
                await dict[i].Item1.ScaleYTo(0, 200);
                dict[i].Item1.BackgroundColor = dict[i].Item2;
                await dict[i].Item1.ScaleYTo(1, 200);
            }
            if(correct == 5)
            {
                writeStats(true);
                GameActive = false;
                correctPopup.Show();
                wordDisplayBorder.IsVisible = true;
                Restart.IsVisible = true;
            }
            else if(y == 6)
            {
                writeStats(false);
                GameActive = false;
                incorrectPopup.Show();
                wordDisplayBorder.IsVisible = true;
                Restart.IsVisible = true;
            }
        }

        private async void writeStats(bool correct)
        {
            if (correct)
            {
                streak++;
                statsList[y - 1]++;
                // Source: https://learn.microsoft.com/en-us/answers/questions/991205/how-to-write-a-text-file-in-maui
                string targetFile = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, "filesave.txt");
                using FileStream outputStream = File.OpenWrite(targetFile);
                using StreamWriter streamWriter = new StreamWriter(outputStream);
                await streamWriter.WriteLineAsync(streak.ToString());
                for (int i = 0; i < 6; i++)
                {
                    await streamWriter.WriteLineAsync(statsList[i].ToString());
                }
            }
            else
            {
                streak = 0;
                string targetFile = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, "filesave.txt");
                using FileStream outputStream = File.OpenWrite(targetFile);
                using StreamWriter streamWriter = new StreamWriter(outputStream);
                await streamWriter.WriteLineAsync(streak.ToString());
                for (int i = 0; i < 6; i++)
                {
                    await streamWriter.WriteLineAsync(statsList[i].ToString());
                }
            }
        }

        private void addStats(object sender, EventArgs e)
        {
            int max = statsList.Max();
            Button x = new Button
            {
                Text = "x",
                HorizontalOptions = LayoutOptions.End,
                TextColor = Colors.White,
                FontSize = 35,
                BackgroundColor = Color.FromArgb("#121213"),
                WidthRequest = 100,
                HeightRequest = 50,

            };
            var popup = (SfPopup)sender;
            x.Clicked +=  (sender, e) =>  popup.Dismiss();
            DataTemplate templateView = new DataTemplate(() =>
            {
                Border border = new Border
                {
                    WidthRequest = 400,
                    HeightRequest = 600,
                    Stroke = Color.FromArgb("#59595a"),
                    Margin = 1.5,
                    BackgroundColor = Color.FromArgb("#121213"),
                    StrokeShape = new RoundRectangle
                    {
                        CornerRadius = new CornerRadius(40, 40, 40, 40)
                    },
                    Content = new VerticalStackLayout
                    {
                        Children =
                        {
                            new BoxView
                            {
                                Color = Colors.Transparent,
                                Margin = new Thickness(0,20,0,0),
                                HeightRequest = 1,
                                HorizontalOptions = LayoutOptions.Fill
                            },
                            x,
                            new Image
                            {
                                Source = "correct.png",
                                Aspect = Aspect.AspectFill,
                                WidthRequest = 100,
                                HeightRequest = 100,
                                Margin = new Thickness(2)
                            },
                            new Label
                            {
                                Text = "Good Work!",
                                FontSize = 30,
                                HorizontalTextAlignment = TextAlignment.Center,
                                Margin = new Thickness(10)
                            },
                            new Label
                            {
                                Text = "Streak: " + streak,
                                FontSize = 25,
                                HorizontalTextAlignment = TextAlignment.Center,
                                FontAttributes = FontAttributes.Bold,
                                Margin = 5
                            },
                            new HorizontalStackLayout
                            {
                                Children =
                                {
                                    new BoxView
                                    {
                                        Color = Colors.Transparent,
                                        Margin = new Thickness(60,0,0,0),
                                        HeightRequest = 1,
                                        HorizontalOptions = LayoutOptions.Fill
                                    },
                                    new Label
                                    {
                                        Text = "1",
                                        FontSize = 20,
                                        Margin = 5
                                    },
                                    new Label
                                    {
                                        BackgroundColor = Colors.White,
                                        WidthRequest = 250 * ((double)statsList[0] / max),
                                        Margin = 2
                                    }
                                }
                            },
                            new HorizontalStackLayout
                            {
                                Children =
                                {
                                    new BoxView
                                    {
                                        Color = Colors.Transparent,
                                        Margin = new Thickness(60,0,0,0),
                                        HeightRequest = 1,
                                        HorizontalOptions = LayoutOptions.Fill
                                    },
                                    new Label
                                    {
                                        Text = "2",
                                        FontSize = 20,
                                        Margin = 5
                                    },
                                    new Label
                                    {
                                        BackgroundColor = Colors.White,
                                        WidthRequest = 250 * ((double)statsList[1] / max),
                                        Margin = 2
                                    }
                                }
                            },
                            new HorizontalStackLayout
                            {
                                Children =
                                {
                                    new BoxView
                                    {
                                        Color = Colors.Transparent,
                                        Margin = new Thickness(60,0,0,0),
                                        HeightRequest = 1,
                                        HorizontalOptions = LayoutOptions.Fill
                                    },
                                    new Label
                                    {
                                        Text = "3",
                                        FontSize = 20,
                                        Margin = 5
                                    },
                                    new Label
                                    {
                                        BackgroundColor = Colors.White,
                                        WidthRequest = 250 * ((double) statsList[2] / max),
                                        Margin = 2
                                    }
                                }
                            },
                            new HorizontalStackLayout
                            {
                                Children =
                                {
                                    new BoxView
                                    {
                                        Color = Colors.Transparent,
                                        Margin = new Thickness(60,0,0,0),
                                        HeightRequest = 1,
                                        HorizontalOptions = LayoutOptions.Fill
                                    },
                                    new Label
                                    {
                                        Text = "4",
                                        FontSize = 20,
                                        Margin = 5
                                    },
                                    new Label
                                    {
                                        BackgroundColor = Colors.White,
                                        WidthRequest = 250 * ((double)statsList[3] / max),
                                        Margin = 2
                                    }
                                }
                            },
                            new HorizontalStackLayout
                            {
                                Children =
                                {
                                    new BoxView
                                    {
                                        Color = Colors.Transparent,
                                        Margin = new Thickness(60,0,0,0),
                                        HeightRequest = 1,
                                        HorizontalOptions = LayoutOptions.Fill
                                    },
                                    new Label
                                    {
                                        Text = "5",
                                        FontSize = 20,
                                        Margin = 5
                                    },
                                    new Label
                                    {
                                        BackgroundColor = Colors.White,
                                        WidthRequest = 250 * ((double)statsList[4] / max),
                                        Margin = 2
                                    }
                                }
                            },
                            new HorizontalStackLayout
                            {
                                Children =
                                {
                                    new BoxView
                                    {
                                        Color = Colors.Transparent,
                                        Margin = new Thickness(60,0,0,0),
                                        HeightRequest = 1,
                                        HorizontalOptions = LayoutOptions.Fill
                                    },
                                    new Label
                                    {
                                        Text = "6",
                                        FontSize = 20,
                                        Margin = 5
                                    },
                                    new Label
                                    {
                                        BackgroundColor = Colors.White,
                                        WidthRequest = 250 * ((double)statsList[5] / max),
                                        Margin = 2
                                    }
                                }
                            },
                            new Label
                            {
                                Text = "Click The Retry",
                                FontSize = 20,
                                HorizontalTextAlignment = TextAlignment.Center,
                                Margin = new Thickness(5)
                            },
                            new Label
                            {
                                Text = "Button To Play Again",
                                FontSize = 20,
                                HorizontalTextAlignment = TextAlignment.Center,
                                Margin = new Thickness(5)
                            }
                        }
                    },
                };
                return border;
            });
            popup.ContentTemplate = templateView;
            popup.Refresh();
        }

        private void addICStats(object sender, EventArgs e)
        {
            int max = statsList.Max();
            Button x = new Button
            {
                Text = "x",
                HorizontalOptions = LayoutOptions.End,
                TextColor = Colors.White,
                FontSize = 35,
                BackgroundColor = Color.FromArgb("#121213"),
                WidthRequest = 100,
                HeightRequest = 50,

            };
            var popup = (SfPopup)sender;
            x.Clicked += (sender, e) => popup.Dismiss();
            DataTemplate templateView = new DataTemplate(() =>
            {
                Border border = new Border
                {
                    WidthRequest = 400,
                    HeightRequest = 600,
                    Stroke = Color.FromArgb("#59595a"),
                    Margin = 1.5,
                    BackgroundColor = Color.FromArgb("#121213"),
                    StrokeShape = new RoundRectangle
                    {
                        CornerRadius = new CornerRadius(40, 40, 40, 40)
                    },
                    Content = new VerticalStackLayout
                    {
                        Children =
                        {
                            new BoxView
                            {
                                Color = Colors.Transparent,
                                Margin = new Thickness(0,20,0,0),
                                HeightRequest = 1,
                                HorizontalOptions = LayoutOptions.Fill
                            },
                            x,
                            new Image
                            {
                                Source = "incorrect.png",
                                Aspect = Aspect.AspectFill,
                                WidthRequest = 100,
                                HeightRequest = 100,
                                Margin = new Thickness(2)
                            },
                            new Label
                            {
                                Text = "Nice Try!",
                                FontSize = 30,
                                HorizontalTextAlignment = TextAlignment.Center,
                                Margin = new Thickness(10)
                            },
                            new Label
                            {
                                Text = "Streak: " + streak,
                                FontSize = 25,
                                HorizontalTextAlignment = TextAlignment.Center,
                                FontAttributes = FontAttributes.Bold,
                                Margin = 5
                            },
                            new HorizontalStackLayout
                            {
                                Children =
                                {
                                    new BoxView
                                    {
                                        Color = Colors.Transparent,
                                        Margin = new Thickness(60,0,0,0),
                                        HeightRequest = 1,
                                        HorizontalOptions = LayoutOptions.Fill
                                    },
                                    new Label
                                    {
                                        Text = "1",
                                        FontSize = 20,
                                        Margin = 5
                                    },
                                    new Label
                                    {
                                        BackgroundColor = Colors.White,
                                        WidthRequest = 250 * ((double)statsList[0] / max),
                                        Margin = 2
                                    }
                                }
                            },
                            new HorizontalStackLayout
                            {
                                Children =
                                {
                                    new BoxView
                                    {
                                        Color = Colors.Transparent,
                                        Margin = new Thickness(60,0,0,0),
                                        HeightRequest = 1,
                                        HorizontalOptions = LayoutOptions.Fill
                                    },
                                    new Label
                                    {
                                        Text = "2",
                                        FontSize = 20,
                                        Margin = 5
                                    },
                                    new Label
                                    {
                                        BackgroundColor = Colors.White,
                                        WidthRequest = 250 * ((double)statsList[1] / max),
                                        Margin = 2
                                    }
                                }
                            },
                            new HorizontalStackLayout
                            {
                                Children =
                                {
                                    new BoxView
                                    {
                                        Color = Colors.Transparent,
                                        Margin = new Thickness(60,0,0,0),
                                        HeightRequest = 1,
                                        HorizontalOptions = LayoutOptions.Fill
                                    },
                                    new Label
                                    {
                                        Text = "3",
                                        FontSize = 20,
                                        Margin = 5
                                    },
                                    new Label
                                    {
                                        BackgroundColor = Colors.White,
                                        WidthRequest = 250 * ((double) statsList[2] / max),
                                        Margin = 2
                                    }
                                }
                            },
                            new HorizontalStackLayout
                            {
                                Children =
                                {
                                    new BoxView
                                    {
                                        Color = Colors.Transparent,
                                        Margin = new Thickness(60,0,0,0),
                                        HeightRequest = 1,
                                        HorizontalOptions = LayoutOptions.Fill
                                    },
                                    new Label
                                    {
                                        Text = "4",
                                        FontSize = 20,
                                        Margin = 5
                                    },
                                    new Label
                                    {
                                        BackgroundColor = Colors.White,
                                        WidthRequest = 250 * ((double)statsList[3] / max),
                                        Margin = 2
                                    }
                                }
                            },
                            new HorizontalStackLayout
                            {
                                Children =
                                {
                                    new BoxView
                                    {
                                        Color = Colors.Transparent,
                                        Margin = new Thickness(60,0,0,0),
                                        HeightRequest = 1,
                                        HorizontalOptions = LayoutOptions.Fill
                                    },
                                    new Label
                                    {
                                        Text = "5",
                                        FontSize = 20,
                                        Margin = 5
                                    },
                                    new Label
                                    {
                                        BackgroundColor = Colors.White,
                                        WidthRequest = 250 * ((double)statsList[4] / max),
                                        Margin = 2
                                    }
                                }
                            },
                            new HorizontalStackLayout
                            {
                                Children =
                                {
                                    new BoxView
                                    {
                                        Color = Colors.Transparent,
                                        Margin = new Thickness(60,0,0,0),
                                        HeightRequest = 1,
                                        HorizontalOptions = LayoutOptions.Fill
                                    },
                                    new Label
                                    {
                                        Text = "6",
                                        FontSize = 20,
                                        Margin = 5
                                    },
                                    new Label
                                    {
                                        BackgroundColor = Colors.White,
                                        WidthRequest = 250 * ((double)statsList[5] / max),
                                        Margin = 2
                                    }
                                }
                            },
                            new Label
                            {
                                Text = "Click The Retry",
                                FontSize = 20,
                                HorizontalTextAlignment = TextAlignment.Center,
                                Margin = new Thickness(5)
                            },
                            new Label
                            {
                                Text = "Button To Play Again",
                                FontSize = 20,
                                HorizontalTextAlignment = TextAlignment.Center,
                                Margin = new Thickness(5)
                            }
                        }
                    },
                };
                return border;
            });
            popup.ContentTemplate = templateView;
            popup.Refresh();
        }

        // TODO:
        //
        // Add animations for:
        //
        // Implement stat saving
        //
    }

}
