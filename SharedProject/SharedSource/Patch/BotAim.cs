using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;

namespace Lethaltrauma
{
  [HarmonyPatch(typeof(HumanPrefab), nameof(HumanPrefab.InitializeCharacter))]
  public static class BotAim_Patches
  {
    [HarmonyPostfix]
    public static void HumanPrefab_InitializeCharacter_Postfix(HumanPrefab __instance, Character npc, ISpatialEntity positionToStayIn = null)
    {
      if (Plugin.Config == null || !Plugin.Config.OverrideAim) return;

      if (npc.AIController is HumanAIController humanAI)
      {
        humanAI.AimSpeed = Plugin.Config.AimSpeed;
        humanAI.AimAccuracy = Plugin.Config.AimAccuracy;
      }
    }

    public static void GlobalAimUpdate()
    {
      foreach (Character character in Character.CharacterList)
      {
        if (Plugin.Config.OverrideHealthMult)
        {
          if (character.AIController is HumanAIController humanAI)
          {
            humanAI.AimSpeed = Plugin.Config.AimSpeed;
            humanAI.AimAccuracy = Plugin.Config.AimAccuracy;
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