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
  public static class PressureKillDelay_Patches
  {
    [HarmonyTargetMethod]
    public static MethodBase Character_Constructor() => typeof(Character).GetConstructors(AccessTools.all)[0];

    [HarmonyPostfix]
    public static void Character_Constructor_Postfix(Character __instance)
    {
      __instance.CharacterHealth.PressureKillDelay = Settings.PressureKillDelay.Value;
    }

    public static float GlobalPressureKillDelay
    {
      set
      {
        foreach (Character character in Character.CharacterList)
        {
          character.CharacterHealth.PressureKillDelay = value;
        }
      }
    }



  }
}