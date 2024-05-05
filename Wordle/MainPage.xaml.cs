namespace Wordle
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        bool GameActive = true;
        int x = 0;
        int y = 0;

        public MainPage()
        {
            InitializeComponent();
            //this.TextEntry.Focus();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }

        // Border with letter color: #afafaf

        private void OnTextChanged(object sender, EventArgs e)
        {
            if(TextEntry.Text[TextEntry.Text.Length - 1].ToString() == "\n")
            if (GameActive)
            {
                if(x < 5)
                {
                    Label currBox = (Label)this.FindByName("Box" + y + x);
                    currBox.Text = TextEntry.Text[TextEntry.Text.Length - 1].ToString().ToUpper();
                    x++;
                }
            }
        }
    }

}
