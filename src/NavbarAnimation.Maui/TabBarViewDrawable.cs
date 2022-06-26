
namespace NavbarAnimation.Maui
{
    public class TabBarViewDrawable : IDrawable
{
        private readonly float innerRadius;
        private readonly float outerRadius;
        private readonly Color barFillColor;
        private readonly Color circleFillColor;

        public PointF CircleCenter { get; set; }


        public TabBarViewDrawable(float innerRadius, float outerRadius, Color barFillColor, Color circleFillColor)
        {
            this.innerRadius = innerRadius;
            this.outerRadius = outerRadius;
            this.barFillColor = barFillColor;
            this.circleFillColor = circleFillColor;
        }


        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            var path = CreatePath(dirtyRect);

            canvas.FillColor = barFillColor;
            canvas.FillPath(path);

            canvas.FillColor = circleFillColor;
            canvas.FillCircle(CircleCenter.X, CircleCenter.Y, innerRadius);
        }

        private PathF CreatePath(RectF dirtyRect)
        {
            var path = new PathF();

            PointF p1;
            PointF p2;
            PointF p3;
            PointF p4;
            PointF p5;
            PointF p6;
            PointF p7;
            PointF p8;
            PointF p9;
            PointF p10;
            PointF p11;

            #region p6

            p6 = new PointF(CircleCenter.X, CircleCenter.Y + outerRadius);

            #endregion

            #region p4 p8

            //double y46 = (innerRadius * 2) - ((outerRadius - innerRadius) / 2);
            float y46 = p6.Y * (4f / 5f);

            float c = (float)((Math.Pow(CircleCenter.X, 2) + Math.Pow(CircleCenter.Y, 2) - Math.Pow(outerRadius, 2)));

            float d = (float)((Math.Pow(2 * CircleCenter.X, 2)) - (4 * (Math.Pow(y46, 2) - (2 * CircleCenter.Y * y46) + c)));

            float x1 = CircleCenter.X - (float)(Math.Sqrt(d) / 2);
            float x2 = CircleCenter.X + (float)(Math.Sqrt(d) / 2);

            p4 = new PointF(Math.Min(x1, x2), y46);
            p8 = new PointF(Math.Max(x1, x2), y46);

            #endregion

            #region p2 p10

            float alpha = (float)((Math.PI / 2) - Math.Atan((CircleCenter.X - p4.X) / (p4.Y - CircleCenter.Y)));

            float l = (float)Math.Tan(alpha) * (p4.Y - CircleCenter.Y);

            p2 = new PointF(p4.X - l, innerRadius);
            p10 = new PointF(p8.X + l, innerRadius);

            #endregion

            #region p1 p11

            p1 = new PointF(p2.X - (outerRadius - innerRadius), p2.Y);
            p11 = new PointF(p10.X + (outerRadius - innerRadius), p10.Y);

            #endregion

            #region p3 p9

            float scale = (float)((outerRadius - innerRadius) / Math.Sqrt(Math.Pow(p4.X - p2.X, 2) + Math.Pow(p4.Y - p2.Y, 2)));

            float pX = (p4.X - p2.X) * scale;
            float pY = (p4.Y - p2.Y) * scale;

            p3 = new PointF(p2.X + pX, p2.Y + pY);
            p9 = new PointF(p10.X - pX, p10.Y + pY);

            #endregion

            #region p5 p7

            float t = (p6.Y - p3.Y) / (p4.Y - p3.Y);

            float x5 = p3.X + (t * (p4.X - p3.X));

            p5 = new PointF(x5, p6.Y);
            p7 = new PointF(p6.X + (p6.X - x5), p6.Y);

            #endregion

            path.MoveTo(0, innerRadius);
            path.LineTo(p1.X, p1.Y);
            path.QuadTo(p2.X, p2.Y, p3.X, p3.Y);
            path.LineTo(p4.X, p4.Y);
            path.QuadTo(p5.X, p5.Y, p6.X, p6.Y);
            path.QuadTo(p7.X, p7.Y, p8.X, p8.Y);
            path.LineTo(p9.X, p9.Y);
            path.QuadTo(p10.X, p10.Y, p11.X, p11.Y);
            path.LineTo(dirtyRect.Width, innerRadius);
            path.LineTo(dirtyRect.Width, dirtyRect.Height);
            path.LineTo(0, dirtyRect.Height);
            path.Close();

            return path;
        }
    }
}
