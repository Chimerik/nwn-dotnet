using System;
using NWN.Core;
using NWN.Core.NWNX;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  static public class SkillBook
  {
    public class Context
    {
      public uint oItem { get; }
      public PlayerSystem.Player oActivator { get; }
      public int skillId { get; }

      public Context(uint oItem, PlayerSystem.Player oActivator, int SkillId)
      {
        this.oItem = oItem;
        this.oActivator = oActivator;
        this.skillId = SkillId;
      }
    }

    public static Pipeline<Context> pipeline = new Pipeline<Context>(
      new Action<Context, Action>[]
      {
        CheckRequiredStatsMiddleware,
        CheckRequiredFeatsMiddleware,
        CheckRequiredSkillsMiddleware,
        ValidationMiddleware,
      }
    );

    private static void CheckRequiredStatsMiddleware(Context ctx, Action next)
    {
      if (!CheckPlayerRequiredStat("MINATTACKBONUS", ctx.skillId, ctx.oActivator))
      { 
        NWScript.SendMessageToPC(ctx.oActivator.oid, "Vous n'êtes pas assez expérimenté en maniement des armes pour retirer quoique ce soit de cet ouvrage");
        return;
      }

      if (!CheckPlayerRequiredStat("MINSTR", ctx.skillId, ctx.oActivator))
      {
        NWScript.SendMessageToPC(ctx.oActivator.oid, "Vous n'avez pas la force nécessaire pour retirer quoique ce soit de cet ouvrage");
        return;
      }

      if (!CheckPlayerRequiredStat("MINDEX", ctx.skillId, ctx.oActivator))
      {
        NWScript.SendMessageToPC(ctx.oActivator.oid, "Vous n'avez pas la dextérité nécessaire pour retirer quoique ce soit de cet ouvrage");
        return;
      }

      if (!CheckPlayerRequiredStat("MINCON", ctx.skillId, ctx.oActivator))
      {
        NWScript.SendMessageToPC(ctx.oActivator.oid,"Vous n'avez pas la constitution nécessaire pour retirer quoique ce soit de cet ouvrage");
        return;
      }

      if (!CheckPlayerRequiredStat("MININT", ctx.skillId, ctx.oActivator))
      {
        NWScript.SendMessageToPC(ctx.oActivator.oid,"Vous n'avez pas l'intelligence nécessaire pour retirer quoique ce soit de cet ouvrage");
        return;
      }

      if (!CheckPlayerRequiredStat("MINWIS", ctx.skillId, ctx.oActivator))
      {
        NWScript.SendMessageToPC(ctx.oActivator.oid,"Vous n'avez pas la sagesse nécessaire pour retirer quoique ce soit de cet ouvrage");
        return;
      }

      if (!CheckPlayerRequiredStat("MINCHA", ctx.skillId, ctx.oActivator))
      {
        NWScript.SendMessageToPC(ctx.oActivator.oid,"Vous n'avez pas le charisme nécessaire pour retirer quoique ce soit de cet ouvrage");
        return;
      }

      next();
    }

    private static void CheckRequiredFeatsMiddleware(Context ctx, Action next)
    {
      (Boolean success, int featId) = CheckPlayerRequiredFeat("PREREQFEAT1", ctx.skillId, ctx.oActivator);
      if (!success)
      {
        NWScript.SendMessageToPC(ctx.oActivator.oid,$"Le don {NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("feat", "FEAT", featId)))} est nécessaire avant de pouvoir retirer quoique ce soit de cet ouvrage");
        return;
      }

      (success, featId) = CheckPlayerRequiredFeat("PREREQFEAT2", ctx.skillId, ctx.oActivator);
      if (!success)
      {
        NWScript.SendMessageToPC(ctx.oActivator.oid,$"Le don {NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("feat", "FEAT", featId)))} est nécessaire avant de pouvoir retirer quoique ce soit de cet ouvrage");
        return;
      }

      if (!CheckPlayerRequiredFeat("OrReqFeat0", ctx.skillId, ctx.oActivator).success &&
          !CheckPlayerRequiredFeat("OrReqFeat1", ctx.skillId, ctx.oActivator).success &&
          !CheckPlayerRequiredFeat("OrReqFeat2", ctx.skillId, ctx.oActivator).success &&
          !CheckPlayerRequiredFeat("OrReqFeat3", ctx.skillId, ctx.oActivator).success &&
          !CheckPlayerRequiredFeat("OrReqFeat4", ctx.skillId, ctx.oActivator).success)
      {
        NWScript.SendMessageToPC(ctx.oActivator.oid,$"Il vous manque un don avant de pouvoir retirer un réel savoir de cet ouvrage");
        return;
      }

      next();
    }
    private static void CheckRequiredSkillsMiddleware(Context ctx, Action next)
    {
      (Boolean success, int skillId) = CheckPlayerRequiredSkill("REQSKILL", "ReqSkillMinRanks", ctx.skillId, ctx.oActivator);
      if (!success)
      {
        NWScript.SendMessageToPC(ctx.oActivator.oid,$"Une maîtrise plus avancée de la compétence {NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("skills", "Name", skillId)))} est nécessaire avant de pouvoir retirer quoique ce soit de cet ouvrage");
        return;
      }

      (success, skillId) = CheckPlayerRequiredSkill("REQSKILL2", "ReqSkillMinRanks2", ctx.skillId, ctx.oActivator);
      if (!success)
      {
        NWScript.SendMessageToPC(ctx.oActivator.oid,$"Une maîtrise plus avancée de la compétence {NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("skills", "Name", skillId)))} est nécessaire avant de pouvoir retirer quoique ce soit de cet ouvrage");
        return;
      }

      int result;
      if (int.TryParse(NWScript.Get2DAString("feat", "MinFortSave", ctx.skillId), out result))
      {
        if (NWScript.GetFortitudeSavingThrow(ctx.oActivator.oid) < result)
        {
          NWScript.SendMessageToPC(ctx.oActivator.oid,$"Une vigueur minimale de {result} est nécessaire pour pouvoir retirer quoique ce soit de cet ouvrage");
          return;
        }
      }

      next();
    }

    private static void ValidationMiddleware(Context ctx, Action next)
    {
      ctx.oActivator.learnableSkills.Add(ctx.skillId, new SkillSystem.Skill(ctx.skillId, 0, ctx.oActivator));
      NWScript.DestroyObject(ctx.oItem);

      next();
    }

    private static Boolean CheckPlayerRequiredStat(string Stat, int SkillId, PlayerSystem.Player player)
    {
      int value;
      if (int.TryParse(NWScript.Get2DAString("feat", Stat, SkillId), out value))
        if (value < NWScript.GetBaseAttackBonus(player.oid))
          return true;

      return false;
    }

    private static (Boolean success, int featId) CheckPlayerRequiredFeat(string Feat, int SkillId, PlayerSystem.Player player)
    {
      int value;
      if (int.TryParse(NWScript.Get2DAString("feat", Feat, SkillId), out value))
        if (CreaturePlugin.GetKnowsFeat(player.oid, value) == 1)
          return (true, value);

      return (false, value);
    }

    private static (Boolean success, int skillId) CheckPlayerRequiredSkill(string Skill, string SkillRank, int SkillId, PlayerSystem.Player player)
    {
      int value;
      int SkillValueRequirement;
      if (int.TryParse(NWScript.Get2DAString("feat", Skill, SkillId), out value))
        if (int.TryParse(NWScript.Get2DAString("feat", SkillRank, SkillId), out SkillValueRequirement))
          if (SkillValueRequirement < NWScript.GetSkillRank(value, player.oid))
            return (true, value);

      return (false, value);
    }
  }
}
