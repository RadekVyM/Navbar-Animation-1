using SimpleToolkit.Core;

namespace NavbarAnimation.Maui.Views.Controls;

public partial class TabBarView : ContentView
{
    bool initialChange = true;
    float innerRadius => (float)backGraphicsView.Height / (11f / 4f);
    float outerRadius => innerRadius + ((float)backGraphicsView.Height / 12f);
    double iconHeight => 20;
    double selectedIconTranslation => ((innerRadius * 2) - iconHeight) / 2;
    double defaultIconTranslation => ((backGraphicsView.Height - innerRadius - iconHeight) / 2) + innerRadius;

    TabBarViewDrawable drawable = null;
    ContentButton currentButton = null;
    TabBarIconView currentIconView = null;
    IList<TabBarIconView> iconViews;
    
    readonly Color barColor;
    readonly Color circleColor;

    public event Action<object, TabBarEventArgs> CurrentPageSelectionChanged;


    public TabBarView()
    {
        App.Current.Resources.TryGetValue("Primary", out object primaryColor);
        App.Current.Resources.TryGetValue("Secondary", out object secondaryColor);

        barColor = primaryColor as Color;
        circleColor = secondaryColor as Color;

        InitializeComponent();

        iconViews = buttonsGrid.Children
            .Cast<ContentButton>()
            .Select(cb => cb.Content)
            .Cast<TabBarIconView>()
            .ToList();
        currentButton = buttonsGrid.First() as ContentButton;
        currentIconView = currentButton.Content as TabBarIconView;

        backGraphicsView.SizeChanged += TabBarViewSizeChanged;
    }


    private void TabBarViewSizeChanged(object sender, EventArgs e)
    {
        if (initialChange)
        {
            drawable = new TabBarViewDrawable(innerRadius, outerRadius, barColor, circleColor);
            backGraphicsView.Drawable = drawable;
            drawable.CircleCenter = new PointF(100, innerRadius);
            backGraphicsView.Invalidate();

            foreach (var iconView in iconViews)
            {
                iconView.TranslationY = defaultIconTranslation;
            }

            initialChange = false;
        }

        SetCircleToX(GetCircleCenterX(currentButton));
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

        baseAnimation.Add(0, 0.8d, GetAnimationCircleToX(GetCircleCenterX(button)));
        baseAnimation.Add(0, (double)baseAnimationLength / animationLength, oldIconAnimation);
        baseAnimation.Add(1 - (double)baseAnimationLength / animationLength, 1, newIconAnimation);

        baseAnimation.Commit(this, "Animation", length: baseAnimationLength);

        currentIconView = iconView;
        currentButton = button;
        CurrentPageSelectionChanged?.Invoke(this, new TabBarEventArgs(currentIconView.Page));
    }

    private Animation GetAnimationCircleToX(float newX)
    {
        var circleAnimation = new Animation(v =>
        {
            drawable.CircleCenter = new PointF((float)v, drawable.CircleCenter.Y);
            backGraphicsView.Invalidate();
        }, drawable.CircleCenter.X, newX, easing: Easing.SpringOut, () =>
        {
            drawable.CircleCenter = new PointF(newX, drawable.CircleCenter.Y);
            backGraphicsView.Invalidate();
        });
        return circleAnimation;
    }

    private void SetCircleToX(float newX)
    {
        drawable.CircleCenter = new PointF(newX, drawable.CircleCenter.Y);
        backGraphicsView.Invalidate();
    }

    private float GetCircleCenterX(ContentButton button)
    {
        var segmentWidth = backGraphicsView.Width / buttonsGrid.Children.Count;
        var circleCenterX = (Grid.GetColumn(button) * segmentWidth) + (segmentWidth / 2);

        return (float)circleCenterX;
    }
}
