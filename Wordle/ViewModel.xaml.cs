using CommunityToolkit.Maui.Core;
using System.ComponentModel;

namespace Wordle;

public partial class ViewModel : INotifyPropertyChanged
{
    private readonly IPopupService popupService;

    public ViewModel(IPopupService popupService)
    {
        this.popupService = popupService;
    }

    public void DisplayPopup()
    {
        this.popupService.ShowPopup<ViewModel>();
    }
}