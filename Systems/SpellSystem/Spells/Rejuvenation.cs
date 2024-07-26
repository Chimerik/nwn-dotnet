using Anvil.API;
using System.Collections.Generic;
using System.Linq;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Rejuvenation(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject target)
    {
      if (target.Tag != "pccorpse" || oCaster is not NwCreature caster)
        return;

      if(caster.Gold < 300)
      {
        caster.LoginPlayer?.SendServerMessage("Vous devez être en possession de 300 po pour lancer ce sort");
        return;
      }

      caster.Gold -= 300;

      SpellUtils.SignalEventSpellCast(target, oCaster, spell.SpellType, false);

      int PcId = target.GetObjectVariable<LocalVariableInt>("_PC_ID").Value;
      NwPlayer oPC = NwModule.Instance.Players.FirstOrDefault(p => p.LoginCreature != null && p.LoginCreature.GetObjectVariable<PersistentVariableInt>("characterId").Value == PcId);

      if (oPC != null)
      {
        oPC.LoginCreature.Location = target.Location;
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpRaiseDead));

        if(spell.SpellType == Spell.RaiseDead)
          oPC.LoginCreature.HP = 1;
      }
      else
      {
        caster.LoginPlayer?.SendServerMessage("Votre sort a bien eu l'effet escompté, cependant l'individu blessé semble encore avoir besoin de repos. Il faudra un certain temps avant de le voir se relever.");

        SqLiteUtils.UpdateQuery("playerCharacters",
          new List<string[]>() { new string[] { "areaTag", target.Area.Tag }, new string[] { "position", target.Position.ToString() } },
          new List<string[]>() { new string[] { "rowid", PcId.ToString() } });
      }

      ((NwPlaceable)target).Inventory.Items.FirstOrDefault(c => c.Tag == "item_pccorpse").Destroy();
      target.Destroy();

      PlayerSystem.DeletePlayerCorpseFromDatabase(PcId);
      target.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpRaiseDead));
    }
  }
}
