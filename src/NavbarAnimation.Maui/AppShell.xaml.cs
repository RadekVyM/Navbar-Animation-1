using NavbarAnimation.Maui.Views.Pages;

namespace NavbarAnimation.Maui
{
    public partial class AppShell : OverlayShell.OverlayShell
    {
        public AppShell()
        {
            InitializeComponent();

            AddTab(typeof(RibbonPage), PageType.RibbonPage);
            AddTab(typeof(FolderPage), PageType.FolderPage);
            AddTab(typeof(HomePage), PageType.HomePage);
            AddTab(typeof(PinPage), PageType.PinPage);
            AddTab(typeof(ChatPage), PageType.ChatPage);
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