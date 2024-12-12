using Avalonia.Controls;
using PerformanceTest.ViewModels;
using Avalonia.LogicalTree;

namespace PerformanceTest.Views;

public partial class ShoppingCartView : UserControl
{
    public ShoppingCartView()
    {
        InitializeComponent();
        DataContext = ShoppingCartViewModel.Get();
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var mainview = this.FindLogicalAncestorOfType<MainView>();
        mainview!.GoToSearch();
    }

    private void Button_Click_1(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var mainview = this.FindLogicalAncestorOfType<MainView>();
        mainview!.GoToCustomer();
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);

        (DataContext as ShoppingCartViewModel)?.Reset();
    }
}
