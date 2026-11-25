using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;

namespace Lethaltrauma
{
  [HarmonyPatch]
  public static class HealthMultipliers_Patches
  {
    [HarmonyTargetMethod]
    public static MethodBase Character_Constructor() => typeof(Character).GetConstructors(AccessTools.all)[0];

    [HarmonyPostfix]
    public static void Character_Constructor_Postfix(Character __instance)
    {
      if (Plugin.Config == null || !Plugin.Config.OverrideHealthMult) return;
      Utils.RunWithDelay(() => SetHealthMultiplier(__instance), 100);
    }

    public static void GlobalUpdateHealthMultipliers()
    {
      foreach (Character character in Character.CharacterList)
      {
        if (Plugin.Config.OverrideHealthMult)
        {
          SetHealthMultiplier(character);
        }
        else
        {
          ResetHealthMultiplier(character);
        }
      }
    }

    public static void SetHealthMultiplier(Character character)
    {
      if (character.IsHuman)
      {
        if (character.TeamID == CharacterTeamType.Team1)
        {
          character.HumanPrefabHealthMultiplier = Plugin.Config.CrewHumanHealth;
        }
        else
        {
          character.HumanPrefabHealthMultiplier = Plugin.Config.EnemyHumanHealth;
        }
      }
      else
      {
        character.HumanPrefabHealthMultiplier = Plugin.Config.MonsterHealth;
      }
    }
    public static void ResetHealthMultiplier(Character character)
    {
      if (character.humanPrefab != null)
      {
        character.HumanPrefabHealthMultiplier = character.humanPrefab.HealthMultiplier;
        if (GameMain.NetworkMember != null)
        {
          character.HumanPrefabHealthMultiplier *= character.humanPrefab.HealthMultiplierInMultiplayer;
        }
      }
      else
      {
        character.HumanPrefabHealthMultiplier = 1.0f;
      }
    }

  }
}