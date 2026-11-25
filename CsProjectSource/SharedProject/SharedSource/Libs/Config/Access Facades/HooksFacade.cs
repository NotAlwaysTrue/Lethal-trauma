using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Barotrauma;
using Barotrauma.Plugins;
using Lethaltrauma;

namespace BaroJunk
{
  public delegate object ConfigHookFunc(object[] args);

  public interface IHooksFacade
  {
    public void CallHook(string name, params object[] args);
    public void CallPatch(MethodBase method);
    public void AddHook(string name, string identifier, ConfigHookFunc func);
  }

  public class HooksFacade : IHooksFacade
  {
    private const string RoundEndHook = "roundend";
    private const string StopHook = "stop";

    private static readonly Dictionary<string, ConfigHookFunc> roundEndHooks = new Dictionary<string, ConfigHookFunc>();
    private static readonly Dictionary<string, ConfigHookFunc> stopHooks = new Dictionary<string, ConfigHookFunc>();

    private static bool roundEndRegistered;

    public void CallHook(string name, params object[] args) { }

    public void CallPatch(MethodBase method) { }

    public void AddHook(string name, string identifier, ConfigHookFunc func)
    {
      Dictionary<string, ConfigHookFunc> table = name switch
      {
        RoundEndHook => roundEndHooks,
        StopHook => stopHooks,
        _ => null
      };

      if (table is null) return;

      if (func is null) table.Remove(identifier);
      else table[identifier] = func;

      if (name == RoundEndHook) EnsureRoundEndRegistered();
    }

    private static void EnsureRoundEndRegistered()
    {
      if (roundEndRegistered) return;
      roundEndRegistered = true;

      Plugin.HookService.RegisterHook<PluginRoundEndedDelegate>(session => InvokeAll(roundEndHooks, session));
    }

    internal static void RunStopHooks() => InvokeAll(stopHooks, null);

    private static void InvokeAll(Dictionary<string, ConfigHookFunc> hooks, object arg)
    {
      foreach (ConfigHookFunc hook in hooks.Values.ToArray())
      {
        try
        {
          hook?.Invoke(new[] { arg });
        }
        catch (Exception e)
        {
          Logger.Default.Warning($"Config hook failed: {e.Message}");
        }
      }
    }
  }
}
