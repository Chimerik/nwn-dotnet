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
        ActivateItemHandlers.ProcessSkillBookCheckPrerequisteStatsMiddleware,
        ActivateItemHandlers.ProcessSkillBookCheckPrerequesiteFeatsMiddleware,
        ActivateItemHandlers.ProcessSkillBookCheckPrerequesiteSkillsMiddleware,
        ActivateItemHandlers.ProcessSkillBookValidationMiddleware,
      }
    );
    private static int HandleMenuTesterActivate(uint oItem, uint oActivator)
    {
      Console.WriteLine($"You activated the item {NWScript.GetName(oItem)}! {NWScript.GetName(oActivator)}");

      return Entrypoints.SCRIPT_HANDLED;
    }

    private static void ProcessSkillBookCheckPrerequisteStatsMiddleware(ActivateItemHandlers.Context ctx, Action next)
    {
      if(!CheckPlayerPrerequesiteStat("MINATTACKBONUS", ctx.SkillId, ctx.oActivator))
      {
        ctx.oActivator.SendMessage("Vous n'êtes pas assez expérimenté en maniement des armes pour retirer quoique ce soit de cet ouvrage");
        return;
      }

      if (!CheckPlayerPrerequesiteStat("MINSTR", ctx.SkillId, ctx.oActivator))
      {
        ctx.oActivator.SendMessage("Vous n'avez pas la force nécessaire pour retirer quoique ce soit de cet ouvrage");
        return;
      }

      if (!CheckPlayerPrerequesiteStat("MINDEX", ctx.SkillId, ctx.oActivator))
      {
        ctx.oActivator.SendMessage("Vous n'avez pas la dextérité nécessaire pour retirer quoique ce soit de cet ouvrage");
        return;
      }

      if (!CheckPlayerPrerequesiteStat("MINCON", ctx.SkillId, ctx.oActivator))
      {
        ctx.oActivator.SendMessage("Vous n'avez pas la constitution nécessaire pour retirer quoique ce soit de cet ouvrage");
        return;
      }

      if (!CheckPlayerPrerequesiteStat("MININT", ctx.SkillId, ctx.oActivator))
      {
        ctx.oActivator.SendMessage("Vous n'avez pas l'intelligence nécessaire pour retirer quoique ce soit de cet ouvrage");
        return;
      }

      if (!CheckPlayerPrerequesiteStat("MINWIS", ctx.SkillId, ctx.oActivator))
      {
        ctx.oActivator.SendMessage("Vous n'avez pas la sagesse nécessaire pour retirer quoique ce soit de cet ouvrage");
        return;
      }

      if (!CheckPlayerPrerequesiteStat("MINCHA", ctx.SkillId, ctx.oActivator))
      {
        ctx.oActivator.SendMessage("Vous n'avez pas le charisme nécessaire pour retirer quoique ce soit de cet ouvrage");
        return;
      }

      next();
    }

    private static void ProcessSkillBookCheckPrerequesiteFeatsMiddleware(ActivateItemHandlers.Context ctx, Action next)
    {
      int result = CheckPlayerPrerequesiteFeat("PREREQFEAT1", ctx.SkillId, ctx.oActivator);
      if (result > -1)
      {
        ctx.oActivator.SendMessage($"Le don {NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("feat", "FEAT", result)))} est nécessaire avant de pouvoir retirer quoique ce soit de cet ouvrage");
        return;
      }

      result = CheckPlayerPrerequesiteFeat("PREREQFEAT2", ctx.SkillId, ctx.oActivator);
      if (result > -1)
      {
        ctx.oActivator.SendMessage($"Le don {NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("feat", "FEAT", result)))} est nécessaire avant de pouvoir retirer quoique ce soit de cet ouvrage");
        return;
      }

      result = CheckPlayerPrerequesiteFeat("OrReqFeat0", ctx.SkillId, ctx.oActivator);
      if (result > -1)
      {
        result = CheckPlayerPrerequesiteFeat("OrReqFeat1", ctx.SkillId, ctx.oActivator);
        if (result > -1)
        {
          result = CheckPlayerPrerequesiteFeat("OrReqFeat2", ctx.SkillId, ctx.oActivator);
          if (result > -1)
          {
            result = CheckPlayerPrerequesiteFeat("OrReqFeat3", ctx.SkillId, ctx.oActivator);
            if (result > -1)
            {
              result = CheckPlayerPrerequesiteFeat("OrReqFeat4", ctx.SkillId, ctx.oActivator);
              if (result > -1)
              {
                ctx.oActivator.SendMessage($"Il vous manque un don avant de pouvoir retirer un réel savoir de cet ouvrage");
                return;
              }
            }
          }
        } 
      }

      next();
    }
    private static void ProcessSkillBookCheckPrerequesiteSkillsMiddleware(ActivateItemHandlers.Context ctx, Action next)
    {
      int result = CheckPlayerPrerequesiteSkill("REQSKILL", "ReqSkillMinRanks", ctx.SkillId, ctx.oActivator);
      if (result > -1)
      {
        ctx.oActivator.SendMessage($"Une maîtrise plus avancée de la compétence {NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("skills", "Name", result)))} est nécessaire avant de pouvoir retirer quoique ce soit de cet ouvrage");
        return;
      }

      result = CheckPlayerPrerequesiteSkill("REQSKILL2", "ReqSkillMinRanks2", ctx.SkillId, ctx.oActivator);
      if (result > -1)
      {
        ctx.oActivator.SendMessage($"Une maîtrise plus avancée de la compétence {NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("skills", "Name", result)))} est nécessaire avant de pouvoir retirer quoique ce soit de cet ouvrage");
        return;
      }

      if (int.TryParse(NWScript.Get2DAString("feat", "MinFortSave", ctx.SkillId), out result))
      {
        if (NWScript.GetFortitudeSavingThrow(ctx.oActivator) < result)
        {
          ctx.oActivator.SendMessage($"Une vigueur minimale de {result} est nécessaire pour pouvoir retirer quoique ce soit de cet ouvrage");
          return;
        }
      }

      next();
    }

    private static void ProcessSkillBookValidationMiddleware(ActivateItemHandlers.Context ctx, Action next)
    {
      ctx.oActivator.LearnableSkills.Add(ctx.SkillId, new SkillSystem.Skill(ctx.SkillId, 0));
      ctx.oItem.Destroy();

      next();
    }

    private static Boolean CheckPlayerPrerequesiteStat(string Stat, int SkillId, PlayerSystem.Player player)
    {
      int value;
      if (int.TryParse(NWScript.Get2DAString("feat", Stat, SkillId), out value))
        if (value < NWScript.GetBaseAttackBonus(player))
          return true;

      return false;
    }

    private static int CheckPlayerPrerequesiteFeat(string Feat, int SkillId, PlayerSystem.Player player)
    {
      int value;
      if (int.TryParse(NWScript.Get2DAString("feat", Feat, SkillId), out value))
        if (player.HasFeat((Feat)value))
          return -1;

      return value;
    }

    private static int CheckPlayerPrerequesiteSkill(string Skill, string SkillRank, int SkillId, PlayerSystem.Player player)
    {
      int value;
      int SkillValueRequirement;
      if (int.TryParse(NWScript.Get2DAString("feat", Skill, SkillId), out value))
        if (int.TryParse(NWScript.Get2DAString("feat", SkillRank, SkillId), out SkillValueRequirement))
          if (SkillValueRequirement < player.GetSkillRank((Skill)value, true))
            return value;

      return -1;
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
        }
        else
          player.SendMessage("Vous connaissez déjà les bases d'entrainement de cette capacité");
      }

      return Entrypoints.SCRIPT_HANDLED;
    }
  }
}
