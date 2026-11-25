using System;

using Barotrauma.Networking;

namespace BaroJunk
{
  public class ConfigClientNetManager
  {
    public ConfigCore Config;
    public ConfigClientNetManager(ConfigCore config) => Config = config;

    public bool Enabled { get; set; }

    public void ReactivePropChanged()
    {
      if (!Enabled || !Config.Settings.SyncOnPropChanged) return;
      Config.Sync();
    }

    public void ConfigUpdated()
    {
      if (!Enabled) return;
      Config.Sync();
    }

    public void UseStrategy(NetManagerStrategy strategy) => Enabled = strategy.NetSync;
  }
}
