using Microsoft.Maui.Controls.Shapes;

namespace NavbarAnimation.Maui.Views.Controls;

public partial class TabBarIconView : ContentView
{
    public PageType Page { get; set; }
    public Geometry Path
    {
        get => (Geometry)GetValue(PathProperty);
        set => SetValue(PathProperty, value);
    }

    public static readonly BindableProperty PathProperty =
        BindableProperty.Create(nameof(Path), typeof(Geometry), typeof(TabBarIconView), default(Geometry), BindingMode.OneWay);

    public TabBarIconView()
	{
		InitializeComponent();
        BindingContext = this;
	}
}