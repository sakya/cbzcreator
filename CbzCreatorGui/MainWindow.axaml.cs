using System;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia.SingleWindow;
using CbzCreatorGui.Pages;

namespace CbzCreatorGui
{
    public partial class MainWindow : MainWindowBase
    {
        public MainWindow()
        {
            InitializeComponent();

            Title = $"CBZ creator - v{Assembly.GetExecutingAssembly().GetName().Version!.ToString()}";
            WindowTitle = Title;
            Container = ContainerGrid;
        }

        protected override async void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            await Task.Delay(10);
            await NavigateTo(new MainPage());
        }
    }
}