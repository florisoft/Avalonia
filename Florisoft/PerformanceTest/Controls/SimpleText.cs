using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace PerformanceTest.Controls;

public class SimpleText : Control
{

    /// <summary>
    /// Defines the <see cref="Text"/> property.
    /// </summary>
    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<SimpleText, string?>(nameof(Text), null);

    /// <summary>
    /// Comment
    /// </summary>
    public string? Text
    {
        get { return GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    static CultureInfo _culture = CultureInfo.GetCultureInfo("en-us");

    public TextStyles Style { get; set; } = TextStyles.Description;

    public override void Render(DrawingContext context)
    {
        var text = Text;

        if (text is null)
            return;

        var formattedText = new FormattedText(
            text,
            _culture,
            FlowDirection.LeftToRight,
            new Typeface("Inter"),
            Style switch
            {
                TextStyles.Small => 12,
                _ => 16
            },
            Brushes.Black)
        {
            MaxTextWidth = Width,
            MaxTextHeight = Height,
            MaxLineCount = 1
        };

        formattedText.SetFontWeight(Style switch
        {
            TextStyles.Title => FontWeight.Bold,
            _ => FontWeight.Normal
        });

        context.DrawText(formattedText, new Point(0, 0));
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        return finalSize;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        return new Size(Width, Height);
    }
}

public enum TextStyles
{
    Small,
    Title,
    Description
}
