namespace Wordle
{
    public partial class MainPage : ContentPage
    {
        bool GameActive = true;
        int x = 0;
        int y = 0;

        public MainPage()
        {
            InitializeComponent();
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
    }

}
