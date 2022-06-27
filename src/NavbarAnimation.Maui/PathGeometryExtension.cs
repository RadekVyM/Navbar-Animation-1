using Microsoft.Maui.Controls.Shapes;

namespace NavbarAnimation.Maui
{
    public class PathGeometryExtension : IMarkupExtension<Geometry>
    {
        PathGeometryConverter pathGeometryConverter = new PathGeometryConverter();

        public string Path { get; set; }

        public Geometry ProvideValue(IServiceProvider serviceProvider)
        {
            var path = pathGeometryConverter.ConvertFromInvariantString(Path) as Geometry;

            return path;
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return (this as IMarkupExtension<Geometry>).ProvideValue(serviceProvider);
        }
    }
}
