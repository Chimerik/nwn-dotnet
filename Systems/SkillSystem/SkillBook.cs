using System;
using Anvil.API;
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
      if (!Feat2da.featTable.HasAbilityPrerequisites(ctx.skillId, ctx.oActivator.oid.LoginCreature))
        return;

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
      if (!Feat2da.featTable.HasSkillPrerequisites(ctx.skillId, ctx.oActivator.oid.LoginCreature))
        return;

      next();
    }

    private static void ValidationMiddleware(Context ctx, Action next)
    {
      /*ctx.oActivator.learnables.Add($"F{ctx.skillId}", new Learnable(LearnableType.Feat, (int)ctx.skillId, 0).InitializeLearnableLevel(ctx.oActivator));
      ctx.oItem.Destroy();*/

      next();
    }
  }
}
