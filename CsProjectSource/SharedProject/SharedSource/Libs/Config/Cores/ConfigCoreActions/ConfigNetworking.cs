using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

using Barotrauma;
using Barotrauma.Networking;
using Lethaltrauma;

namespace BaroJunk
{

  public partial class ConfigCore
  {
    public string NetHeader => ID;

    public void NetEncode(IWriteMessage msg)
    {
      foreach (ConfigEntry entry in this.GetEntriesRec())
      {
        SimpleResult result = NetParser.Encode(msg, entry.Value, entry.Type);
        if (!result.Ok)
        {
          Logger.Warning(result.Details);
        }
      }
    }

    public void NetDecode(IReadMessage msg)
    {
      foreach (ConfigEntry entry in this.GetEntriesRec())
      {
        SimpleResult result = NetParser.Decode(msg, entry.Type);
        if (result.Ok) entry.Value = result.Result;
        else Logger.Warning(result.Details);
      }

      Manager.RecievedConfigSync();
    }

    public SimpleResult Ask()
    {
      if (!Facades.NetFacade.IsMultiplayer) return SimpleResult.Failure("It's not multiplayer");
      return SimpleResult.Success();
    }

    public SimpleResult Sync()
    {
      if (!Facades.NetFacade.IsMultiplayer) return SimpleResult.Failure("It's not multiplayer");

      if (Facades.NetFacade.IsClient && !Facades.NetFacade.DoIHavePermissions())
      {
        return SimpleResult.Failure(Facades.NetFacade.DontHavePermissionsString);
      }

      ConfigSetting.Instance.IsDirty = true;
      return SimpleResult.Success();
    }
  }

}
