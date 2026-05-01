using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;

namespace Lethaltrauma
{
  public static class BotAim_Patches
  {
    public static void Patch(Harmony harmony)
    {
      harmony.Patch(
        original: typeof(HumanPrefab).GetMethod("InitializeCharacter", AccessTools.all),
        postfix: new HarmonyMethod(typeof(BotAim_Patches).GetMethod("HumanPrefab_InitializeCharacter_Postfix"))
      );
    }

    public static void HumanPrefab_InitializeCharacter_Postfix(HumanPrefab __instance, Character npc, ISpatialEntity positionToStayIn = null)
    {
      if (Mod.Config == null || !Mod.Config.OverrideAim) return;

      if (npc.AIController is HumanAIController humanAI)
      {
        humanAI.AimSpeed = Mod.Config.AimSpeed;
        humanAI.AimAccuracy = Mod.Config.AimAccuracy;
      }
    }

    public static void GlobalAimUpdate()
    {
      foreach (Character character in Character.CharacterList)
      {
        if (Mod.Config.OverrideHealthMult)
        {
          if (character.AIController is HumanAIController humanAI)
          {
            humanAI.AimSpeed = Mod.Config.AimSpeed;
            humanAI.AimAccuracy = Mod.Config.AimAccuracy;
          }
        }
        else
        {
          if (character.humanPrefab != null)
          {
            if (character.AIController is HumanAIController humanAI)
            {
              humanAI.AimSpeed = character.humanPrefab.AimSpeed;
              humanAI.AimAccuracy = character.humanPrefab.AimAccuracy;
            }
          }
        }
      }
    }



  }
}