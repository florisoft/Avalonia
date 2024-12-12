using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using SkiaSharp;

namespace PerformanceTest.Controls;

public class ImmediateText : Control
{
    /// <summary>
    /// Defines the <see cref="Text"/> property.
    /// </summary>
    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<ImmediateText, string?>(nameof(Text), null);

    /// <summary>
    /// Comment
    /// </summary>
    public string? Text
    {
        get { return GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public TextStyles Style { get; set; } = TextStyles.Description;

    protected override Size ArrangeOverride(Size finalSize)
    {
        return finalSize;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        return new Size(Width, Height);
    }

    struct Painter : ICustomDrawOperation
    {
        public Rect Bounds { get; }
        public TextStyles Style { get; set; } = TextStyles.Description;
        public string? Text { get; set; }

        public void Dispose()
        {
        }

        public Painter(Rect bounds, TextStyles style, string? text)
        {
            Bounds = bounds;
            Style = style;
            Text = text;
        }

        public bool Equals(ICustomDrawOperation? other) => false;

        public bool HitTest(Point p) => false;

        static SKPaint _smallText = new SKPaint
        {
            LcdRenderText = false,
            SubpixelText = false,
            Color = SKColors.Black,          // Text color
            TextSize = 12,                   // Font size
            Typeface = SKTypeface.FromFamilyName(
                "Roboto",           // Specify your font family (or null for default)
                SKFontStyleWeight.Normal,  // Bold weight
                SKFontStyleWidth.Normal,
                SKFontStyleSlant.Upright),
            IsAntialias = false,              // Enable anti-aliasing for smoother text
            TextAlign = SKTextAlign.Left     // Text alignment,
        };

        static SKPaint _titleText = new SKPaint
        {
            LcdRenderText = false,
            SubpixelText = false,
            Color = SKColors.Black,          // Text color
            TextSize = 16,                   // Font size
            Typeface = SKTypeface.FromFamilyName(
                "Roboto",           // Specify your font family (or null for default)
                SKFontStyleWeight.Bold,  // Bold weight
                SKFontStyleWidth.Normal,
                SKFontStyleSlant.Upright),
            IsAntialias = false,              // Enable anti-aliasing for smoother text
            TextAlign = SKTextAlign.Left,            
        };

        static SKPaint _descriptionText = new SKPaint
        {
            LcdRenderText = false,
            SubpixelText = false,
            Color = SKColors.Black,          // Text color
            TextSize = 16,                   // Font size
            Typeface = SKTypeface.FromFamilyName(
                "Roboto",           // Specify your font family (or null for default)
                SKFontStyleWeight.Normal,  // Bold weight
                SKFontStyleWidth.Normal,
                SKFontStyleSlant.Upright),
            IsAntialias = false,              // Enable anti-aliasing for smoother text
            TextAlign = SKTextAlign.Left     // Text alignment,
        };

        public void Render(ImmediateDrawingContext context)
        {
            var leaseFeature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();

            using var lease = leaseFeature!.Lease();
            var canvas = lease.SkCanvas;

            canvas.DrawText(Text ?? "", 0f, Style switch
            {
                TextStyles.Small => 10f,
                TextStyles.Title => 16f,
                _ => 16f
            }, Style switch
            {
                TextStyles.Small => _smallText,
                TextStyles.Title => _titleText,
                _ => _descriptionText
            });
        }
    }

    public override void Render(DrawingContext context)
    {
        context.Custom(new Painter(new Rect(0, 0, Bounds.Width, Bounds.Height), Style, Text));
    }
}
