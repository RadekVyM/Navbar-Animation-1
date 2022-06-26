#if IOS

using Microsoft.Maui.Controls.Compatibility.Platform.iOS;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Platform;
using UIKit;

namespace NavbarAnimation.Maui.OverlayShell
{
    public class OverlayShellRenderer : ShellRenderer
    {
        UIView overlay = null;

        protected override async Task OnCurrentItemChangedAsync()
        {
            await base.OnCurrentItemChangedAsync();

            UpdateOverlay();
        }

        // TODO: Work in progress
        private void UpdateOverlay()
        {
            var overlayShell = Element as OverlayShell;
            var newOverlay = overlayShell.Overlay.ToPlatform(Element.Handler.MauiContext);

            if (overlay is not null)
            {
                View.WillRemoveSubview(overlay);
            }

            overlay = newOverlay;
            View.AddSubview(overlay);
            overlay.Frame = View.Bounds;

            View.BringSubviewToFront(overlay);
        }
    }
}

#endif