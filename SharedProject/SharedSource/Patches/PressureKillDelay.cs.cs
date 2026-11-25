using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;

namespace Lethaltrauma
{
  public static class PressureKillDelay_Patches
  {
    public static void Patch(Harmony harmony)
    {
      harmony.Patch(
        original: typeof(Character).GetConstructors(AccessTools.all)[0],
        postfix: new HarmonyMethod(typeof(PressureKillDelay_Patches).GetMethod("Character_Constructor_Postfix"))
      );
    }

    public static void Character_Constructor_Postfix(Character __instance)
    {
      if (Mod.Config == null) return;
      __instance.CharacterHealth.PressureKillDelay = Mod.Config.PressureKillDelay;
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