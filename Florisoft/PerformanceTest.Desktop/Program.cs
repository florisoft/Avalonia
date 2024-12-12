﻿using System;

using Avalonia;
using Avalonia.Vulkan;

namespace PerformanceTest.Desktop;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
                .With(new Win32PlatformOptions() { RenderingMode = new[] { Win32RenderingMode.Vulkan } })
                .With(new VulkanOptions()
                {
                    VulkanInstanceCreationOptions = new VulkanInstanceCreationOptions()
                    {
                        UseDebug = false,
                    }
                })
            .WithInterFont()
            .LogToTrace();

}
