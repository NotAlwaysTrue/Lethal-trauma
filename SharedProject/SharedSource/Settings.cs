using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Barotrauma;
using Barotrauma.Extensions;
using Barotrauma.Plugins;
using Microsoft.Xna.Framework;

namespace Lethaltrauma
{
  public static class Settings
  {
    private const string IdentifierPrefix = "Lethaltrauma.";

    private const float MultiplierMax = 10.0f;
    private const float MultiplierStep = 0.05f;

    #region Damage

    public static readonly FloatSetting RangedWeaponDamage =
      Float(nameof(RangedWeaponDamage), 1.0f, "Ranged weapon damage multiplier");

    public static readonly FloatSetting MeleeWeaponDamage =
      Float(nameof(MeleeWeaponDamage), 1.0f, "Melee weapon damage multiplier");

    public static readonly FloatSetting ExplosionDamage =
      Float(nameof(ExplosionDamage), 1.0f, "Explosion damage multiplier");

    public static readonly FloatSetting TurretDamage =
      Float(nameof(TurretDamage), 1.0f, "Turret damage multiplier");

    public static readonly FloatSetting MonsterAttackDamage =
      Float(nameof(MonsterAttackDamage), 1.0f, "Monster attack damage multiplier");

    #endregion

    #region Health

    public static readonly BooleanSetting OverrideHealthMult =
      Bool(nameof(OverrideHealthMult), false, "Override health multipliers");

    public static readonly FloatSetting CrewHumanHealth =
      Float(nameof(CrewHumanHealth), 1.0f, "Crew human health multiplier");

    public static readonly FloatSetting EnemyHumanHealth =
      Float(nameof(EnemyHumanHealth), 1.0f, "Enemy human health multiplier");

    public static readonly FloatSetting MonsterHealth =
      Float(nameof(MonsterHealth), 1.0f, "Monster health multiplier");

    #endregion

    #region Aim

    public static readonly BooleanSetting OverrideAim =
      Bool(nameof(OverrideAim), false, "Override bot aim");

    public static readonly FloatSetting AimSpeed =
      Float(nameof(AimSpeed), 1.0f, "Aim speed");

    public static readonly FloatSetting AimAccuracy =
      Float(nameof(AimAccuracy), 1.0f, "Aim accuracy");

    #endregion

    #region Misc

    public static readonly BooleanSetting NoReputationLossInMask =
      Bool(nameof(NoReputationLossInMask), false, "No reputation loss while wearing a mask");

    public static readonly FloatSetting PressureKillDelay =
      Float(nameof(PressureKillDelay), 5.0f, "Pressure kill delay", min: 0.0f, max: 60.0f, step: 0.5f);

    public static readonly BooleanSetting AllowFriendlyFire =
      Bool(nameof(AllowFriendlyFire), true, "Allow friendly fire");

    public static readonly FloatSetting WeaponStructureDamage =
      Float(nameof(WeaponStructureDamage), 1.0f, "Weapon structure damage multiplier");

    public static readonly FloatSetting MonsterStructureDamage =
      Float(nameof(MonsterStructureDamage), 1.0f, "Monster structure damage multiplier");

    #endregion

    public static readonly IReadOnlyList<(string Name, ISetting Setting)> All = new (string, ISetting)[]
    {
      (nameof(RangedWeaponDamage), RangedWeaponDamage),
      (nameof(MeleeWeaponDamage), MeleeWeaponDamage),
      (nameof(ExplosionDamage), ExplosionDamage),
      (nameof(TurretDamage), TurretDamage),
      (nameof(MonsterAttackDamage), MonsterAttackDamage),

      (nameof(OverrideHealthMult), OverrideHealthMult),
      (nameof(CrewHumanHealth), CrewHumanHealth),
      (nameof(EnemyHumanHealth), EnemyHumanHealth),
      (nameof(MonsterHealth), MonsterHealth),

      (nameof(OverrideAim), OverrideAim),
      (nameof(AimSpeed), AimSpeed),
      (nameof(AimAccuracy), AimAccuracy),

      (nameof(NoReputationLossInMask), NoReputationLossInMask),
      (nameof(PressureKillDelay), PressureKillDelay),
      (nameof(AllowFriendlyFire), AllowFriendlyFire),
      (nameof(WeaponStructureDamage), WeaponStructureDamage),
      (nameof(MonsterStructureDamage), MonsterStructureDamage),
    };

    public static void RegisterAll(ISettingsService settingsService)
    {
      foreach ((string _, ISetting setting) in All)
      {
        settingsService.RegisterSetting(setting);
      }
    }

    public static ISetting Find(string name)
      => All.FirstOrDefault(entry => string.Equals(entry.Name, name, StringComparison.OrdinalIgnoreCase)).Setting;

    public static object GetValue(ISetting setting) => setting switch
    {
      BaseSetting<float> number => number.Value,
      BaseSetting<bool> flag => flag.Value,
      _ => null
    };

    public static bool TrySetValue(ISetting setting, string raw)
    {
      if (setting is null || raw is null) return false;

      switch (setting)
      {
        case BaseSetting<float> number:
          if (!float.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out float numberValue)) return false;
          number.Set(numberValue);
          return true;

        case BaseSetting<bool> flag:
          if (!bool.TryParse(raw, out bool flagValue)) return false;
          flag.Set(flagValue);
          return true;

        default:
          return false;
      }
    }

    public static Dictionary<string, object> ToDictionary()
    {
      Dictionary<string, object> values = new Dictionary<string, object>(All.Count);
      foreach ((string name, ISetting setting) in All)
      {
        values[name] = GetValue(setting);
      }
      return values;
    }

    public static object[] Snapshot()
    {
      object[] snapshot = new object[All.Count];
      for (int i = 0; i < All.Count; i++)
      {
        snapshot[i] = GetValue(All[i].Setting);
      }
      return snapshot;
    }

    public static bool HasChanged(object[] snapshot)
    {
      if (snapshot is null || snapshot.Length != All.Count) return true;

      for (int i = 0; i < All.Count; i++)
      {
        if (!Equals(GetValue(All[i].Setting), snapshot[i])) return true;
      }
      return false;
    }

    #region Factories

    private static FloatSetting Float(string name, float defaultValue, string label, float min = 0.0f, float max = MultiplierMax, float step = MultiplierStep)
      => new FloatSetting((IdentifierPrefix + name).ToIdentifier(), defaultValue, label)
      {
        Range = new Vector2(min, max),
        StepValue = step,
        ShowInUI = true,
        SyncMode = SettingSyncMode.ServerAuthority,
        IsAllowedToSet = (setting, client) => Plugin.HasConfigPermission(client)
      };

    private static BooleanSetting Bool(string name, bool defaultValue, string label)
      => new BooleanSetting((IdentifierPrefix + name).ToIdentifier(), defaultValue, label)
      {
        ShowInUI = true,
        SyncMode = SettingSyncMode.ServerAuthority,
        IsAllowedToSet = (setting, client) => Plugin.HasConfigPermission(client)
      };

    #endregion
  }
}
