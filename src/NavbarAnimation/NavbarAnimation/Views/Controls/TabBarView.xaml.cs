using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NavbarAnimation
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabBarView : ContentView
    {
        #region Private members

        float innerRadius => (float)Height / (11f / 4f);
        float outerRadius => innerRadius + ((float)Height / 12f);
        Point circleCenter;
        float density => (float)Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Density;
        double segment => ((float)Width / stack.Children.Count);
        double defaultCanvasViewTranslation => -(Width / 2) + (segment / 2) - 0.6d;
        double defaultLeftBoxViewTranslation => -leftBoxCanvasView.Width - (leftBoxCanvasView.Width / 2) + (2 * segment);
        double defaultRightBoxViewTranslation => rightBoxCanvasView.Width - (rightBoxCanvasView.Width / 2) - (0.5 * segment);
        SKPath path;
        bool finished = true;
        int currentIndex = 0;
        double svgSize = 20;
        double selectedSvgTranslation => ((innerRadius * 2) - svgSize) / 2;
        double defaultSvgTranslation => ((Height - innerRadius - svgSize) / 2) + innerRadius;

        #endregion

        #region Constructor

        public TabBarView()
        {
            InitializeComponent();

            path = new SKPath();

            var pageEnums = Enum.GetValues(typeof(PageEnum)).Cast<PageEnum>();

            foreach (var page in pageEnums)
            {
                string path = "";

                switch (page)
                {
                    case PageEnum.RibbonPage:
                        path = "RibbonPath";
                        break;
                    case PageEnum.FolderPage:
                        path = "FolderPath";
                        break;
                    case PageEnum.HomePage:
                        path = "HomePath";
                        break;
                    case PageEnum.PinPage:
                        path = "PinPath";
                        break;
                    case PageEnum.ChatPage:
                        path = "ChatPath";
                        break;
                }

                var svg = new TabSvgView
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.Start,
                    Colour = Color.White,
                    Path = App.Current.Resources.GetValue<string>(path),
                    SvgHeight = svgSize,
                    SvgWidth = svgSize,
                    Page = page
                };

                TapGestureRecognizer recognizer = new TapGestureRecognizer();
                recognizer.Tapped += RecognizerTapped;

                svg.GestureRecognizers.Add(recognizer);

                stack.Children.Add(svg);
            }

            clipCanvasView.TranslationX = defaultCanvasViewTranslation;
            stack.Children[0].TranslationY = selectedSvgTranslation;

            SizeChanged += TabBarViewSizeChanged;
        }

        #endregion

        #region Private methods

        private void RecognizerTapped(object sender, EventArgs e)
        {
            var svg = sender as TabSvgView;

            int difference = Math.Abs(currentIndex - stack.Children.IndexOf(svg));

            if (difference == 0)
                return;

            uint baseAnimationTime = 400;
            uint animationTime = (uint)(Math.Pow(difference, 1 / 3d) * baseAnimationTime);

            int oldIndex = currentIndex;
            currentIndex = stack.Children.IndexOf(svg);

            MoveSvgsToDefaultPosition(oldIndex);

            Shell.Current.GoToAsync("///" + svg.Page.ToString());

            Animation baseAnimation = new Animation();

            Animation canvasViewAnimation = new Animation(v =>
            {
                this.clipCanvasView.TranslationX = v;
            }, this.clipCanvasView.TranslationX, (segment * currentIndex) + defaultCanvasViewTranslation, easing: Easing.SpringOut);
            Animation leftBoxViewAnimation = new Animation(v =>
            {
                this.leftBoxCanvasView.TranslationX = v;
            }, this.leftBoxCanvasView.TranslationX, (segment * currentIndex) + defaultLeftBoxViewTranslation, easing: Easing.SpringOut);
            Animation rightBoxViewAnimation = new Animation(v =>
            {
                this.rightBoxCanvasView.TranslationX = v;
            }, this.rightBoxCanvasView.TranslationX, defaultRightBoxViewTranslation + (segment * currentIndex), easing: Easing.SpringOut);
            Animation oldSvgAnimation = new Animation(v =>
            {
                this.stack.Children[oldIndex].TranslationY = v;
            }, this.stack.Children[oldIndex].TranslationY, defaultSvgTranslation, easing: Easing.SpringOut);
            Animation newSvgAnimation = new Animation(v =>
            {
                this.stack.Children[currentIndex].TranslationY = v;
            }, this.stack.Children[currentIndex].TranslationY, selectedSvgTranslation, easing: Easing.SpringOut);

            baseAnimation.Add(0, 0.8d, canvasViewAnimation);
            baseAnimation.Add(0, 0.8d, leftBoxViewAnimation);
            baseAnimation.Add(0, 0.8d, rightBoxViewAnimation);
            baseAnimation.Add(0, (double)baseAnimationTime / animationTime, oldSvgAnimation);
            baseAnimation.Add(1 - (double)baseAnimationTime / animationTime, 1, newSvgAnimation);

            if (finished)
            {
                finished = false;
                baseAnimation.Commit(this, "ClipTranslation", 16, animationTime, finished: (a, b) =>
                {
                    finished = true;
                    this.clipCanvasView.TranslationX = (segment * currentIndex) + defaultCanvasViewTranslation;
                    this.leftBoxCanvasView.TranslationX = (segment * currentIndex) + defaultLeftBoxViewTranslation;
                    this.rightBoxCanvasView.TranslationX = (segment * currentIndex) + defaultRightBoxViewTranslation;
                    MoveSvgsToDefaultPosition(currentIndex);
                });
            }
        }

        private void CreatePath()
        {
            Point p1;
            Point p2;
            Point p3;
            Point p4;
            Point p5;
            Point p6;
            Point p7;
            Point p8;
            Point p9;
            Point p10;
            Point p11;

            #region p6

            p6 = new Point(circleCenter.X, circleCenter.Y + outerRadius);

            #endregion

            #region p4 p8

            //double y46 = (innerRadius * 2) - ((outerRadius - innerRadius) / 2);
            double y46 = p6.Y * (4d / 5d);

            double c = (Math.Pow(circleCenter.X, 2) + Math.Pow(circleCenter.Y, 2) - Math.Pow(outerRadius, 2));

            double d = (Math.Pow(2 * circleCenter.X, 2)) - (4 * (Math.Pow(y46, 2) - (2 * circleCenter.Y * y46) + c));

            double x1 = circleCenter.X - (Math.Sqrt(d) / 2);
            double x2 = circleCenter.X + (Math.Sqrt(d) / 2);

            p4 = new Point(Math.Min(x1, x2), y46);
            p8 = new Point(Math.Max(x1, x2), y46);

            #endregion

            #region p2 p10

            double alpha = (Math.PI / 2) - Math.Atan((circleCenter.X - p4.X) / (p4.Y - circleCenter.Y));

            double l = Math.Tan(alpha) * (p4.Y - circleCenter.Y);

            p2 = new Point(p4.X - l, innerRadius);
            p10 = new Point(p8.X + l, innerRadius);

            #endregion

            #region p1 p11

            p1 = new Point(p2.X - (outerRadius - innerRadius), p2.Y);
            p11 = new Point(p10.X + (outerRadius - innerRadius), p10.Y);

            #endregion

            #region p3 p9

            double scale = (outerRadius - innerRadius) / (Math.Sqrt(Math.Pow(p4.X - p2.X, 2) + Math.Pow(p4.Y - p2.Y, 2)));

            double pX = (p4.X - p2.X) * scale;
            double pY = (p4.Y - p2.Y) * scale;

            p3 = new Point(p2.X + pX, p2.Y + pY);
            p9 = new Point(p10.X - pX, p10.Y + pY);

            #endregion

            #region p5 p7

            double t = (p6.Y - p3.Y) / (p4.Y - p3.Y);

            double x5 = p3.X + (t * (p4.X - p3.X));

            p5 = new Point(x5, p6.Y);
            p7 = new Point(p6.X + (p6.X - x5), p6.Y);

            #endregion

            path = new SKPath();

            path.MoveTo(0, innerRadius * density);
            path.LineTo((float)p1.X * density, (float)p1.Y * density);
            path.QuadTo((float)p2.X * density, (float)p2.Y * density, (float)p3.X * density, (float)p3.Y * density);
            path.LineTo((float)p4.X * density, (float)p4.Y * density);
            path.QuadTo((float)p5.X * density, (float)p5.Y * density, (float)p6.X * density, (float)p6.Y * density);
            path.QuadTo((float)p7.X * density, (float)p7.Y * density, (float)p8.X * density, (float)p8.Y * density);
            path.LineTo((float)p9.X * density, (float)p9.Y * density);
            path.QuadTo((float)p10.X * density, (float)p10.Y * density, (float)p11.X * density, (float)p11.Y * density);
        }

        private void TabBarViewSizeChanged(object sender, EventArgs e)
        {
            clipCanvasView.InvalidateSurface();
            clipCanvasView.TranslationX = (segment * currentIndex) + defaultCanvasViewTranslation;
            leftBoxCanvasView.TranslationX = (segment * currentIndex) + defaultLeftBoxViewTranslation;
            rightBoxCanvasView.TranslationX = (segment * currentIndex) + defaultRightBoxViewTranslation;

            MoveSvgsToDefaultPosition(currentIndex);
        }

        private void MoveSvgsToDefaultPosition(int insteadOf = -1)
        {
            for (int i = 0; i < stack.Children.Count; i++)
            {
                if (i != insteadOf)
                    stack.Children[i].TranslationY = defaultSvgTranslation;
            }
            if (insteadOf != -1 && insteadOf < stack.Children.Count)
                stack.Children[insteadOf].TranslationY = selectedSvgTranslation;
        }

        private void ClipCanvasPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            var info = e.Info;

            canvas.Clear();

            circleCenter = new Point(Width / 2d, innerRadius);

            CreatePath();

            path.LineTo(info.Width, innerRadius * density);
            path.LineTo(info.Width, info.Height);
            path.LineTo(0, info.Height);
            path.Close();

            using (SKPaint circlePaint = new SKPaint())
            {
                circlePaint.Color = Color.FromHex("#fba303").ToSKColor();
                circlePaint.Style = SKPaintStyle.Fill;
                circlePaint.IsAntialias = true;

                using (SKPaint clipRectPaint = new SKPaint())
                {
                    clipRectPaint.Color = Color.FromHex("#fb526b").ToSKColor();
                    clipRectPaint.Style = SKPaintStyle.Fill;
                    clipRectPaint.IsAntialias = true;

                    canvas.DrawCircle((float)circleCenter.X * density, (float)circleCenter.Y * density, innerRadius * density, circlePaint);

                    canvas.DrawPath(path, clipRectPaint);
                }
            }
        }

        private void CanvasPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            var info = e.Info;

            canvas.Clear();

            using (SKPaint paint = new SKPaint())
            {
                paint.Color = Color.FromHex("#fb526b").ToSKColor();
                paint.Style = SKPaintStyle.Fill;
                paint.IsAntialias = true;

                SKPath path = new SKPath();

                path.MoveTo(0, innerRadius * density);
                path.LineTo(info.Width, innerRadius * density);
                path.LineTo(info.Width, info.Height);
                path.LineTo(0, info.Height);
                path.Close();

                canvas.DrawPath(path, paint);
            }
        }

        #endregion
    }
}