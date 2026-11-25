using System;
using System.Runtime.CompilerServices;

using Barotrauma;
using Barotrauma.Networking;
using Barotrauma.Plugins;
using BaroJunk;
using HarmonyLib;
using Microsoft.Xna.Framework;

namespace Lethaltrauma
{
  public partial class Plugin
  {
    public static readonly IDebugConsole DebugConsole = PluginServiceProvider.GetService<IDebugConsole>();
    public static readonly ISettingsService SettingsService = PluginServiceProvider.GetService<ISettingsService>();
    public static readonly IContentPackageRetrievalService PackageRetrieval = PluginServiceProvider.GetService<IContentPackageRetrievalService>();
    public static readonly ISimpleHookService HookService = PluginServiceProvider.GetService<ISimpleHookService>();

    public static Plugin Instance { get; private set; }
    public static Config Config { get; private set; } = new Config();
    public static Logger Logger { get; private set; } = new Logger
    {
      PrintFilePath = false
    };

    public static ContentPackage Package => PackageRetrieval.ContentPackage;

    public static Harmony Harmony { get; private set; }

    public void Init()
    {
      Instance = this;

      Harmony = PluginServiceProvider.GetService<IHarmonyProvider>().GetHarmony();

      DebugConsole.NewMessage("Plugin loaded", Color.Lime);

      SettingsService.RegisterSetting(ConfigSetting.Instance);

      Config.Settings().CommandName = "lt";
      Config.Settings().DeeplyReactive = true;
      Config.OnUpdated(() => GlobalUpdate());
      Config.OnPropChanged((key, value) => GlobalUpdate());

      Config.UseStrategy(ConfigStrategy.MultiplayerBothSides);

      PatchAll();
      InitProjectSpecific();
    }

    public void OnContentLoaded()
    {
    }

    public void Dispose()
    {
      HooksFacade.RunStopHooks();

      DisposeProjectSpecific();
      DisposeBuildSpecific();
      Harmony?.UnpatchSelf();
      Harmony = null;
      Instance = null;
      DebugConsole.NewMessage("Plugin unloaded", Color.Red);
    }

    public void GlobalUpdate()
    {
      BotAim_Patches.GlobalAimUpdate();
      HealthMultipliers_Patches.GlobalUpdateHealthMultipliers();
      PressureKillDelay_Patches.GlobalPressureKillDelay = Config.PressureKillDelay;
    }

    public void PatchAll()
    {
      BotAim_Patches.Patch(Harmony);
      CrimesInMask_Patches.Patch(Harmony);
      FriendlyFire_Patches.Patch(Harmony);
      HealthMultipliers_Patches.Patch(Harmony);
      PressureKillDelay_Patches.Patch(Harmony);
      StructureDamage_Patches.Patch(Harmony);
      WeaponDamage_Patches.Patch(Harmony);
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
