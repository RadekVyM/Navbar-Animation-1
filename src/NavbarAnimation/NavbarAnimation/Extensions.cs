using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace NavbarAnimation
{
    public static class Extensions
    {
        public static T GetValue<T>(this ResourceDictionary dictionary, string key)
        {
            object value;

            dictionary.TryGetValue(key, out value);

            return (T)value;
        }

        public static Color GetColour(this object value)
        {
            if (value == null)
                return Color.Transparent;
            if (value.ToString() == "")
                return Color.Transparent;
            if (value is Color color)
                return color;
            else if (value is DynamicResource resource)
                return App.Current.Resources.GetValue<Color>(resource.Key);
            else if (value is string code)
                return Color.FromHex(code);
            else
                return Color.Transparent;
        }
    }
}
