using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Barotrauma.Items.Components;

namespace Lethaltrauma
{
  public static class StructureDamage_Patches
  {
    public static void Patch(Harmony harmony)
    {
      harmony.Patch(
        original: typeof(Structure).GetMethod("AddDamage", AccessTools.all, new Type[]{
          typeof(Character),
          typeof(Vector2),
          typeof(Attack),
          typeof(Vector2),
          typeof(float),
          typeof(bool),
        }),
        prefix: new HarmonyMethod(typeof(StructureDamage_Patches).GetMethod("Structure_AddDamage_Prefix"))
      );

      harmony.Patch(
        original: typeof(Item).GetMethod("AddDamage", AccessTools.all),
        prefix: new HarmonyMethod(typeof(StructureDamage_Patches).GetMethod("Item_AddDamage_Prefix"))
      );

      harmony.Patch(
        original: typeof(Explosion).GetMethod("RangedStructureDamage", AccessTools.all),
        prefix: new HarmonyMethod(typeof(StructureDamage_Patches).GetMethod("Explosion_RangedStructureDamage_Prefix"))
      );
    }

    public static void Explosion_RangedStructureDamage_Prefix(ref float damage, Character attacker)
    {
      if (attacker is null) return;
      if (attacker.IsHuman)
      {
        damage *= Plugin.Config.WeaponStructureDamage;
      }
      else
      {
        damage *= Plugin.Config.MonsterStructureDamage;
      }
    }

    public static void Structure_AddDamage_Prefix(Structure __instance, ref AttackResult __result, Character attacker, Vector2 worldPosition, Attack attack, Vector2 impulseDirection, float deltaTime, bool playSound = false)
    {
      if (attacker is null) return;
      if (attacker.IsHuman)
      {
        attack.DamageMultiplier = Plugin.Config.WeaponStructureDamage;
      }
      else
      {
        attack.DamageMultiplier = Plugin.Config.MonsterStructureDamage;
      }
    }

    public static void Item_AddDamage_Prefix(Item __instance, ref AttackResult __result, Character attacker, Vector2 worldPosition, Attack attack, Vector2 impulseDirection, float deltaTime, bool playSound = true)
    {
      if (attacker is null) return;

      bool isDoor = __instance.GetComponent<Door>() != null;

      if (isDoor)
      {
        if (attacker.IsHuman)
        {
          attack.DamageMultiplier = Plugin.Config.WeaponStructureDamage;
        }
        else
        {
          attack.DamageMultiplier = Plugin.Config.MonsterStructureDamage;
        }
      }
    }

  }
}