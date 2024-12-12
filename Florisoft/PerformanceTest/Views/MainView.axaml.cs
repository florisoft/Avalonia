using Avalonia.Controls;

namespace PerformanceTest.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        GoToCustomer();
    }

    static ParcelLookupView? s_parcelLookupView;
    static ShoppingCartView? s_shoppingCartView;
    static SelectCustomerView? s_selectCustomerView;

    public void GoToSearch()
    {
        title.Text = "Stock item lookup";
        page_container.Content = s_parcelLookupView ??= new ParcelLookupView();
    }

    public void GoToCart()
    {
        title.Text = "Shopping cart";
        page_container.Content = s_shoppingCartView ??= new ShoppingCartView();
    }

    public void GoToCustomer()
    {
        title.Text = "Select order";
        page_container.Content = s_selectCustomerView ??= new SelectCustomerView();
    }
}
