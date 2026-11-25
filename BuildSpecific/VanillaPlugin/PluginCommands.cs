using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Barotrauma;
using Barotrauma.Plugins;
using Lethaltrauma;

namespace BaroJunk
{
  public static class PluginCommands
  {
    public static string AssemblyName => typeof(PluginCommands).Assembly.GetName().Name;

    public static List<string> AddedCommands = new List<string>();

    public static void Add(string name, Action<string[]> callback, Func<string[][]> hints = null, string help = "", bool addToStart = true)
    {
      Plugin.RegisterCommand(name, help, CommandFlags.None, args =>
      {
        try
        {
          callback(args);
        }
        catch (Exception e)
        {
          Logger.Default.Error(e.Message);
        }
      }, hints);

      AddedCommands.Add(name);
    }

    public static void PrintCommands()
    {
      foreach (string name in AddedCommands)
      {
        Logger.Default.Log(name);
      }
    }

    public static void RemoveCommands()
    {
      foreach (string name in AddedCommands.ToList())
      {
        Plugin.DeregisterCommand(name);
      }

      AddedCommands.Clear();
    }
  }
}
