using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PerformanceTest.ViewModels;

public partial class SelectCustomerViewModel : ViewModelBase
{
    [ObservableProperty]
    List<ItemViewModel>? _items;

    [ObservableProperty]
    ItemViewModel? _selectedItem;

    [ObservableProperty]
    string? _search;

    List<ItemViewModel>? _allItems;
    List<ItemViewModel>? _filteredItems;

    public SelectCustomerViewModel()
    {
        var items = new List<ItemViewModel>();
        for (int i = 0; i < 500; i++)
        {
            items.Add(new ItemViewModel()
            {
                Index = i,
                Description = StringGenerator.GenerateRandomString(14),// "Description " + i,
                Title = StringGenerator.GenerateRandomString(6),//"Title " + i
            });
        }

        items.Sort((a, b) => a.Title.CompareTo(b.Title));

        _allItems = items;
        Items = _allItems;
    }

    static SelectCustomerViewModel? _instance;
    public static SelectCustomerViewModel Get()
    {
        return _instance ??= new SelectCustomerViewModel();
    }

    public void Reset()
    {
        SelectedItem = null;
    }

    partial void OnSearchChanged(string? oldValue, string? newValue)
    {
        if (newValue is null || newValue == "")
        {
            Items = _allItems;
            _filteredItems = null;
        }
        else
        {
            bool forwardSearch = oldValue is not null && (newValue?.StartsWith(oldValue) ?? false);

            var searchList = forwardSearch ? (_filteredItems ?? _allItems) : _allItems;

            _filteredItems = (_filteredItems ?? _allItems)!.Where(x => x.Title.Contains(newValue, StringComparison.OrdinalIgnoreCase)).ToList();
            Items = _filteredItems;
        }
    }

    static class StringGenerator
    {
        private static readonly Random _random = new Random(234545);

        public static string GenerateRandomString(int maxLength)
        {
            if (maxLength < 1)
                throw new ArgumentException("maxLength must be at least 1.", nameof(maxLength));

            char firstChar = (char)_random.Next('A', 'Z' + 1);
            char[] remainingChars = new char[maxLength - 1];

            for (int i = 0; i < remainingChars.Length; i++)
            {
                remainingChars[i] = (char)_random.Next('a', 'z' + 1);
            }

            return firstChar + new string(remainingChars);
        }
    }
}
