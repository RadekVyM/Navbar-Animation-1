using NavbarAnimation.Maui.Views.Pages;
using SimpleToolkit.SimpleShell;

namespace NavbarAnimation.Maui;

public partial class AppShell : SimpleShell
{
    public AppShell()
    {
        InitializeComponent();

        AddTab(typeof(HomePage), PageType.HomePage);
        AddTab(typeof(ChatPage), PageType.ChatPage);
        AddTab(typeof(RibbonPage), PageType.RibbonPage);
        AddTab(typeof(PinPage), PageType.PinPage);
        AddTab(typeof(FolderPage), PageType.FolderPage);
    }


    private void AddTab(Type page, PageType pageEnum)
    {
        var tab = new Tab { Route = pageEnum.ToString(), Title = pageEnum.ToString() };
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