using SimpleToolkit.Helpers;
using SimpleToolkit.SimpleButton;

namespace NavbarAnimation.Maui.Views.Controls;

public partial class TabBarView : VerticalStackLayout
{
    public const double TabsHeight = 80;
    public const double IconHeight = 20;

    readonly Color barColor;
    readonly Color circleColor;

    double selectedIconTranslation =>
        ((CalculateInnerRadius((float)Height, tabsPadding) * 2) - IconHeight) / 2;
    double defaultIconTranslation =>
        ((CalculateTabsHeight((float)Height, tabsPadding) -
        CalculateInnerRadius((float)Height, tabsPadding) - IconHeight) / 2) +
        CalculateInnerRadius((float)Height, tabsPadding);

    TabBarViewDrawable drawable = null;
    SimpleButton currentButton = null;
    TabBarIconView currentIconView => currentButton.Content as TabBarIconView;

    private Thickness tabsPadding { get; set; }

    public event Action<object, TabBarEventArgs> CurrentPageSelectionChanged;


    public TabBarView()
    {
        App.Current.Resources.TryGetValue("Primary", out object primaryColor);
        App.Current.Resources.TryGetValue("Secondary", out object secondaryColor);

        barColor = primaryColor as Color;
        circleColor = secondaryColor as Color;

        InitializeComponent();

        currentButton = buttonsGrid.First() as SimpleButton;

        SizeChanged += OnTabBarSizeChanged;
    }


    private void OnTabBarSizeChanged(object sender, EventArgs e)
    {
        var insets = WindowInsetsProvider.GetInsets();
        tabsPadding = insets with { Top = 0 };

        backGraphicsView.HeightRequest = buttonsGrid.HeightRequest = TabsHeight + tabsPadding.Bottom;
        buttonsGrid.Padding = tabsPadding;

        backGraphicsView.Drawable ??= drawable = new TabBarViewDrawable(barColor, circleColor);
        drawable.TabsPadding = tabsPadding;
        backGraphicsView.Invalidate();

        var iconViews = buttonsGrid.Children
            .Cast<SimpleButton>()
            .Select(cb => cb.Content)
            .Cast<TabBarIconView>();

        foreach (var iconView in iconViews)
            iconView.TranslationY = defaultIconTranslation;

        SetCircleCenterX(CalculateCircleCenterX(currentButton));
        currentIconView.TranslationY = selectedIconTranslation;
    }

    private void ButtonTapped(object sender, EventArgs e)
    {
        var button = sender as SimpleButton;
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

    private float CalculateCircleCenterX(SimpleButton button)
    {
        var tabsWidth = Width - tabsPadding.HorizontalThickness;
        var segmentWidth = tabsWidth / buttonsGrid.Children.Count;
        var circleCenterX = (Grid.GetColumn(button) * segmentWidth) + (segmentWidth / 2) + tabsPadding.Left;

        return (float)circleCenterX;
    }

    public static float CalculateTabsHeight(float viewHeight, Thickness padding) =>
        (float)(viewHeight - padding.VerticalThickness);

    public static float CalculateInnerRadius(float viewHeight, Thickness padding) =>
        CalculateTabsHeight(viewHeight, padding) / (11f / 4f);

    public static float CalculateOuterRadius(float viewHeight, Thickness padding) =>
        CalculateInnerRadius(viewHeight, padding) + (CalculateTabsHeight(viewHeight, padding) / 12f);
}