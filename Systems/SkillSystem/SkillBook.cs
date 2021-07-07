using System;
using NWN.API;
using NWN.API.Constants;
using NWN.Core;
using NWN.Core.NWNX;
using Action = System.Action;

namespace NWN.Systems
{
  static public class SkillBook
  {
    public class Context
    {
      public NwItem oItem { get; }
      public PlayerSystem.Player oActivator { get; }
      public Feat skillId { get; }

      public Context(NwItem oItem, PlayerSystem.Player oActivator, Feat SkillId)
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
        ctx.oActivator.oid.SendServerMessage("Vous n'êtes pas assez expérimenté en maniement des armes pour retirer quoique ce soit de cet ouvrage");
        return;
      }

      if (!CheckPlayerRequiredStat("MINSTR", ctx.skillId, ctx.oActivator))
      {
        ctx.oActivator.oid.SendServerMessage("Vous n'avez pas la force nécessaire pour retirer quoique ce soit de cet ouvrage");
        return;
      }

      if (!CheckPlayerRequiredStat("MINDEX", ctx.skillId, ctx.oActivator))
      {
        ctx.oActivator.oid.SendServerMessage("Vous n'avez pas la dextérité nécessaire pour retirer quoique ce soit de cet ouvrage");
        return;
      }

      if (!CheckPlayerRequiredStat("MINCON", ctx.skillId, ctx.oActivator))
      {
        ctx.oActivator.oid.SendServerMessage("Vous n'avez pas la constitution nécessaire pour retirer quoique ce soit de cet ouvrage");
        return;
      }

      if (!CheckPlayerRequiredStat("MININT", ctx.skillId, ctx.oActivator))
      {
        ctx.oActivator.oid.SendServerMessage("Vous n'avez pas l'intelligence nécessaire pour retirer quoique ce soit de cet ouvrage");
        return;
      }

      if (!CheckPlayerRequiredStat("MINWIS", ctx.skillId, ctx.oActivator))
      {
        ctx.oActivator.oid.SendServerMessage("Vous n'avez pas la sagesse nécessaire pour retirer quoique ce soit de cet ouvrage");
        return;
      }

      if (!CheckPlayerRequiredStat("MINCHA", ctx.skillId, ctx.oActivator))
      {
        ctx.oActivator.oid.SendServerMessage("Vous n'avez pas le charisme nécessaire pour retirer quoique ce soit de cet ouvrage");
        return;
      }

      next();
    }

    private static void CheckRequiredFeatsMiddleware(Context ctx, Action next)
    {
      if (!Feat2da.featTable.HasFeatPrerequisites(ctx.skillId, ctx.oActivator.oid.LoginCreature))
        return;

      next();
    }
    private static void CheckRequiredSkillsMiddleware(Context ctx, Action next)
    {
      (Boolean success, int skillId) = CheckPlayerRequiredSkill("REQSKILL", "ReqSkillMinRanks", ctx.skillId, ctx.oActivator);
      if (!success)
      {
        ctx.oActivator.oid.SendServerMessage($"Une maîtrise plus avancée de la compétence {NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("skills", "Name", skillId)))} est nécessaire avant de pouvoir retirer quoique ce soit de cet ouvrage");
        return;
      }

      (success, skillId) = CheckPlayerRequiredSkill("REQSKILL2", "ReqSkillMinRanks2", ctx.skillId, ctx.oActivator);
      if (!success)
      {
        ctx.oActivator.oid.SendServerMessage($"Une maîtrise plus avancée de la compétence {NWScript.GetStringByStrRef(int.Parse(NWScript.Get2DAString("skills", "Name", skillId)))} est nécessaire avant de pouvoir retirer quoique ce soit de cet ouvrage");
        return;
      }

      int result;
      if (int.TryParse(NWScript.Get2DAString("feat", "MinFortSave", (int)ctx.skillId), out result))
      {
        if (ctx.oActivator.oid.LoginCreature.GetBaseSavingThrow(SavingThrow.Fortitude) < result)
        {
          ctx.oActivator.oid.SendServerMessage($"Une vigueur minimale de {result} est nécessaire pour pouvoir retirer quoique ce soit de cet ouvrage");
          return;
        }
      }

      next();
    }

    private static void ValidationMiddleware(Context ctx, Action next)
    {
      ctx.oActivator.learnableSkills.Add(ctx.skillId, new SkillSystem.Skill(ctx.skillId, 0, ctx.oActivator));
      ctx.oItem.Destroy();

      next();
    }

    private static Boolean CheckPlayerRequiredStat(string Stat, Feat SkillId, PlayerSystem.Player player)
    {
      int value;

      if (int.TryParse(NWScript.Get2DAString("feat", Stat, (int)SkillId), out value))
      {
        switch (Stat)
        {
          case "MINATTACKBONUS":
            if (value > player.oid.LoginCreature.BaseAttackBonus)
              return false;
            break;
          case "MINSTR":
            if (value > player.oid.LoginCreature.GetAbilityScore(Ability.Strength, true))
              return false;
            break;
          case "MINCON":
            if (value > player.oid.LoginCreature.GetAbilityScore(Ability.Constitution, true))
              return false;
            break;
          case "MINDEX":
            if (value > player.oid.LoginCreature.GetAbilityScore(Ability.Dexterity, true))
              return false;
            break;
          case "MININT":
            if (value > player.oid.LoginCreature.GetAbilityScore(Ability.Intelligence, true))
              return false;
            break;
          case "MINWIS":
            if (value > player.oid.LoginCreature.GetAbilityScore(Ability.Wisdom, true))
              return false;
            break;
          case "MINCHA":
            if (value > player.oid.LoginCreature.GetAbilityScore(Ability.Charisma, true))
              return false;
            break;
        }

      }

      return true;
    }
    private static (Boolean success, int skillId) CheckPlayerRequiredSkill(string Skill, string SkillRank, Feat SkillId, PlayerSystem.Player player)
    {
      int value;
      int SkillValueRequirement;
      if (int.TryParse(NWScript.Get2DAString("feat", Skill, (int)SkillId), out value))
        if (int.TryParse(NWScript.Get2DAString("feat", SkillRank, (int)SkillId), out SkillValueRequirement))
          if (SkillValueRequirement > player.oid.LoginCreature.GetSkillRank((Skill)value, true))
            return (false, value);

      return (true, value);
    }
  }
}
