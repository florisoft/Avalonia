using Android.App;
using Android.Content.PM;

using Avalonia;
using Avalonia.Android;
using Avalonia.Vulkan;

namespace PerformanceTest.Android;

[Activity(
    Label = "PerformanceTest.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App>
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
                .With(new AndroidPlatformOptions() { RenderingMode = new[] { AndroidRenderingMode.Vulkan } })
                .With(new VulkanOptions()
                {
                    VulkanInstanceCreationOptions = new VulkanInstanceCreationOptions()
                    {
                        UseDebug = false,
                    }
                })
            .WithInterFont();
    }
}
