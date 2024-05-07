namespace Wordle;
using CommunityToolkit.Maui.Views;

public partial class IntroPopup : Popup
{
    public IntroPopup()
    {
        InitializeComponent();
    }

    private void closeClicked(object? sender, EventArgs e) => Close();
}