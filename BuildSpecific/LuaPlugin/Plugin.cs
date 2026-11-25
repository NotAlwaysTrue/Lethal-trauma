using System;
using Barotrauma;
using Microsoft.Xna.Framework;
using Barotrauma.LuaCs;
using System.Linq;

namespace Lethaltrauma
{
  public partial class Plugin : IAssemblyPlugin
  {
    public void Initialize() => Init();
    public void OnLoadCompleted() { }
    public void PreInitPatching() { }

    public void DisposeBuildSpecific()
    {

    }
  }
}