using Microsoft.Maui;

using System.Diagnostics.Metrics;
using System.Reflection.Metadata;

namespace TestProject;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
               // .UseMauiCommunityToolkitMediaElement()
            .ConfigureFonts(fonts =>
			{
				//fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				//fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Roboto-Medium.ttf", "Ofont");
                fonts.AddFont("fontello.ttf", "Icons");
            });
        return builder.Build();
    }
}
