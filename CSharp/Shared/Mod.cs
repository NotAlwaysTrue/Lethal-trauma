using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;

namespace Lethaltrauma
{
  public partial class Mod : IAssemblyPlugin
  {
    public static Mod Instance { get; private set; }
    public static Config Config { get; private set; } = new Config();
    public void Initialize()
    {
      Instance = this;
    }


    public void OnLoadCompleted() { }
    public void PreInitPatching() { }
    public void Dispose()
    {
      Instance = null;
    }
  }
}