
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

        public MainPage()
        {
            InitializeComponent();
            string line;
            try
            {
                //Pass the file path and file name to the StreamReader constructor
                // TODO: Change this to a relative path
                StreamReader sr = new StreamReader("C:\\Users\\sukiw\\source\\repos\\Wordle\\Wordle\\Resources\\Raw\\sgb-words.txt");
                //Read the first line of text
                line = sr.ReadLine();
                //Continue to read until you reach end of file
                while (line != null)
                {
                    wordList.Add(line);
                    //Read the next line
                    line = sr.ReadLine();
                }
                //close the file
                sr.Close();
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            // Display instructions
            //DisplayPopup();
            popup.Show();
            // Start the game by setting default values and picking a new word
            startGame();
        }

        private void closeClicked(object? sender, EventArgs e) => popup.Dismiss();
        private void helpClicked(object? sender, EventArgs e) => popup.Show();
        private void focusEntry(object? sender, EventArgs e) => TextEntry.Focus();
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


        private void OnTextChanged(object sender, EventArgs e)
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
                        currBorder.Stroke = Color.FromArgb("#afafaf");
                        x++;
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
                    if (y == 6)
                    {
                        GameActive = false;
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
            for(int i = 0; i < 5; i++)
            {
                Border currBorder = (Border)this.FindByName("Border" + y + i);
                if (word[i] == text[i])
                {
                    currBorder.BackgroundColor = Color.FromArgb("#6aaa64");
                    correct++;
                    letterCount[word[i]]--;
                }
                else if (word.Contains(text[i]))
                {
                    //currBorder.BackgroundColor = Color.FromArgb("#c9b458");
                    yellowChars.Add((text[i], i));
                }
                else
                {
                    currBorder.BackgroundColor = Color.FromArgb("#3a3a3c");
                }
            }
            foreach((char, int) c in yellowChars)
            {
                Border currBorder = (Border)this.FindByName("Border" + y + c.Item2);
                if (letterCount[c.Item1] != 0)
                {
                    currBorder.BackgroundColor = Color.FromArgb("#c9b458");
                    letterCount[c.Item1]--;
                }
                else
                {
                    currBorder.BackgroundColor = Color.FromArgb("#3a3a3c");
                }
            }
            if(correct == 5)
            {
                GameActive = false;
            }
        }

        // TODO:
        // On Enter isn't finished
        //
        // Add animations
        //
        // Add ending screen w/ stats
    }

}
