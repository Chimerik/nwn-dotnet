using System;
using System.Collections.Generic;
using NWN.Enums;
using NWN.Systems;

namespace NWN.ScriptHandlers
{
  static public class ActivateItemHandlers
  {
    public static Dictionary<string, Func<uint, uint, int>> Register = new Dictionary<string, Func<uint, uint, int>>
        {
            { "MenuTester", HandleMenuTesterActivate },
            { "test_block", HandleBlockTesterActivate },
            { "skillbook", HandleSkillBookActivate },
        };
    private static int HandleMenuTesterActivate(uint oItem, uint oActivator)
    {
      Console.WriteLine($"You activated the item {NWScript.GetName(oItem)}! {NWScript.GetName(oActivator)}");

      return Entrypoints.SCRIPT_HANDLED;
    }

    private static int HandleBlockTesterActivate(uint oItem, uint oActivator)
    {
      PlayerSystem.Player player;
      if (PlayerSystem.Players.TryGetValue(oActivator, out player))
      {
        player.BoulderBlock();
      }

      return Entrypoints.SCRIPT_HANDLED;
    }
    private static int HandleSkillBookActivate(uint oItem, uint oActivator)
    {
      PlayerSystem.Player player;
      if (PlayerSystem.Players.TryGetValue(oActivator, out player))
      {
        var FeatBook = oItem.AsItem();
        int FeatId = FeatBook.Locals.Int.Get("_SKILL_ID");
        if (!player.HasFeat((Feat)FeatId))
        {
          int value;
          if (int.TryParse(NWScript.Get2DAString("feat", "MINATTACKBONUS", FeatId), out value))
          {
            if (value > NWScript.GetBaseAttackBonus(player))
            {
              player.SendMessage("Vous n'êtes pas assez expérimenté en maniement des armes pour retirer quoique ce soit de cet ouvrage");
              return Entrypoints.SCRIPT_HANDLED;
            }
          }

          if (int.TryParse(NWScript.Get2DAString("feat", "MINSTR", FeatId), out value))
          {
            if (value > NWScript.GetAbilityScore(player, Ability.Strength, true))
            {
              player.SendMessage("Vous n'avez pas la force nécessaire pour retirer quoique ce soit de cet ouvrage");
              return Entrypoints.SCRIPT_HANDLED;
            }
          }

          if (int.TryParse(NWScript.Get2DAString("feat", "MINDEX", FeatId), out value))
          {
            if (value > NWScript.GetAbilityScore(player, Ability.Dexterity, true))
            {
              player.SendMessage("Vous n'avez pas la dextérité nécessaire pour retirer quoique ce soit de cet ouvrage");
              return Entrypoints.SCRIPT_HANDLED;
            }
          }

          if (int.TryParse(NWScript.Get2DAString("feat", "MINCON", FeatId), out value))
          {
            if (value > NWScript.GetAbilityScore(player, Ability.Constitution, true))
            {
              player.SendMessage("Vous n'avez pas la constitution nécessaire pour retirer quoique ce soit de cet ouvrage");
              return Entrypoints.SCRIPT_HANDLED;
            }
          }

          if (int.TryParse(NWScript.Get2DAString("feat", "MININT", FeatId), out value))
          {
            if (value > NWScript.GetAbilityScore(player, Ability.Intelligence, true))
            {
              player.SendMessage("Vous n'avez pas l'intelligence nécessaire pour retirer quoique ce soit de cet ouvrage");
              return Entrypoints.SCRIPT_HANDLED;
            }
          }

          if (int.TryParse(NWScript.Get2DAString("feat", "MINWIS", FeatId), out value))
          {
            if (value > NWScript.GetAbilityScore(player, Ability.Wisdom, true))
            {
              player.SendMessage("Vous n'avez pas la sagesse nécessaire pour retirer quoique ce soit de cet ouvrage");
              return Entrypoints.SCRIPT_HANDLED;
            }
          }

          if (int.TryParse(NWScript.Get2DAString("feat", "MINCHA", FeatId), out value))
          {
            if (value > NWScript.GetAbilityScore(player, Ability.Charisma, true))
            {
              player.SendMessage("Vous n'avez pas le charisme nécessaire pour retirer quoique ce soit de cet ouvrage");
              return Entrypoints.SCRIPT_HANDLED;
            }
          }

          if (int.TryParse(NWScript.Get2DAString("feat", "PREREQFEAT1", FeatId), out value))
          {
            if (!player.HasFeat((Feat)value))
            {
              player.SendMessage($"Le don {NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("feat", "FEAT", value)))} est nécessaire avant de pouvoir retirer quoique ce soit de cet ouvrage");
              return Entrypoints.SCRIPT_HANDLED;
            }
          }

          if (int.TryParse(NWScript.Get2DAString("feat", "PREREQFEAT2", FeatId), out value))
          {
            if (!player.HasFeat((Feat)value))
            {
              player.SendMessage($"Le don {NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("feat", "FEAT", value)))} est nécessaire avant de pouvoir retirer quoique ce soit de cet ouvrage");
              return Entrypoints.SCRIPT_HANDLED;
            }
          }

          if (int.TryParse(NWScript.Get2DAString("feat", "OrReqFeat0", FeatId), out value))
          {
            if (!player.HasFeat((Feat)value))
            {
              if (int.TryParse(NWScript.Get2DAString("feat", "OrReqFeat1", FeatId), out value))
              {
                if (!player.HasFeat((Feat)value))
                {
                  if (int.TryParse(NWScript.Get2DAString("feat", "OrReqFeat2", FeatId), out value))
                  {
                    if (!player.HasFeat((Feat)value))
                    {
                      if (int.TryParse(NWScript.Get2DAString("feat", "OrReqFeat3", FeatId), out value))
                      {
                        if (!player.HasFeat((Feat)value))
                        {
                          if (int.TryParse(NWScript.Get2DAString("feat", "OrReqFeat4", FeatId), out value))
                          {
                            if (!player.HasFeat((Feat)value))
                            {
                              player.SendMessage($"Il vous manque un don avant de pouvoir retirer un réel savoir de cet ouvrage");
                              return Entrypoints.SCRIPT_HANDLED;
                            }
                          }
                        }
                      }
                    }
                  }
                }
              }
            }
          }

          if (int.TryParse(NWScript.Get2DAString("feat", "REQSKILL", FeatId), out value))
          {
            if (!player.HasSkill((Skill)value))
            {
              player.SendMessage($"La compétence {NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("skills", "Name", value)))} est nécessaire avant de pouvoir retirer quoique ce soit de cet ouvrage");
              return Entrypoints.SCRIPT_HANDLED;
            }

            int SkillValueRequirement;
            if (int.TryParse(NWScript.Get2DAString("feat", "ReqSkillMinRanks", FeatId), out SkillValueRequirement))
            {
              if (SkillValueRequirement > player.GetSkillRank((Skill)value, true))
              {
                player.SendMessage($"Une minimum de {SkillValueRequirement} dans la compétence {NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("skills", "Name", value)))} est nécessaire avant de pouvoir retirer quoique ce soit de cet ouvrage");
                return Entrypoints.SCRIPT_HANDLED;
              }
            }  
          }

          if (int.TryParse(NWScript.Get2DAString("feat", "REQSKILL2", FeatId), out value))
          {
            if (!player.HasSkill((Skill)value))
            {
              player.SendMessage($"La compétence {NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("skills", "Name", value)))} est nécessaire avant de pouvoir retirer quoique ce soit de cet ouvrage");
              return Entrypoints.SCRIPT_HANDLED;
            }

            int SkillValueRequirement;
            if (int.TryParse(NWScript.Get2DAString("feat", "ReqSkillMinRanks2", FeatId), out SkillValueRequirement))
            {
              if (SkillValueRequirement > player.GetSkillRank((Skill)value, true))
              {
                player.SendMessage($"Une minimum de {SkillValueRequirement} dans la compétence {NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("skills", "Name", value)))} est nécessaire avant de pouvoir retirer quoique ce soit de cet ouvrage");
                return Entrypoints.SCRIPT_HANDLED;
              }
            }
          }

          if (int.TryParse(NWScript.Get2DAString("feat", "MinFortSave", FeatId), out value))
          {
            if (NWScript.GetFortitudeSavingThrow(player) < value)
            {
              player.SendMessage($"Il faut minimum {value} est nécessaire pour pouvoir retirer quoique ce soit de cet ouvrage");
              return Entrypoints.SCRIPT_HANDLED;
            }
          }

          player.LearnableSkills.Add(FeatId, new SkillSystem.Skill(FeatId, 0));
          FeatBook.Destroy();
        }
        else
          player.SendMessage("Vous connaissez déjà les bases d'entrainement de cette capacité");
      }

      return Entrypoints.SCRIPT_HANDLED;
    }
  }
}
