using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using Syncfusion.Maui.Core.Hosting;
using Microsoft.Maui.LifecycleEvents;
#if WINDOWS
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;
#endif

namespace Wordle
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>().ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            }).UseMauiCommunityToolkit();
            #if WINDOWS
            builder.ConfigureLifecycleEvents(events =>
            {
                events.AddWindows(windowsLifecycleBuilder =>
                {
                    windowsLifecycleBuilder.OnWindowCreated(window =>
                    {
                        window.ExtendsContentIntoTitleBar = false;
                        var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
                        var id = Win32Interop.GetWindowIdFromWindow(handle);
                        var appWindow = AppWindow.GetFromWindowId(id);
                        switch (appWindow.Presenter)
                        {
                            case OverlappedPresenter overlappedPresenter:
                                overlappedPresenter.SetBorderAndTitleBar(true, true);
                                overlappedPresenter.Maximize();
                                break;
                        }
                    });
                });
            });
            #endif
            builder.ConfigureSyncfusionCore();
#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}