#if ANDROID
using Android.Content;
using Android.Views;
using AndroidX.Fragment.App;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.Platform.Compatibility;
using Microsoft.Maui.Platform;
using System.ComponentModel;
using AView = Android.Views.View;

namespace NavbarAnimation.Maui.OverlayShell
{
    public class OverlayShellRenderer : ShellRenderer
    {
        AView overlay = null;
        CustomFrameLayout frame = null;

        public OverlayShellRenderer()
        {
        }

        public OverlayShellRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == OverlayShell.OverlayProperty.PropertyName)
                UpdateOverlay();
        }

        protected override void SwitchFragment(FragmentManager manager, AView targetView, ShellItem newItem, bool animate = true)
        {
            // For more details see https://github.dev/dotnet/maui/blob/41bb8c96490c137b8fcd19de281a0c1e69bc9ade/src/Controls/src/Core/Compatibility/Handlers/Shell/Android/ShellRenderer.cs
            targetView.SetFitsSystemWindows(false);
            base.SwitchFragment(manager, targetView, newItem, animate);

            frame = targetView as CustomFrameLayout;

            UpdateOverlay();
        }

        private void UpdateOverlay()
        {
            if (frame is null)
                return;

            var overlayShell = Element as OverlayShell;

            if (overlay is null)
            {
                overlay = overlayShell.Overlay.ToPlatform(overlayShell.Handler.MauiContext);

                frame.AddView(overlay);

                frame.ChildViewAdded -= FrameChildViewAdded;
            }
            else
            {
                frame.RemoveView(overlay);
                overlay = overlayShell.Overlay.ToPlatform(overlayShell.Handler.MauiContext);

                frame.ChildViewAdded -= FrameChildViewAdded;
                frame.AddView(overlay);
            }

            frame.ChildViewAdded += FrameChildViewAdded;
            BringOverlayToFront(frame);
        }

        private void FrameChildViewAdded(object sender, ViewGroup.ChildViewAddedEventArgs e)
        {
            var frame = e.Parent as CustomFrameLayout;
            BringOverlayToFront(frame);
        }

        private void BringOverlayToFront(CustomFrameLayout frame)
        {
            for (int i = 0; i < frame.ChildCount; i++)
            {
                var child = frame.GetChildAt(i);

                if (child == overlay)
                {
                    overlay.BringToFront();
                    return;
                }
            }
        }
    }
}

#endif
