using System;

using Barotrauma;
using Barotrauma.Plugins;
using Lethaltrauma;

namespace BaroJunk
{
  public interface IConsoleFacade
  {
    public void Register(string name, string help, Action<string[]> onExecute, Func<string[][]> hints);
    public void Deregister(string name);
  }

  public class ConsoleFacade : IConsoleFacade
  {
    public void Register(string name, string help, Action<string[]> onExecute, Func<string[][]> hints)
      => Plugin.RegisterCommand(name, help, CommandFlags.None, onExecute, hints);

    public void Deregister(string name)
      => Plugin.DeregisterCommand(name);
  }
}
