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

    public void Initialize()
    {
      Instance = this;

      PluginService.TryGetPackageForPlugin<Mod>(out ContentPackage package);
      Package = package;

      Config.OnUpdated(() => Logger.Log(JsonSerializer.Serialize(Mod.Config)));

      InitializeProjSpecific();
    }

    public partial void InitializeProjSpecific();
    public partial void DisposeProjSpecific();

    public void OnLoadCompleted() { }
    public void PreInitPatching() { }
    public void Dispose()
    {
      DisposeProjSpecific();
      Instance = null;
    }
  }
}