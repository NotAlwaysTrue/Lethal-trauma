using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Barotrauma;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Text;

namespace BaroJunk
{
  public class ConfigCommandsManager
  {
    public ConfigCore Config;
    public ConfigCommandsManager(ConfigCore config) => Config = config;

    private string registeredName;

    private string commandName;
    public string CommandName
    {
      get => commandName;
      set
      {
        commandName = value;
        UpdateCommand(commandName);
      }
    }

    private void UpdateCommand(string name)
    {
      if (!string.IsNullOrEmpty(registeredName))
      {
        Config.Facades.ConsoleFacade.Deregister(registeredName);
        registeredName = null;
      }

      if (name is null) return;

      Config.Facades.ConsoleFacade.Register(name, "", EditConfig_VanillaCommand, Config.ToHints());
      registeredName = name;
    }

    public void EditConfig_VanillaCommand(string[] args)
    {
      if (args.Length == 0)
      {
        Config.Logger.Log(Config.ToText());
        return;
      }

      ReactiveEntry entry = Config.ReactiveGetEntry(args[0]);

      if (args.Length == 1)
      {
        if (entry.IsValid)
        {
          Config.Logger.Log(entry.Value);
        }
        else
        {
          Config.Logger.Warning("No such prop");
        }
        return;
      }

      if (args.Length > 1)
      {
        if (Config.Facades.NetFacade.IsMultiplayer && !Config.Facades.NetFacade.DoIHavePermissions())
        {
          Config.Logger.Warning(Config.Facades.NetFacade.DontHavePermissionsString);
          return;
        }

        if (!entry.IsValid)
        {
          Config.Logger.Warning("No such prop");
          return;
        }

        try
        {
          SimpleResult result = Config.Parser.Parse(args[1], entry.Type);
          if (result.Ok)
          {
            entry.Value = result.Result;
            if (Config.Facades.NetFacade.IsMultiplayer) Config.Sync();
            Config.Logger.Log($"{args[0]} = [{result.Result}]");
          }
          else
          {
            Config.Logger.Warning(result.Details);
          }
        }
        catch (Exception e)
        {
          Config.Logger.Warning(e.Message);
        }
      }
    }
  }
}
