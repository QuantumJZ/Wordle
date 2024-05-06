using System.IO;

namespace Wordle
{
    public partial class MainPage : ContentPage
    {
        bool GameActive;
        int x;
        int y;
        List<string> wordList = new List<string>();
        string word = "";
        Random rand = new Random();

        public MainPage()
        {
            InitializeComponent();
            string line;
            try
            {
                //Pass the file path and file name to the StreamReader constructor
                StreamReader sr = new StreamReader("Resources\\Raw\\sgb-words.txt");
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
            // Start the game by setting default values and picking a new word
            startGame();
        }

        private void startGame()
        {
            GameActive = true;
            x = 0;
            y = 0;
            word = wordList[rand.Next(wordList.Count())];
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
                    TextEntry.Text = TextEntry.Text.Substring(0, x);
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
                if(y < 6)
                {
                    y++;
                    x = 0;
                    TextEntry.Text = "";
                }
            }
        }

        // TODO:
        // On Enter isn't finished
        // Implement word checking
    }

}
