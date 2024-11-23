using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class RangerUtils
  {
    public static void OnDamageVolee(OnCreatureDamage onDamage)
    {
      if (onDamage.Target is not NwCreature damaged || onDamage.DamagedBy is not NwCreature damager
        || !damaged.ActiveEffects.Any(e => e.Tag == EffectSystem.MarqueDuChasseurTag && e.Creator == damager))
        return; 

      foreach(var target in damaged.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 9, true))
      {
        if(target == damager || !damager.IsReactionTypeHostile(target)) 
          continue;
        
        int bonus = NwRandom.Roll(Utils.random, damager.KnowsFeat((Feat)CustomSkill.RangerPourfendeur) ? 10 : 6);
        NwItem weapon = damager.GetItemInSlot(InventorySlot.RightHand);
        DamageType damageType = weapon is null || !ItemUtils.IsWeapon(weapon.BaseItem) ? DamageType.Bludgeoning : weapon.BaseItem.WeaponType.FirstOrDefault();


        NWScript.AssignCommand(damager, () => target.ApplyEffect(EffectDuration.Instant, Effect.Damage(NwRandom.Roll(Utils.random, bonus), damageType)));
        
        break;
      }
    }
  }
}
