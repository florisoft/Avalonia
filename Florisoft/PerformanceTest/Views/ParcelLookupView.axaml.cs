using Avalonia.Controls;
using Avalonia.LogicalTree;
using PerformanceTest.ViewModels;

namespace PerformanceTest.Views;

public partial class ParcelLookupView : UserControl
{
    public ParcelLookupView()
    {
        InitializeComponent();
        DataContext = ParcelLookupViewModel.Get();
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var mainview = this.FindLogicalAncestorOfType<MainView>();
        mainview!.GoToCart();
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);

        (DataContext as ParcelLookupViewModel)?.Reset();
    }
}
