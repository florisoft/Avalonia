using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Interactivity;

namespace PerformanceTest.Controls;

public class ListBox : Avalonia.Controls.ListBox
{
    protected override Type StyleKeyOverride => typeof(Avalonia.Controls.ListBox);

    const string PART_EMPTY_CONTENT = "PART_EmptyContent";

    internal bool IsRecycling = true;
    IDisposable? _transitionDisabler;

    bool _isVirtualized = true;

    ContentPresenter? _emptyContentPresenter;
    ScrollViewer? _scrollViewer;

    /// <summary>
    /// Defines the <see cref="EmptyContent"/> property.
    /// </summary>
    public static readonly StyledProperty<object?> EmptyContentProperty =
        AvaloniaProperty.Register<ListBox, object?>(nameof(EmptyContent), null);

    public static readonly StyledProperty<bool> IsHighlightedProperty;
    public static readonly RoutedEvent<RoutedEventArgs> IsHighlightedChangedEvent;

    /// <summary>
    /// Defines the <see cref="EmptyContentTemplate"/> property.
    /// </summary>
    public static readonly StyledProperty<IDataTemplate?> EmptyContentTemplateProperty =
        AvaloniaProperty.Register<ListBox, IDataTemplate?>(nameof(EmptyContentTemplate), null);

    public static readonly DirectProperty<ListBox, int> HighlightedIndexProperty;

    /// <summary>
    /// Content of the list when SouceItems is empty.
    /// </summary>
    public object? EmptyContent
    {
        get { return GetValue(EmptyContentProperty); }
        set { SetValue(EmptyContentProperty, value); }
    }

    /// <summary>
    /// Template of the EmptyContent.
    /// </summary>
    public IDataTemplate? EmptyContentTemplate
    {
        get { return GetValue(EmptyContentTemplateProperty); }
        set { SetValue(EmptyContentTemplateProperty, value); }
    }

    /// <summary>
    /// Only set once in XAML, cannot change after initialization. Default IsVirtualized is true, using our own custom VirtualizingStackPanel.
    /// </summary>
    public bool IsVirtualized
    {
        get => _isVirtualized;
        set => _isVirtualized = value;
    }

    static ListBox()
    {
        //////HighlightedIndexProperty = AvaloniaProperty.RegisterDirect("HighlightedIndexProperty", (ListBox o) => o.HighlightedIndex, delegate (ListBox o, int v)
        //////{
        //////    o.HighlightedIndex = v;
        //////}, -1, BindingMode.TwoWay);

        //////IsHighlightedProperty = AvaloniaProperty.RegisterAttached<SelectingItemsControl, Control, bool>("IsHighlighted", defaultValue: false, inherits: false, BindingMode.TwoWay);
        //////IsHighlightedChangedEvent = RoutedEvent.Register<SelectingItemsControl, RoutedEventArgs>("IsHighlightedChanged", RoutingStrategies.Bubble);
    }

    int _highlightedIndex = -1;
    public int HighlightedIndex
    {
        get
        {
            return _highlightedIndex;
        }
        set
        {
            int old = _highlightedIndex;
            _highlightedIndex = value;
            RaisePropertyChanged(HighlightedIndexProperty, old, _highlightedIndex);
            MarkContainerHighlighted(old, false, true);
            MarkContainerHighlighted(_highlightedIndex, true, true);
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (!IsVirtualized)
        {
            ItemsPanel = new FuncTemplate<Panel?>(() => new StackPanel());
        }
        else
        {
            ItemsPanel = new FuncTemplate<Panel?>(() => new VirtualizingStackPanel());
        }

        _emptyContentPresenter = e.NameScope.Find(PART_EMPTY_CONTENT) as ContentPresenter;
        _scrollViewer = e.NameScope.Find<ScrollViewer>("PART_ScrollViewer");
        CheckEmpty();

        if (HighlightedIndex >= 0)
        {
            MarkContainerHighlighted(HighlightedIndex, true, false);
        }
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new ListBoxItem();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        if (!IsRecycling)
        {
            return base.NeedsContainerOverride(item, index, out recycleKey);
        }

        if (item is Control)
        {
            recycleKey = null;
            return false;
        }
        else
        {
            recycleKey = item?.GetType();
            return true;
        }
    }

    protected override void ContainerForItemPreparedOverride(Control container, object? item, int index)
    {
        if (IsRecycling)
        {
            container.InvalidateAllMeasures(Size.Infinity);
            base.ContainerForItemPreparedOverride(container, item, index);
            _transitionDisabler?.Dispose();
        }
        else
        {
            base.ContainerForItemPreparedOverride(container, item, index);
        }

        //container.SetValue(IsHighlightedProperty, HighlightedIndex == index);

    }

    bool _ignoreContainerHighlightedChanged = false;
    private void MarkContainerHighlighted(Control container, bool highlighted)
    {
        _ignoreContainerHighlightedChanged = true;
        try
        {
            container.SetCurrentValue(IsHighlightedProperty, highlighted);
        }
        finally
        {
            _ignoreContainerHighlightedChanged = false;
        }
    }

    void MarkContainerHighlighted(int index, bool selected, bool scrollIntoView)
    {
        if (scrollIntoView)
        {
            this.ScrollItemToTop(index);
        }

        Control? control = ContainerFromIndex(index);
        if (control != null)
        {
            MarkContainerHighlighted(control, selected);

        }
    }

    protected override void ContainerIndexChangedOverride(Control container, int oldIndex, int newIndex)
    {
        container.SetValue(IsHighlightedProperty, HighlightedIndex == newIndex);
        base.ContainerIndexChangedOverride(container, oldIndex, newIndex);
    }

    System.Reflection.FieldInfo? _ignoreContainerSelectionChangedField;

    // De element.ClearValue(IsSelectedProperty); geeft problemen, zie 'https://github.com/florisoft/frontend.apps/issues/1025'
    // Dit in comment gezet, en kon in gallery app en cashcarry geen ongewenste gevolgen ontdekken.
    protected override void ClearContainerForItemOverride(Control element)
    {
        if (!IsRecycling)
        {
            base.ClearContainerForItemOverride(element);
            return;
        }

        _ignoreContainerSelectionChangedField ??= typeof(SelectingItemsControl).GetField("_ignoreContainerSelectionChanged", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        _ignoreContainerSelectionChangedField!.SetValue(this, true);
        element.ClearValue(IsSelectedProperty);
        _ignoreContainerSelectionChangedField!.SetValue(this, false);
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        if (!IsRecycling)
        {
            base.PrepareContainerForItemOverride(container, item, index);
            return;
        }

        // TODO: doordat we hier niet de base aanroepen mist de GetOrCreateSelectionModel();
        // TEST: als je item als container geeft, lijkt de ItemsControl zijn eigen werk over te slaan
        base.PrepareContainerForItemOverride(container, container, index);

        // TODO: te optimaliseren? de container instanties zijn 'recycled', per instantie maar 1 keer kijken
        // of er iets te disablen is, en zo niet dan daarna niet meer gaan zoeken?
        _transitionDisabler = container.DisableTransitions();

        var lbc = (ContentControl)container;

        if (lbc.Content is not null)
        {
            return;
        }

        IDataTemplate? itemtemplate = null;

        if (lbc.ContentTemplate is null)
        {
            itemtemplate = ItemTemplate;
            foreach (var dt in DataTemplates)
            {
                if (dt.Match(item))
                {
                    itemtemplate = dt;
                    break;
                }
            }
        }

        var result = itemtemplate?.Build(item);
        lbc.Content = result;
    }

    void CheckEmpty()
    {
        if (_emptyContentPresenter is null)
            return;

        _emptyContentPresenter.IsVisible = !(Items.Count > 0 || (ItemsSource?.GetEnumerator().MoveNext() ?? false));
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ItemsSourceProperty)
        {
            CheckEmpty();
        }
    }

    public void ScrollSelectedItemToTop()
    {
        ScrollItemToTop(SelectedIndex);
    }

    public void ScrollSelectedItemIntoView()
    {
        if (SelectedIndex < 0)
            return;
        this.ScrollIntoView(SelectedIndex);
    }

    void ScrollItemToTop(int index)
    {
        if (_scrollViewer is null)
            return;

        if (index >= 0)
        {
            _scrollViewer?.ScrollToEnd();
            ScrollIntoView(index);
        }
    }
}
