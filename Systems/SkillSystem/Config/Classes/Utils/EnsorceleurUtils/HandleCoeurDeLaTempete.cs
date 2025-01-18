using System.Collections.Generic;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class EnsoUtils
  {
    public static void HandleCoeurDeLaTempete(NwCreature creature, List<DamageType> damageTypeList)
    {
      if (creature.KnowsFeat((Feat)CustomSkill.EnsoCoeurDeLaTempete))
      {
        DamageType damageType = damageTypeList.Contains(DamageType.Electrical) ? DamageType.Electrical
          : damageTypeList.Contains(DamageType.Sonic) ? DamageType.Sonic : DamageType.BaseWeapon;

        if(damageType != DamageType.BaseWeapon)
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
}
