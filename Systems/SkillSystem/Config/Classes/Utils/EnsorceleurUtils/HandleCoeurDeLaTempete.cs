using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class EnsoUtils
  {
    public static void HandleCoeurDeLaTempete(NwCreature creature, DamageType damageType)
    {
      if (creature is not null && creature.KnowsFeat((Feat)CustomSkill.EnsoCoeurDeLaTempete) && Utils.In(damageType, DamageType.Electrical, DamageType.Sonic))
      {
        int damage = creature.GetClassInfo(ClassType.Sorcerer).Level / 2;

        foreach (var target in creature.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 6, false))
        {
          if (creature.IsReactionTypeHostile(target) && creature.IsCreatureSeen(target))
            NWScript.AssignCommand(creature, () => target.ApplyEffect(EffectDuration.Instant, Effect.Damage(damage, damageType)));
        }
      } 
    }
  }
}
