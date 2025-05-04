using SkiaSharp.Extended.UI.Controls;
using CommunityToolkit.Maui;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace TestProject;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseSkiaSharp()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
			{
                fonts.AddEmbeddedResourceFont(typeof(App).Assembly, "Resources/Images");
                fonts.AddFont("Roboto-Medium.ttf", "Ofont");
                fonts.AddFont("fontello.ttf", "Icons");
            });
        return builder.Build();
    }
}
