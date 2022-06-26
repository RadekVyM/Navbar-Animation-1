using Microsoft.Maui.Controls.Shapes;

namespace NavbarAnimation.Maui
{
    public class PathGeometryExtension : IMarkupExtension<Geometry>
    {
        PathGeometryConverter PathGeometryConverter = new PathGeometryConverter();

        public string Path { get; set; }

        public Geometry ProvideValue(IServiceProvider serviceProvider)
        {
            var path = new PathGeometryConverter().ConvertFromInvariantString(Path) as Geometry;

            return path;
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return (this as IMarkupExtension<Geometry>).ProvideValue(serviceProvider);
        }
    }
}
