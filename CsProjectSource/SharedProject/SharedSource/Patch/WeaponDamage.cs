using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;

using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using Barotrauma.Items.Components;

namespace Lethaltrauma
{
  [HarmonyPatch(typeof(Projectile), nameof(Projectile.HandleProjectileCollision))]
  public static class Projectile_HandleProjectileCollision_Patch
  {
    [HarmonyPostfix]
    public static void Projectile_HandleProjectileCollision_Postfix(Projectile __instance, ref bool __result, Fixture target, Vector2 collisionNormal, Vector2 velocity)
    {
      SharedState.fromRangedWeapon = null;
    }
  }

  [HarmonyPatch(typeof(Character), nameof(Character.ApplyAttack))]
  public static class Character_ApplyAttack_Patch
  {
    [HarmonyPrefix]
    public static bool Character_ApplyAttack_Prefix(Character __instance, ref AttackResult __result, Character attacker, Vector2 worldPosition, Attack attack, float deltaTime, Vector2 impulseDirection, bool playSound = false, Limb targetLimb = null, float penetration = 0f)
    {
      if (Plugin.Config == null) return true;

      bool fromTurret = attack.SourceItem?.GetComponent<Projectile>()?.Launcher?.GetComponent<Turret>() != null;
      bool fromRangedWeapon = attack.SourceItem?.GetComponent<RangedWeapon>() != null || attack.SourceItem?.GetComponent<Projectile>()?.Launcher?.GetComponent<RangedWeapon>() != null;
      bool fromMeleeWeapon = attack.SourceItem?.GetComponent<MeleeWeapon>() != null;

      SharedState.fromRangedWeapon = fromRangedWeapon;

      if (fromTurret) attack.DamageMultiplier = Plugin.Config.TurretDamage;
      if (fromRangedWeapon) attack.DamageMultiplier = Plugin.Config.RangedWeaponDamage;
      if (fromMeleeWeapon) attack.DamageMultiplier = Plugin.Config.MeleeWeaponDamage;



      //TODO there's probably more cases
      if (!fromTurret && !fromRangedWeapon && !fromMeleeWeapon)
      {
        attack.DamageMultiplier = Plugin.Config.MonsterAttackDamage;
      }

      return true;
    }
  }

  [HarmonyPatch(typeof(Explosion), nameof(Explosion.DamageCharacters))]
  public static class Explosion_DamageCharacters_Patch
  {
    [HarmonyPrefix]
    public static bool Explosion_DamageCharacters_Prefix(Explosion __instance, Vector2 worldPosition, Attack attack, float force, Entity damageSource, Character attacker)
    {
      if (Plugin.Config == null) return true;
      attack.DamageMultiplier = Plugin.Config.ExplosionDamage;

      return true;
    }
  }
}