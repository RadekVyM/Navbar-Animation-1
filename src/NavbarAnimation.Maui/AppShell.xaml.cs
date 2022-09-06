using NavbarAnimation.Maui.Views.Pages;

namespace NavbarAnimation.Maui
{
    public partial class AppShell : SimpleToolkit.SimpleShell.SimpleShell
    {
        public AppShell()
        {
            InitializeComponent();

            AddTab(typeof(RibbonPage), PageType.RibbonPage);
            AddTab(typeof(FolderPage), PageType.FolderPage);
            AddTab(typeof(HomePage), PageType.HomePage);
            AddTab(typeof(PinPage), PageType.PinPage);
            AddTab(typeof(ChatPage), PageType.ChatPage);

            pageContainer.SizeChanged += PageContainerSizeChanged;
        }

        private void PageContainerSizeChanged(object sender, EventArgs e)
        {
            var insets = this.Window.GetSafeAreaInsets();

            pageContainer.Margin = insets;
            tabBarView.Margin = insets;
            bottomBackgroundRectangle.IsVisible = insets.Bottom > 0;
            bottomBackgroundRectangle.HeightRequest = insets.Bottom;
        }

        private void AddTab(Type page, PageType pageEnum)
        {
            Tab tab = new Tab { Route = pageEnum.ToString(), Title = pageEnum.ToString() };
            tab.Items.Add(new ShellContent { ContentTemplate = new DataTemplate(page) });

            tabBar.Items.Add(tab);
        }

        private void TabBarViewCurrentPageChanged(object sender, TabBarEventArgs e)
        {
            Shell.Current.GoToAsync("///" + e.CurrentPage.ToString());
        }
    }

    public enum PageType
    {
        RibbonPage, FolderPage, HomePage, PinPage, ChatPage
    }
}