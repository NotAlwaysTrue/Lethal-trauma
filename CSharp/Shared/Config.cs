using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;
using BaroJunk;

namespace Lethaltrauma
{
  public class Config : IConfig
  {
    public float RangedWeaponDamage { get; set; } = 1.0f;
    public float MeleeWeaponDamage { get; set; } = 1.0f;
    public float ExplosionDamage { get; set; } = 1.0f;
    public float TurretDamage { get; set; } = 1.0f;
    public float MonsterAttackDamage { get; set; } = 1.0f;
    public bool OverrideHealthMult { get; set; } = false;
    public float CrewHumanHealth { get; set; } = 1.0f;
    public float EnemyHumanHealth { get; set; } = 1.0f;
    public float MonsterHealth { get; set; } = 1.0f;

    public bool OverrideAim { get; set; } = false;
    public float AimSpeed { get; set; } = 1.0f;
    public float AimAccuracy { get; set; } = 1.0f;

    public bool NoReputationLossInMask { get; set; } = false;
    public float PressureKillDelay { get; set; } = 5.0f;
    public bool AllowFriendlyFire { get; set; } = true;
    public float WeaponStructureDamage { get; set; } = 1.0f;
    public float MonsterStructureDamage { get; set; } = 1.0f;

  }
}