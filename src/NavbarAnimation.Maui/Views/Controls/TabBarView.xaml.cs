using SimpleToolkit.Core;

namespace NavbarAnimation.Maui.Views.Controls;

public partial class TabBarView : ContentView
{
    public const double TabsHeight = 80;
    public const double IconHeight = 20;

    readonly Color barColor;
    readonly Color circleColor;

    bool initialChange = true;
    double selectedIconTranslation =>
        ((CalculateInnerRadius((float)backGraphicsView.Height, TabsPadding) * 2) - IconHeight) / 2;
    double defaultIconTranslation =>
        ((CalculateTabsHeight((float)backGraphicsView.Height, TabsPadding) -
        CalculateInnerRadius((float)backGraphicsView.Height, TabsPadding) - IconHeight) / 2) +
        CalculateInnerRadius((float)backGraphicsView.Height, TabsPadding);

    TabBarViewDrawable drawable = null;
    ContentButton currentButton = null;
    TabBarIconView currentIconView => currentButton.Content as TabBarIconView;

    public static readonly BindableProperty TabsPaddingProperty =
        BindableProperty.Create(nameof(TabsPadding), typeof(Thickness), typeof(TabBarView), defaultValue: Thickness.Zero, propertyChanged: OnTabsPaddingChanged);

    public Thickness TabsPadding
    {
        get => (Thickness)GetValue(TabsPaddingProperty);
        set => SetValue(TabsPaddingProperty, value);
    }

    public event Action<object, TabBarEventArgs> CurrentPageSelectionChanged;


    public TabBarView()
    {
        App.Current.Resources.TryGetValue("Primary", out object primaryColor);
        App.Current.Resources.TryGetValue("Secondary", out object secondaryColor);

        barColor = primaryColor as Color;
        circleColor = secondaryColor as Color;

        InitializeComponent();

        currentButton = buttonsGrid.First() as ContentButton;
        rootGrid.HeightRequest = TabsHeight + TabsPadding.VerticalThickness;

        backGraphicsView.SizeChanged += TabBarViewSizeChanged;
    }


    private void TabBarViewSizeChanged(object sender, EventArgs e)
    {
        if (initialChange)
        {
            drawable = new TabBarViewDrawable(barColor, circleColor)
            {
                TabsPadding = TabsPadding,
            };
            backGraphicsView.Drawable = drawable;
            backGraphicsView.Invalidate();

            var iconViews = buttonsGrid.Children
                .Cast<ContentButton>()
                .Select(cb => cb.Content)
                .Cast<TabBarIconView>();

            foreach (var iconView in iconViews)
                iconView.TranslationY = defaultIconTranslation;

            initialChange = false;
        }

        SetCircleCenterX(CalculateCircleCenterX(currentButton));
        currentIconView.TranslationY = selectedIconTranslation;
    }

    private void ButtonTapped(object sender, EventArgs e)
    {
        var button = sender as ContentButton;
        var iconView = button.Content as TabBarIconView;

        int difference = Math.Abs(Grid.GetColumn(currentButton) - Grid.GetColumn(button));

        if (difference == 0)
            return;

        uint baseAnimationLength = 400;
        uint animationLength = (uint)(Math.Pow(difference, 1 / 3d) * baseAnimationLength);
        
        var baseAnimation = new Animation();
        var oldIconView = currentIconView;

        var oldIconAnimation = new Animation(v =>
        {
            oldIconView.TranslationY = v;
        }, oldIconView.TranslationY, defaultIconTranslation, easing: Easing.SpringOut);
        var newIconAnimation = new Animation(v =>
        {
            iconView.TranslationY = v;
        }, iconView.TranslationY, selectedIconTranslation, easing: Easing.SpringOut);

        baseAnimation.Add(0, 0.8d, CreateAnimationCircleToX(CalculateCircleCenterX(button)));
        baseAnimation.Add(0, (double)baseAnimationLength / animationLength, oldIconAnimation);
        baseAnimation.Add(1 - (double)baseAnimationLength / animationLength, 1, newIconAnimation);

        baseAnimation.Commit(this, "Animation", length: baseAnimationLength);

        currentButton = button;
        CurrentPageSelectionChanged?.Invoke(this, new TabBarEventArgs(currentIconView.Page));
    }

    private Animation CreateAnimationCircleToX(float newX)
    {
        var circleAnimation = new Animation(v =>
        {
            SetCircleCenterX((float)v);
        }, drawable.CircleCenterX, newX, easing: Easing.SpringOut, () =>
        {
            SetCircleCenterX(newX);
        });
        return circleAnimation;
    }

    private void SetCircleCenterX(float newX)
    {
        drawable.CircleCenterX = newX;
        backGraphicsView.Invalidate();
    }

    private float CalculateCircleCenterX(ContentButton button)
    {
        var tabsWidth = backGraphicsView.Width - TabsPadding.HorizontalThickness;
        var segmentWidth = tabsWidth / buttonsGrid.Children.Count;
        var circleCenterX = (Grid.GetColumn(button) * segmentWidth) + (segmentWidth / 2) + TabsPadding.Left;

        return (float)circleCenterX;
    }

    private static void OnTabsPaddingChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var tabBarView = bindable as TabBarView;

        if (newValue is not Thickness padding)
            return;

        tabBarView.rootGrid.HeightRequest = TabsHeight + padding.VerticalThickness;
        tabBarView.buttonsGrid.Padding = padding;

        if (tabBarView.drawable is null)
            return;

        tabBarView.drawable.TabsPadding = padding;
        tabBarView.backGraphicsView.Invalidate();
    }

    public static float CalculateTabsHeight(float viewHeight, Thickness padding) =>
        (float)(viewHeight - padding.VerticalThickness);

    public static float CalculateInnerRadius(float viewHeight, Thickness padding) =>
        CalculateTabsHeight(viewHeight, padding) / (11f / 4f);

    public static float CalculateOuterRadius(float viewHeight, Thickness padding) =>
        CalculateInnerRadius(viewHeight, padding) + (CalculateTabsHeight(viewHeight, padding) / 12f);
}