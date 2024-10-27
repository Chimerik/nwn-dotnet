using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static bool CanCastSpell(NwCreature caster, NwGameObject target, NwSpell spell, SpellEntry spellEntry)
    {
      if (spell.InnateSpellLevel > 9)
        return true;

      NwItem offHand = caster.GetItemInSlot(InventorySlot.LeftHand);
      NwItem rightHand = caster.GetItemInSlot(InventorySlot.RightHand);

      if (Spells2da.spellTable.GetRow(spell.Id).requiresSomatic && !caster.KnowsFeat((Feat)CustomSkill.MageDeGuerre)
        && rightHand is not null && (offHand is not null || ItemUtils.IsTwoHandedWeapon(rightHand.BaseItem, caster.Size)))
      {
        caster.LoginPlayer?.SendServerMessage("Vous ne pouvez pas lancer de sort nécessitant une composante somatique en ayant vos deux mains occupées", ColorConstants.Red);
        return false;
      }

      foreach (var eff in caster.ActiveEffects)
        switch (eff.Tag)
        {
          case EffectSystem.ShieldArmorDisadvantageEffectTag:
            caster?.ControllingPlayer.SendServerMessage("Vous ne pouvez pas lancer de sort en étant équipé d'une armure ou d'un bouclier que vous ne maîtrisez pas", ColorConstants.Red);
            return false;
          case EffectSystem.BarbarianRageEffectTag:
            caster?.ControllingPlayer.SendServerMessage("Vous ne pouvez pas lancer de sort sous l'effet de la rage", ColorConstants.Red);
            return false;
        }

      if (target is not null
        && (caster.ActiveEffects.Any(e => Utils.In(e.EffectType, EffectType.Blindness, EffectType.Darkness))
        || target.ActiveEffects.Any(e => e.EffectType == EffectType.Darkness))
        && target.DistanceSquared(caster) < 9)
      {
        caster.LoginPlayer?.SendServerMessage($"Vous devez vous situer à moins de {"3m".ColorString(ColorConstants.White)} de cette cible pour l'atteindre", ColorConstants.Red);
        return false;
      }      

      return true;
    }
  }
}
