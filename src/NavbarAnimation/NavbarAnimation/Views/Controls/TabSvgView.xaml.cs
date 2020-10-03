using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NavbarAnimation
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabSvgView : ContentView
    {
        #region Public members

        public PageEnum Page { get; set; }

        public object Colour
        {
            get => GetValue(ColourProperty);
            set => SetValue(ColourProperty, value);
        }

        public string Path
        {
            get => (string)GetValue(PathProperty);
            set => SetValue(PathProperty, value);
        }

        public double SvgWidth
        {
            get => (double)GetValue(SvgWidthProperty);
            set => SetValue(SvgWidthProperty, value);
        }

        public double SvgHeight
        {
            get => (double)GetValue(SvgHeightProperty);
            set => SetValue(SvgHeightProperty, value);
        }

        public static readonly BindableProperty ColourProperty =
            BindableProperty.Create(nameof(Colour), typeof(object), typeof(TabSvgView), Color.Black, BindingMode.OneWay, propertyChanged: MyPropertyChanged);

        public static readonly BindableProperty PathProperty =
            BindableProperty.Create(nameof(Path), typeof(string), typeof(TabSvgView), "", BindingMode.OneWay, propertyChanged: MyPropertyChanged);

        public static readonly BindableProperty SvgWidthProperty =
            BindableProperty.Create(nameof(SvgWidth), typeof(double), typeof(TabSvgView), 20d, BindingMode.OneWay, propertyChanged: MyPropertyChanged);

        public static readonly BindableProperty SvgHeightProperty =
            BindableProperty.Create(nameof(SvgHeight), typeof(double), typeof(TabSvgView), 20d, BindingMode.OneWay, propertyChanged: MyPropertyChanged);

        private static void MyPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            TabSvgView svgView = bindable as TabSvgView;

            try
            {
                svgView.canvasView.InvalidateSurface();
            }
            catch { };
        }

        #endregion


        public TabSvgView()
        {
            InitializeComponent();
        }


        private void CanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            var info = e.Info;
            canvas.Clear();

            if (string.IsNullOrWhiteSpace(Path))
                return;

            SKPath path = SKPath.ParseSvgPathData(Path);

            using (SKPaint paint = new SKPaint())
            {
                paint.Style = SKPaintStyle.Stroke;
                paint.Color = Colour.GetColour().ToSKColor();
                paint.StrokeCap = SKStrokeCap.Round;
                paint.StrokeJoin = SKStrokeJoin.Round;
                paint.IsAntialias = true;
                paint.StrokeWidth = 0.3f;

                path.GetBounds(out SKRect bounds);

                canvas.Translate(info.Width / 2, info.Height / 2);
                canvas.Scale(Math.Min((float)(info.Width / (bounds.Width + (bounds.Width * 0.2d))), (float)(info.Height / (bounds.Height + (bounds.Height * 0.2d)))));
                canvas.Translate(-bounds.MidX, -bounds.MidY);

                canvas.DrawPath(path, paint);
            };
        }
    }
}