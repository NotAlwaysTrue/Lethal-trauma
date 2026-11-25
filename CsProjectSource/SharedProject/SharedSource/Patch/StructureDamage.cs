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
  [HarmonyPatch(typeof(Explosion), nameof(Explosion.RangedStructureDamage))]
  public static class Explosion_RangedStructureDamage_Patch
  {
    [HarmonyPrefix]
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
  }

  [HarmonyPatch(typeof(Structure), nameof(Structure.AddDamage), new Type[] {
    typeof(Character),
    typeof(Vector2),
    typeof(Attack),
    typeof(Vector2),
    typeof(float),
    typeof(bool),
  })]
  public static class Structure_AddDamage_Patch
  {
    [HarmonyPrefix]
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
  }

  [HarmonyPatch(typeof(Item), nameof(Item.AddDamage))]
  public static class Item_AddDamage_Patch
  {
    [HarmonyPrefix]
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