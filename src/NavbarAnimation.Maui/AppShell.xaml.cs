using NavbarAnimation.Maui.Views.Pages;
using SimpleToolkit.Core;

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

            Loaded += AppShellLoaded;
        }

        private static void AppShellLoaded(object sender, EventArgs e)
        {
            var shell = sender as AppShell;

            shell.Window.SubscribeToSafeAreaChanges(safeArea =>
            {
                shell.pageContainer.Margin = safeArea;
                shell.tabBarView.Margin = safeArea;
                shell.bottomBackgroundRectangle.IsVisible = safeArea.Bottom > 0;
                shell.bottomBackgroundRectangle.HeightRequest = safeArea.Bottom;
            });
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