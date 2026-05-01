using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;
using BaroJunk;

namespace Lethaltrauma
{
  public partial class Mod : IAssemblyPlugin
  {
    public static SettingsServer SettingsServer { get; private set; } = new();

    public partial void InitializeProjSpecific()
    {
      PluginCommands.Add("lt_settings", (args) => SettingsServer.OpenSettingsPage());

      SettingsServer.OpenSettingsPage();
    }

    public partial void DisposeProjSpecific()
    {
      SettingsServer.Stop();
      SettingsServer = null;
    }
  }
}