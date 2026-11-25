using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Barotrauma;
using Lethaltrauma;

namespace BaroJunk
{
  public static class PluginCommands
  {
    public static List<DebugConsole.Command> AddedCommands = [];

    public static void Add(string name, Action<string[]> callback, Func<string[][]> hints = null, string help = "", bool addToStart = true)
    {
      DebugConsole.Command command = new DebugConsole.Command(name, help, (string[] args) =>
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
      AddedCommands.Add(command);

      if (addToStart)
      {
        DebugConsole.Commands.Insert(0, command);
      }
      else
      {
        DebugConsole.Commands.Add(command);
      }
    }

    public static void RemoveCommands()
    {
      AddedCommands.ForEach(c => DebugConsole.Commands.Remove(c));
      AddedCommands.Clear();
    }
  }
}
