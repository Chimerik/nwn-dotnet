using Anvil.API;
using Anvil.API.Events;
using System.Collections.Generic;
using System.Linq;

namespace NWN.Systems
{
  class RaiseDead
  {
    public RaiseDead(SpellEvents.OnSpellCast onSpellCast)
    {
      if (!(onSpellCast.Caster is NwCreature { IsPlayerControlled: true } oCaster) || onSpellCast.TargetObject.Tag != "pccorpse")
        return;

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, oCaster, onSpellCast.Spell.SpellType, false);

      int PcId = onSpellCast.TargetObject.GetObjectVariable<LocalVariableInt>("_PC_ID").Value;
      NwPlayer oPC = NwModule.Instance.Players.FirstOrDefault(p => p.LoginCreature.GetObjectVariable<PersistentVariableInt>("characterId").Value == PcId);

      if (oPC != null)
      {
        oPC.LoginCreature.Location = onSpellCast.TargetObject.Location;
        onSpellCast.TargetObject.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpRaiseDead));

        if(onSpellCast.Spell.SpellType == Spell.RaiseDead)
          oPC.LoginCreature.HP = 1;
      }
      else
      {
        oCaster.ControllingPlayer.SendServerMessage("Votre sort a bien eu l'effet escompté, cependant l'individu blessé semble encore avoir besoin de repos. Il faudra un certain temps avant de le voir se relever.");

        SqLiteUtils.UpdateQuery("playerCharacters",
          new List<string[]>() { new string[] { "areaTag", onSpellCast.TargetObject.Area.Tag }, new string[] { "position", onSpellCast.TargetObject.Position.ToString() } },
          new List<string[]>() { new string[] { "rowid", PcId.ToString() } });
      }

      ((NwPlaceable)onSpellCast.TargetObject).Inventory.Items.FirstOrDefault(c => c.Tag == "item_pccorpse").Destroy();
      onSpellCast.TargetObject.Destroy();

      PlayerSystem.DeletePlayerCorpseFromDatabase(PcId);

      SpellUtils.SignalEventSpellCast(onSpellCast.TargetObject, oCaster, onSpellCast.Spell.SpellType, false);
      onSpellCast.TargetObject.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpRaiseDead));
    }
  }
}
