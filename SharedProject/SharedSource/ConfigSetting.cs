using System;
using System.Text.Json;
using System.Xml.Linq;

using Barotrauma;
using Barotrauma.Extensions;
using Barotrauma.Networking;
using Barotrauma.Plugins;
using BaroJunk;

namespace Lethaltrauma
{
  public sealed class ConfigSetting : ISetting
  {
    public static ConfigSetting Instance { get; } = new ConfigSetting();

    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
      IncludeFields = false,
      WriteIndented = false
    };

    private bool dirty;

    private ConfigSetting() { }

    public Identifier Identifier => "Lethaltrauma.Config".ToIdentifier();

    public SettingSyncMode SyncMode => SettingSyncMode.ServerAuthority;

    public SettingIsAllowedToSetDelegate IsAllowedToSet { get; set; } =
      (setting, client) => Plugin.HasConfigPermission(client);

    public bool HasPendingChanges => false;

    public bool IsDirty
    {
      get => dirty;
      set => dirty = value;
    }

    public bool ShowInUI { get; set; } = false;

    public void ResetToDefault()
    {
    }

    public void Apply()
    {
      dirty = true;
    }

    public void Revert()
    {
    }

    public bool ShouldSave() => false;

    public void Load(XElement element)
    {
    }

    public void Save(XElement element)
    {
    }

    public void WriteMessage(IWriteMessage writeMessage)
    {
      writeMessage.WriteString(JsonSerializer.Serialize(Plugin.Config, JsonOptions));
    }

    public void ReadMessage(IReadMessage readMessage)
    {
      string json = readMessage.ReadString();
      if (string.IsNullOrWhiteSpace(json)) return;

      Config received = JsonSerializer.Deserialize<Config>(json, JsonOptions);
      if (received is null) return;

      received.CopyTo(Plugin.Config);
      Plugin.Config.GetReactiveCore().RaiseUpdated();
    }

#if CLIENT
    public void AddUI(GUIFrame content)
    {
    }
#endif
  }
}
