namespace NavbarAnimation.Maui.OverlayShell
{
    public class OverlayShell : Shell
    {
        public IView Overlay
        {
            get => (IView)GetValue(OverlayProperty);
            set => SetValue(OverlayProperty, value);
        }

        public static readonly BindableProperty OverlayProperty =
            BindableProperty.Create(nameof(Overlay), typeof(IView), typeof(OverlayShell), default(IView), BindingMode.OneWay);

        public OverlayShell() : base()
        {
        }
    }
}
