using Avalonia.Controls;
using PerformanceTest.ViewModels;
using Avalonia.LogicalTree;

namespace PerformanceTest.Views;

public partial class SelectCustomerView : UserControl
{
    public SelectCustomerView()
    {
        InitializeComponent();
        DataContext = SelectCustomerViewModel.Get();
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var mainview = this.FindLogicalAncestorOfType<MainView>();
        mainview!.GoToCart();
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);

        (DataContext as SelectCustomerViewModel)?.Reset();
    }
}
