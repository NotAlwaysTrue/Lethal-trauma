using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;

namespace Lethaltrauma
{
  [HarmonyPatch(typeof(CampaignMode), nameof(CampaignMode.OutpostNPCAttacked))]
  public static class CampaignMode_OutpostNPCAttacked_Patch
  {
    [HarmonyPrefix]
    public static bool CampaignMode_OutpostNPCAttacked_Prefix(Character npc, Character attacker, AttackResult attackResult)
    {
      if (Settings.NoReputationLossInMask.Value && attacker.HideFace) return false;
      return true;
    }
  }

  [HarmonyPatch(typeof(HumanAIController), nameof(HumanAIController.ApplyStealingReputationLoss))]
  public static class HumanAIController_ApplyStealingReputationLoss_Patch
  {
    [HarmonyPrefix]
    public static bool HumanAIController_ApplyStealingReputationLoss_Prefix(Item item)
    {
      if (Settings.NoReputationLossInMask.Value && Character.Controlled?.HideFace == true) return false;
      return true;
    }
  }

  [HarmonyPatch(typeof(HumanAIController), nameof(HumanAIController.StructureDamaged))]
  public static class HumanAIController_StructureDamaged_Patch
  {
    [HarmonyPrefix]
    public static bool HumanAIController_StructureDamaged_Replace(HumanAIController __instance, MapEntity structure, float damageAmount, Character attacker)
    {
      const float MaxDamagePerSecond = 5.0f;
      const float MaxDamagePerFrame = MaxDamagePerSecond * (float)Timing.Step;

      const float WarningThreshold = 5.0f;
      const float ArrestThreshold = 20.0f;
      const float KillThreshold = 50.0f;

      if (attacker == null || damageAmount <= 0.0f) { return false; }
      if (structure is not Structure structureEntity) { return true; }
      if (structureEntity.Submarine == null || !structureEntity.Submarine.Info.IsOutpost || attacker.TeamID == structureEntity.Submarine.TeamID) { return false; }
      //structure not indestructible = something that's "meant" to be destroyed, like an ice wall in mines
      if (!structureEntity.Prefab.IndestructibleInOutposts) { return false; }

      bool someoneSpoke = false;
      float maxAccumulatedDamage = 0.0f;
      foreach (Character otherCharacter in Character.CharacterList)
      {
        if (otherCharacter == attacker || otherCharacter.TeamID == attacker.TeamID || otherCharacter.IsDead ||
            otherCharacter.Info?.Job == null ||
            otherCharacter.AIController is not HumanAIController otherHumanAI ||
            Vector2.DistanceSquared(otherCharacter.WorldPosition, attacker.WorldPosition) > 1000.0f * 1000.0f)
        {
          continue;
        }
        if (!otherCharacter.CanSeeTarget(attacker, seeThroughWindows: true)) { continue; }

        otherHumanAI.structureDamageAccumulator.TryAdd(attacker, 0.0f);
        float prevAccumulatedDamage = otherHumanAI.structureDamageAccumulator[attacker];
        otherHumanAI.structureDamageAccumulator[attacker] += MathHelper.Clamp(damageAmount, -MaxDamagePerFrame, MaxDamagePerFrame);
        float accumulatedDamage = Math.Max(otherHumanAI.structureDamageAccumulator[attacker], maxAccumulatedDamage);
        maxAccumulatedDamage = Math.Max(accumulatedDamage, maxAccumulatedDamage);

        if (GameMain.GameSession?.Campaign?.Map?.CurrentLocation?.Reputation != null && attacker.IsPlayer)
        {
          if (!Settings.NoReputationLossInMask.Value || !attacker.HideFace)
          {
            var reputationLoss = damageAmount * Reputation.ReputationLossPerWallDamage;
            GameMain.GameSession.Campaign.Map.CurrentLocation.Reputation.AddReputation(-reputationLoss, Reputation.MaxReputationLossFromWallDamage);
          }
        }

        if (!attacker.IsCriminal)
        {
          if (accumulatedDamage <= WarningThreshold) { return false; }

          if (accumulatedDamage > WarningThreshold && prevAccumulatedDamage <= WarningThreshold &&
              !someoneSpoke && !attacker.IsIncapacitated && attacker.Stun <= 0.0f)
          {
            //if the damage is still fairly low, wait and see if the character keeps damaging the walls to the point where we need to intervene
            if (accumulatedDamage < ArrestThreshold)
            {
              if (otherHumanAI.ObjectiveManager.CurrentObjective is AIObjectiveIdle idleObjective)
              {
                idleObjective.FaceTargetAndWait(attacker, 5.0f);
              }
            }
            otherCharacter.Speak(TextManager.Get("dialogdamagewallswarning").Value, null, Rand.Range(0.5f, 1.0f), "damageoutpostwalls".ToIdentifier(), 10.0f);
            someoneSpoke = true;
          }
        }

        // React if we are security
        if (attacker.IsCriminal ||
            (accumulatedDamage > ArrestThreshold && prevAccumulatedDamage <= ArrestThreshold) ||
            (accumulatedDamage > KillThreshold && prevAccumulatedDamage <= KillThreshold))
        {
          var combatMode = accumulatedDamage > KillThreshold ? AIObjectiveCombat.CombatMode.Offensive : AIObjectiveCombat.CombatMode.Arrest;
          if (combatMode == AIObjectiveCombat.CombatMode.Offensive)
          {
            attacker.IsCriminal = true;
          }
          if (!TriggerSecurity(otherHumanAI, combatMode))
          {
            // Else call the others
            foreach (Character security in Character.CharacterList.Where(c => c.TeamID == otherCharacter.TeamID).OrderBy(c => Vector2.DistanceSquared(attacker.WorldPosition, c.WorldPosition)))
            {
              if (!TriggerSecurity(security.AIController as HumanAIController, combatMode))
              {
                // Only alert one guard at a time
                return false;
              }
            }
          }
        }
      }

      bool TriggerSecurity(HumanAIController humanAI, AIObjectiveCombat.CombatMode combatMode)
      {
        if (humanAI == null) { return false; }
        if (!humanAI.Character.IsSecurity) { return false; }
        if (humanAI.ObjectiveManager.IsCurrentObjective<AIObjectiveCombat>()) { return false; }
        humanAI.AddCombatObjective(combatMode, attacker, delay: HumanAIController.GetReactionTime(),
            onCompleted: () =>
            {
              //if the target is arrested successfully, reset the damage accumulator
              foreach (Character anyCharacter in Character.CharacterList)
              {
                if (anyCharacter.AIController is HumanAIController anyAI)
                {
                  anyAI.structureDamageAccumulator?.Remove(attacker);
                }
              }
            });
        return true;
      }

      return false;
    }
  }
}