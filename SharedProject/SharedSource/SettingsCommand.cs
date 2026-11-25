using System;
using System.Collections.Generic;
using System.Linq;

using Barotrauma;
using Barotrauma.Plugins;

namespace Lethaltrauma
{
  public static class SettingsCommand
  {
    private const string CommandName = "lt";

    public static void Register()
      => Plugin.RegisterCommand(
        CommandName,
        "Shows or changes Lethaltrauma's settings. Usage: lt [setting] [value]",
        CommandFlags.None,
        Execute,
        Hints);

    public static void Deregister() => Plugin.DeregisterCommand(CommandName);

    private static void Execute(string[] args)
    {
      if (args is null || args.Length == 0)
      {
        foreach ((string name, ISetting setting) in Settings.All)
        {
          Plugin.Logger.Log($"{name} = {Settings.GetValue(setting)}");
        }
        return;
      }

      ISetting target = Settings.Find(args[0]);

      if (target is null)
      {
        Plugin.Logger.Warning($"No such setting: {args[0]}");
        return;
      }

      if (args.Length == 1)
      {
        Plugin.Logger.Log($"{args[0]} = {Settings.GetValue(target)}");
        return;
      }

      if (!Plugin.LocalClientHasConfigPermission())
      {
        Plugin.Logger.Warning("You don't have permission to change the settings.");
        return;
      }

      if (!Settings.TrySetValue(target, args[1]))
      {
        Plugin.Logger.Warning($"Can't parse '{args[1]}' as a value for {args[0]}.");
        return;
      }

      Plugin.ApplySettings();

      Plugin.Logger.Log($"{args[0]} = {Settings.GetValue(target)}");
    }

    private static string[][] Hints()
      => new[] { Settings.All.Select(entry => entry.Name).ToArray() };
  }
}
