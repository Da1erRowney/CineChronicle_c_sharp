using Android.App;
using Android.Content;
using Android.Content.PM;
using Microsoft.Maui.Authentication;

namespace CineChronicle.Platforms.Android;

[Activity(
    NoHistory = true,
    LaunchMode = LaunchMode.SingleTop,
    Exported = true)] // Явно указываем exported
[IntentFilter(
    new[] { Intent.ActionView },
    Categories = new[] {
        Intent.CategoryDefault,
        Intent.CategoryBrowsable
    },
    DataScheme = "com.googleusercontent.apps.28327101441-8lg1g6eb10bqubimnnv4gmeddcbuvhsc",
    DataPath = "/oauth2redirect"
)]
public class GoogleAuthCallbackActivity : WebAuthenticatorCallbackActivity
{
}