using System;
using Xamarin.Forms;

namespace NavbarAnimation
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            AddTab(typeof(RibbonPage), PageEnum.RibbonPage);
            AddTab(typeof(FolderPage), PageEnum.FolderPage);
            AddTab(typeof(HomePage), PageEnum.HomePage);
            AddTab(typeof(PinPage), PageEnum.PinPage);
            AddTab(typeof(ChatPage), PageEnum.ChatPage);
        }

        private void AddTab(Type page, PageEnum pageEnum)
        {
            Tab tab = new Tab { Route = pageEnum.ToString(), Title = pageEnum.ToString() };
            tab.Items.Add(new ShellContent { ContentTemplate = new DataTemplate(page) });

            tabBar.Items.Add(tab);
        }
    }

    public enum PageEnum
    {
        RibbonPage, FolderPage, HomePage, PinPage, ChatPage
    }
}
