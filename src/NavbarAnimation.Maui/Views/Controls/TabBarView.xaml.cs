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

        iconViews = iconsStack.Children.Cast<TabBarIconView>().ToList();
        currentIconView = iconViews.First();

        SizeChanged += TabBarViewSizeChanged;
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

        SetCircleToX(GetCircleCenterX(currentIconView));
        currentIconView.TranslationY = selectedIconTranslation;
    }

    private void ButtonTapped(object sender, EventArgs e)
    {
        var view = sender as BindableObject;
        var iconView = GetIconViewInColumn(Grid.GetColumn(view));

        int difference = Math.Abs(Grid.GetColumn(currentIconView) - Grid.GetColumn(iconView));

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

        baseAnimation.Add(0, 0.8d, GetAnimationCircleToX(GetCircleCenterX(iconView)));
        baseAnimation.Add(0, (double)baseAnimationLength / animationLength, oldIconAnimation);
        baseAnimation.Add(1 - (double)baseAnimationLength / animationLength, 1, newIconAnimation);

        baseAnimation.Commit(this, "Animation", length: baseAnimationLength);

        currentIconView = iconView;
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

    private float GetCircleCenterX(TabBarIconView iconView)
    {
        var segmentWidth = iconsStack.Width / iconsStack.Children.Count;
        var circleCenterX = (Grid.GetColumn(iconView) * segmentWidth) + (segmentWidth / 2);

        return (float)circleCenterX;
    }

    private TabBarIconView GetIconViewInColumn(int column)
    {
        foreach (var iconView in iconViews)
        {
            if (Grid.GetColumn(iconView) == column)
                return iconView;
        }
        return null;
    }
}
