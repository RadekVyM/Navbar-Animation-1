#if WINDOWS

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Handlers;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WGrid = Microsoft.UI.Xaml.Controls.Grid;

namespace NavbarAnimation.Maui.OverlayShell
{
    public class OverlayShellHandler : ShellHandler
    {
        Canvas overlayCanvas = null;
        WGrid overlayContainer = null;

        public override void SetVirtualView(IView view)
        {
            base.SetVirtualView(view);

            overlayCanvas = new Canvas();
            overlayContainer = new WGrid();

            overlayCanvas.HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Stretch;
            overlayCanvas.VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Stretch;
            overlayCanvas.Height = 0;

            overlayCanvas.Children.Add(overlayContainer);

            PlatformView.SizeChanged += (s, e) =>
            {
                overlayContainer.Width = e.NewSize.Width;
                overlayContainer.Height = e.NewSize.Height;
            };

            PlatformView.ContentOverlay = overlayCanvas;
            UpdateOverlay();
        }

        protected override void ConnectHandler(ShellView platformView)
        {
            base.ConnectHandler(platformView);
            platformView.SizeChanged += OnPlatformViewSizeChanged;
        }

        protected override void DisconnectHandler(ShellView platformView)
        {
            base.DisconnectHandler(platformView);
            platformView.SizeChanged -= OnPlatformViewSizeChanged;
        }

        private void OnPlatformViewSizeChanged(object sender, SizeChangedEventArgs e)
        {
            overlayContainer.Width = e.NewSize.Width;
            overlayContainer.Height = e.NewSize.Height;
        }

        private void UpdateOverlay()
        {
            var underBarsShell = VirtualView as OverlayShell;

            var overlay = underBarsShell.Overlay.ToPlatform(underBarsShell.Handler.MauiContext);
            overlayContainer.Children.Clear();
            overlayContainer.Children.Add(overlay);
        }

        public override void UpdateValue(string property)
        {
            base.UpdateValue(property);

            if (property == OverlayShell.OverlayProperty.PropertyName)
            {
                UpdateOverlay();
            }
        }
    }
}

#endif
