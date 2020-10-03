using Android.Content.Res;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Xamarin.Forms.Platform.Android;

namespace NavbarAnimation.Droid
{
    // https://www.andrewhoefling.com/Blog/Post/xamarin-forms-shell-customizing-the-tabbar-android

    public class CustomShellItemRenderer : ShellItemRenderer
    {
        FrameLayout shellOverlay;
        View outerlayout;
        BottomNavigationView bottomView;
        IVisualElementRenderer viewRenderer;

        public CustomShellItemRenderer(IShellContext shellContext) : base(shellContext)
        {
        }

        public override Android.Views.View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            outerlayout = base.OnCreateView(inflater, container, savedInstanceState);
            bottomView = outerlayout.FindViewById<BottomNavigationView>(Resource.Id.bottomtab_tabbar);
            shellOverlay = outerlayout.FindViewById<FrameLayout>(Resource.Id.bottomtab_tabbar_container);

            if (ShellItem is CustomTabBar todoTabBar && todoTabBar.CustomView != null)
                SetupLargeTab();

            return outerlayout;
        }

        private void SetupLargeTab()
        {
            var todoTabBar = (CustomTabBar)ShellItem;

            bottomView.Measure((int)MeasureSpecMode.Unspecified, (int)MeasureSpecMode.Unspecified);

            outerlayout.Measure((int)MeasureSpecMode.Unspecified, (int)MeasureSpecMode.Unspecified);

            shellOverlay.RemoveAllViews();
            shellOverlay.AddView(ConvertFormsToNative(todoTabBar.CustomView, 
                new Xamarin.Forms.Rectangle(0, 0, Resources.DisplayMetrics.WidthPixels / Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Density, (double)bottomView?.MeasuredHeight / Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Density)));
            // I use Resources.DisplayMetrics.WidthPixels because I want the width of the screen without the height of the navigation bar.
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);

            SetupLargeTab();
        }

        private View ConvertFormsToNative(Xamarin.Forms.View view, Xamarin.Forms.Rectangle size)
        {
            viewRenderer = Platform.CreateRendererWithContext(view, Context);
            var viewGroup = viewRenderer.View;
            viewRenderer.Tracker.UpdateLayout();

            if (view.HeightRequest > 0)
                size.Height = view.HeightRequest;

            var layoutParams = new ViewGroup.LayoutParams((int)(size.Width * Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Density), (int)(size.Height * Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Density));
            viewGroup.LayoutParameters = layoutParams;
            view.Layout(size);
            viewGroup.Layout(0, 0, (int)view.Width, (int)view.Height);
            return viewGroup;
        }
    }
}