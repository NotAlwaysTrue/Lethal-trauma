using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;
using System.Text.Json;
using BaroJunk;
namespace Lethaltrauma
{
  public partial class Mod : IAssemblyPlugin
  {
    public IPluginManagementService PluginService { get; set; }
    public static Mod Instance { get; private set; }
    public static Config Config { get; private set; } = new();
    public static Logger Logger { get; private set; } = new()
    {
      PrintFilePath = false
    };
    public static ContentPackage Package { get; private set; }

    public Harmony Harmony { get; } = new Harmony("Lethaltrauma");

    public void Initialize()
    {
      Instance = this;

      PluginService.TryGetPackageForPlugin<Mod>(out ContentPackage package);
      Package = package;

      Config.Settings().CommandName = "lt";
      Config.Settings().DeeplyReactive = true;
      Config.OnUpdated(() => GlobalUpdate());
      Config.OnPropChanged((key, value) => GlobalUpdate());

      Config.UseStrategy(ConfigStrategy.MultiplayerBothSides);

      PatchAll();
      InitializeProjSpecific();
    }

    public void GlobalUpdate()
    {
      BotAim_Patches.GlobalAimUpdate();
      HealthMultipliers_Patches.GlobalUpdateHealthMultipliers();
      PressureKillDelay_Patches.GlobalPressureKillDelay = Mod.Config.PressureKillDelay;
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

    public partial void InitializeProjSpecific();
    public partial void DisposeProjSpecific();

    public void OnLoadCompleted() { }
    public void PreInitPatching() { }
    public void Dispose()
    {
      DisposeProjSpecific();
      Harmony.UnpatchSelf();
      Instance = null;
    }
  }
}