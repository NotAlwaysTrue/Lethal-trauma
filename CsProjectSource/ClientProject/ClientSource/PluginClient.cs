using Barotrauma;
using Barotrauma.Plugins;
using BaroJunk;

namespace Lethaltrauma
{
  public partial class Plugin
  {
    public static SettingsServer SettingsServer { get; private set; } = new SettingsServer();

    public partial void InitProjectSpecific()
    {
      Plugin.RegisterCommand(
        "lt_settings",
        "Opens the Lethaltrauma settings page in the Steam overlay.",
        CommandFlags.DoNotRelayToServer,
        args => SettingsServer.OpenSettingsPage(),
        null);

      SettingsServer.Start();
    }

    public partial void DisposeProjectSpecific()
    {
      SettingsServer?.Stop();
      SettingsServer = null;
    }
  }
}
