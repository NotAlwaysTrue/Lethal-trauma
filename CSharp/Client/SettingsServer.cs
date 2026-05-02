using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;
using BaroJunk;
using Qoollo.Net.Http;
using Barotrauma.Steam;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Lethaltrauma
{
  public class SettingsServer
  {
    public static int Port = 6999;
    public static string SettingsURL = $"http://localhost:{Port}/";
    public HttpServer HttpServer { get; private set; }

    public Guid ID = Guid.NewGuid();

    public void OpenSettingsPage()
    {
      if (HttpServer is null) Start();
      SteamManager.OverlayCustomUrl(SettingsURL);
    }

    public string SetConfig(HttpListenerRequest request)
    {
      using (Stream body = request.InputStream)
      using (StreamReader reader = new StreamReader(body, request.ContentEncoding))
      {
        string json = reader.ReadToEnd();
        Config config = JsonSerializer.Deserialize<Config>(json);
        config.CopyTo(Mod.Config);
        Mod.Config.GetReactiveCore().RaiseUpdated();
        Mod.Config.GetCore().Sync();
      }

      return "bruh";
    }

    public string GetConfig(HttpListenerRequest request)
      => JsonSerializer.Serialize(Mod.Config);

    public string GetServerID(HttpListenerRequest request)
    {
      return ID.ToString();
    }

    public void Start()
    {
      HttpServer = new HttpServer(Port);
      HttpServer.ServeStatic(new DirectoryInfo(Path.Combine(Mod.Package.Dir, "Web Root")));
      HttpServer.Post["/SetConfig"] = SetConfig;
      HttpServer.Get["/GetConfig"] = GetConfig;
      HttpServer.Get["/GetServerID"] = GetServerID;
      HttpServer.Run();
    }

    public void Stop()
    {
      if (HttpServer != null && HttpServer.IsListening)
      {
        HttpServer.Stop();
        HttpServer = null;
      }
    }
  }
}