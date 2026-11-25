using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

using Barotrauma;
using Barotrauma.Networking;
using Lethaltrauma;

namespace BaroJunk
{
  public interface INetFacade
  {
    public bool IsMultiplayer { get; }
    public bool IsClient { get; }
    public HashSet<string> AlreadyListeningFor { get; }
    public string DontHavePermissionsString { get; }
    public void ClientSend(string header);
    public void ClientEncondeAndSend(string header, ConfigCore config);
    public void ServerSend(string header, Client client);
    public void ServerEncondeAndSend(string header, ConfigCore config, Client client);
    public void ServerEncondeAndBroadcast(string header, ConfigCore config);
    public void ListenForServer(string header, Action<IReadMessage> callback);
    public void ListenForClients(string header, Action<IReadMessage, Client> callback);
    public bool DoesClientHasPermissions(Client client);
    public bool DoIHavePermissions();
  }

  public class NetFacade : INetFacade
  {
    public bool IsMultiplayer => GameMain.NetworkMember != null;

    public bool IsClient => GameMain.NetworkMember?.IsClient == true;

    public HashSet<string> AlreadyListeningFor { get; } = new HashSet<string>();

    public string DontHavePermissionsString => "You need to be the host or have ConsoleCommands permission to do that";

    public bool DoIHavePermissions() => Plugin.LocalClientHasConfigPermission();

    public bool DoesClientHasPermissions(Client client) => Plugin.HasConfigPermission(client);

    public void ClientSend(string header) => MarkDirty();

    public void ClientEncondeAndSend(string header, ConfigCore config) => MarkDirty();

    public void ServerSend(string header, Client client) => MarkDirty();

    public void ServerEncondeAndSend(string header, ConfigCore config, Client client) => MarkDirty();

    public void ServerEncondeAndBroadcast(string header, ConfigCore config) => MarkDirty();

    public void ListenForServer(string header, Action<IReadMessage> callback) => AlreadyListeningFor.Add(header);

    public void ListenForClients(string header, Action<IReadMessage, Client> callback) => AlreadyListeningFor.Add(header);

    private static void MarkDirty() => ConfigSetting.Instance.IsDirty = true;
  }
}
