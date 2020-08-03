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

    public class Context
    {
      public NWItem oItem { get; }
      public PlayerSystem.Player oActivator { get; }
      public int SkillId { get; }

      public Context(NWItem oItem, PlayerSystem.Player oActivator, int SkillId)
      {
        this.oItem = oItem;
        this.oActivator = oActivator;
        this.SkillId = SkillId;
      }
    }

    private static Pipeline<Context> pipeline = new Pipeline<Context>(
      new Action<Context, Action>[]
      {
        ActivateItemHandlers.ProcessSkillBookCheckStatsMiddleware,
      }
    );
    private static int HandleMenuTesterActivate(uint oItem, uint oActivator)
    {
      Console.WriteLine($"You activated the item {NWScript.GetName(oItem)}! {NWScript.GetName(oActivator)}");

      return Entrypoints.SCRIPT_HANDLED;
    }

    private static void ProcessSkillBookCheckStatsMiddleware(ActivateItemHandlers.Context ctx, Action next)
    {
      if(!CheckPlayerStat("MINATTACKBONUS", ctx.SkillId, ctx.oActivator))
      {
        ctx.oActivator.SendMessage("Vous n'êtes pas assez expérimenté en maniement des armes pour retirer quoique ce soit de cet ouvrage");
        return;
      }

      if (!CheckPlayerStat("MINSTR", ctx.SkillId, ctx.oActivator))
      {
        ctx.oActivator.SendMessage("Vous n'avez pas la force nécessaire pour retirer quoique ce soit de cet ouvrage");
        return;
      }

      if (!CheckPlayerStat("MINDEX", ctx.SkillId, ctx.oActivator))
      {
        ctx.oActivator.SendMessage("Vous n'avez pas la dextérité nécessaire pour retirer quoique ce soit de cet ouvrage");
        return;
      }

      if (!CheckPlayerStat("MINCON", ctx.SkillId, ctx.oActivator))
      {
        ctx.oActivator.SendMessage("Vous n'avez pas la constitution nécessaire pour retirer quoique ce soit de cet ouvrage");
        return;
      }

      if (!CheckPlayerStat("MININT", ctx.SkillId, ctx.oActivator))
      {
        ctx.oActivator.SendMessage("Vous n'avez pas l'intelligence nécessaire pour retirer quoique ce soit de cet ouvrage");
        return;
      }

      if (!CheckPlayerStat("MINWIS", ctx.SkillId, ctx.oActivator))
      {
        ctx.oActivator.SendMessage("Vous n'avez pas la sagesse nécessaire pour retirer quoique ce soit de cet ouvrage");
        return;
      }

      if (!CheckPlayerStat("MINCHA", ctx.SkillId, ctx.oActivator))
      {
        ctx.oActivator.SendMessage("Vous n'avez pas le charisme nécessaire pour retirer quoique ce soit de cet ouvrage");
        return;
      }

      next();
    }

    private static Boolean CheckPlayerStat(string Stat, int SkillId, PlayerSystem.Player player)
    {
      int value;
      if (int.TryParse(NWScript.Get2DAString("feat", Stat, SkillId), out value))
      {
        if (value < NWScript.GetBaseAttackBonus(player))
        {
          return true;
        }
      }

      return false;
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
          pipeline.Execute(new Context(
          oItem: FeatBook,
          oActivator: player,
          SkillId: FeatId
        ));

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
