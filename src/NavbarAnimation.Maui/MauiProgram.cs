using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using NavbarAnimation.Maui.OverlayShell;

namespace NavbarAnimation.Maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Comfortaa-Regular.ttf", "RegularFont");
                    fonts.AddFont("Comfortaa-Bold.ttf", "BoldFont");
                    fonts.AddFont("Comfortaa-Medium.ttf", "MediumFont");
                    fonts.AddFont("Comfortaa-SemiBold.ttf", "SemiBoldFont");
                })
                .ConfigureMauiHandlers(handlers =>
                {
#if ANDROID
                    handlers.TryAddHandler(typeof(OverlayShell.OverlayShell), typeof(OverlayShellRenderer));
#elif IOS || MACCATALYST
                    handlers.TryAddHandler(typeof(OverlayShell.OverlayShell), typeof(OverlayShellRenderer));
#elif WINDOWS
                    handlers.TryAddHandler(typeof(OverlayShell.OverlayShell), typeof(OverlayShellHandler));
#endif
                });

            return builder.Build();
        }
    }
}