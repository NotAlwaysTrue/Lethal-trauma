using System;
using System.Xml.Linq;

using Barotrauma;
using Barotrauma.Networking;
using Barotrauma.Plugins;

namespace Lethaltrauma
{
  public class ObservableFloatSetting : FloatSetting
  {
    public event Action Changed;

    public ObservableFloatSetting(Identifier identifier, float defaultValue, LocalizedString label)
      : base(identifier, defaultValue, label)
    {
    }

    public override void Set(float newValue) => Update(() => base.Set(newValue));

    public override void Load(XElement element) => Update(() => base.Load(element));

    public override void ReadMessage(IReadMessage readMessage) => Update(() => base.ReadMessage(readMessage));

    private void Update(Action update)
    {
      float previous = Value;
      update();
      if (!Equals(Value, previous)) Changed?.Invoke();
    }
  }

  public class ObservableBooleanSetting : BooleanSetting
  {
    public event Action Changed;

    public ObservableBooleanSetting(Identifier identifier, bool defaultValue, LocalizedString label)
      : base(identifier, defaultValue, label)
    {
    }

    public override void Set(bool newValue) => Update(() => base.Set(newValue));

    public override void Load(XElement element) => Update(() => base.Load(element));

    public override void ReadMessage(IReadMessage readMessage) => Update(() => base.ReadMessage(readMessage));

    private void Update(Action update)
    {
      bool previous = Value;
      update();
      if (Value != previous) Changed?.Invoke();
    }
  }
}
