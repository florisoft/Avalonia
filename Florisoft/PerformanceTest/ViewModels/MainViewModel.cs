using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PerformanceTest.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    List<ItemViewModel> _items;

    public MainViewModel()
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

        Items = items;
    }

    static class StringGenerator
    {
        private static readonly Random _random = new Random(8726354);

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
