using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void MonkStunStrike(NwCreature caster)
    {
      NwItem mainHandWeapon = caster.GetItemInSlot(InventorySlot.RightHand);
      
      if(mainHandWeapon is not null && mainHandWeapon.IsRangedWeapon)
      {
        caster.LoginPlayer?.SendServerMessage("Vous devez être à main nue ou équipé d'une arme de mêlée", ColorConstants.Red);
        return;
      }

      var cdEff = caster.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.MonkStunStrike);

      if(cdEff is not null)
      {
        caster.LoginPlayer?.SendServerMessage($"{"Frappe étourdissante".ColorString(ColorConstants.Cyan)} - Rechargement - {cdEff.DurationRemaining.ToString().ColorString(ColorConstants.White)} s", ColorConstants.Orange);
        return;
      }

      caster.OnCreatureAttack -= CreatureUtils.OnMonkStunStrike;
      caster.OnCreatureAttack += CreatureUtils.OnMonkStunStrike;

      FeatUtils.DecrementKi(caster);
    }
  }
}
