using System;
using System.Runtime.CompilerServices;

using Barotrauma;
using Barotrauma.Networking;
using Barotrauma.Plugins;
using HarmonyLib;
using Microsoft.Xna.Framework;

[assembly: IgnoresAccessChecksTo("Barotrauma")]
[assembly: IgnoresAccessChecksTo("BarotraumaCore")]
[assembly: IgnoresAccessChecksTo("DedicatedServer")]

namespace Lethaltrauma
{
  public partial class Plugin : IBarotraumaPlugin
  {
    public static readonly IDebugConsole DebugConsole = PluginServiceProvider.GetService<IDebugConsole>();
    public static readonly ISettingsService SettingsService = PluginServiceProvider.GetService<ISettingsService>();
    public static readonly IContentPackageRetrievalService PackageRetrieval = PluginServiceProvider.GetService<IContentPackageRetrievalService>();
    public static readonly ISimpleHookService HookService = PluginServiceProvider.GetService<ISimpleHookService>();

    public static Plugin Instance { get; private set; }
    public static Logger Logger { get; private set; } = new Logger
    {
      PrintFilePath = false
    };

    private PluginOnPreUpdateDelegate onPreUpdate;
    private object[] settingsSnapshot;
    private float settingsCheckTimer;

    private const float SettingsCheckInterval = 0.25f;

    public static ContentPackage Package => PackageRetrieval.ContentPackage;

    public static Harmony Harmony { get; private set; }

    public void Init()
    {
      Instance = this;

      Harmony = PluginServiceProvider.GetService<IHarmonyProvider>().GetHarmony();

      DebugConsole.NewMessage("Plugin loaded", Color.Lime);

      Settings.RegisterAll(SettingsService);

      settingsSnapshot = Settings.Snapshot();
      GlobalUpdate();

      onPreUpdate = OnPreUpdate;
      HookService.RegisterHook(onPreUpdate);

      SettingsCommand.Register();

      PatchAll();
      InitProjectSpecific();
    }

    public void OnContentLoaded()
    {
    }

    public void Dispose()
    {
      SettingsCommand.Deregister();

      if (onPreUpdate is not null) HookService.DeregisterHook(onPreUpdate);
      onPreUpdate = null;

      DisposeProjectSpecific();
      Harmony?.UnpatchSelf();
      Harmony = null;
      Instance = null;
      DebugConsole.NewMessage("Plugin unloaded", Color.Red);
    }

    private void OnPreUpdate(float deltaTime)
    {
      settingsCheckTimer -= deltaTime;
      if (settingsCheckTimer > 0f) return;
      settingsCheckTimer = SettingsCheckInterval;

      if (!Settings.HasChanged(settingsSnapshot)) return;

      ApplySettings();
    }

    public static void ApplySettings()
    {
      if (Instance is null) return;

      Instance.settingsSnapshot = Settings.Snapshot();
      GlobalUpdate();
    }

    public static void GlobalUpdate()
    {
      BotAim_Patches.GlobalAimUpdate();
      HealthMultipliers_Patches.GlobalUpdateHealthMultipliers();
      PressureKillDelay_Patches.GlobalPressureKillDelay = Settings.PressureKillDelay.Value;
    }

    public void PatchAll()
    {
      Harmony.PatchAll(typeof(Plugin).Assembly);
    }

    public static bool HasConfigPermission(Client client)
    {
      if (client is null) return true;
      if (GameMain.IsSingleplayer) return true;

#if SERVER
      return client.Connection == GameMain.Server?.OwnerConnection ||
             client.HasPermission(ClientPermissions.ConsoleCommands);
#else
      return client.HasPermission(ClientPermissions.ConsoleCommands);
#endif
    }

    public static bool LocalClientHasConfigPermission()
    {
#if CLIENT
      return GameMain.IsSingleplayer ||
             GameMain.Client is null ||
             GameMain.Client.IsServerOwner ||
             GameMain.Client.HasPermission(ClientPermissions.ConsoleCommands);
#else
      return true;
#endif
    }

    public partial void InitProjectSpecific();
    public partial void DisposeProjectSpecific();

    public static void RegisterCommand(string name, string help, CommandFlags flags, Action<string[]> onExecute, Func<string[][]> hints = null)
    {
#if CLIENT
      DebugConsole.RegisterCommand(name, help, flags, args => onExecute(args), hints);
#else
      DebugConsole.RegisterCommand(name, help, flags, args => onExecute(args), null, hints);
#endif
    }

    public static void DeregisterCommand(string name) => DebugConsole.DeregisterCommand(name);
  }
}

namespace System.Runtime.CompilerServices
{
  [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
  internal sealed class IgnoresAccessChecksToAttribute : Attribute
  {
    public IgnoresAccessChecksToAttribute(string assemblyName)
    {
      AssemblyName = assemblyName;
    }

    public string AssemblyName { get; }
  }
}
