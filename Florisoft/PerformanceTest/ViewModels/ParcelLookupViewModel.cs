using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PerformanceTest.ViewModels;

public partial class ParcelLookupViewModel : ViewModelBase
{
    [ObservableProperty]
    List<ItemViewModel> _items;

    [ObservableProperty]
    ItemViewModel? _selectedItem;

    [ObservableProperty]
    int _selectedStockIndex = 0;

    List<ItemViewModel> _itemsA;
    List<ItemViewModel> _itemsB;
    List<ItemViewModel> _itemsC;

    public ParcelLookupViewModel()
    {
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
            _itemsA = items;
        }

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
            _itemsB = items;
        }

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
            _itemsC = items;
        }

        SelectStockByIndex(SelectedStockIndex);
    }

    void SelectStockByIndex(int index)
    {
        switch (index)
        {
            case 0: Items = _itemsA; break;
            case 1:
                Items = _itemsB;
                break;
            case 2:
                Items = _itemsC;
                break;
        }
    }

    public void Reset()
    {
        SelectedItem = null;
    }

    partial void OnSelectedStockIndexChanged(int value)
    {
        SelectStockByIndex(value);
    }

    static ParcelLookupViewModel? _instance;
    public static ParcelLookupViewModel Get()
    {
        return _instance ??= new ParcelLookupViewModel();
    }

    static class StringGenerator
    {
        private static readonly Random _random = new Random(762534);

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
