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


namespace Lethaltrauma
{
  public class SettingsServer
  {
    public static int Port = 6999;
    public static string SettingsURL = $"http://localhost:{Port}/";
    public HttpServer HttpServer { get; private set; }

    public void OpenSettingsPage()
    {
      if (HttpServer is null) Setup();
      SteamManager.OverlayCustomUrl(SettingsURL);
    }

    private void Setup()
    {
      HttpServer = new(Port);
      HttpServer.ServeStatic(new DirectoryInfo(Path.Combine(Mod.Package.Dir, "Web Root")));
      HttpServer.Post["/SetConfig"] = SetConfig;
      HttpServer.Get["/GetConfig"] = GetConfig;
      HttpServer.Run();
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
      }

      return "bruh";
    }

    public string GetConfig(HttpListenerRequest request)
      => JsonSerializer.Serialize(Mod.Config);

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