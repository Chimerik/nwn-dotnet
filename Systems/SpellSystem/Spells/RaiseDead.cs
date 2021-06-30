using NWN.API;
using NWN.API.Constants;
using NWN.API.Events;
using NWN.Core;
using NWN.Core.NWNX;
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

      int PcId = onSpellCast.TargetObject.GetLocalVariable<int>("_PC_ID").Value;
      NwPlayer oPC = NwModule.Instance.Players.FirstOrDefault(p => ObjectPlugin.GetInt(p.LoginCreature, "characterId") == PcId);

      if (oPC != null)
      {
        oPC.LoginCreature.Location = onSpellCast.TargetObject.Location;
        onSpellCast.TargetObject.ApplyEffect(EffectDuration.Instant, API.Effect.VisualEffect(API.Constants.VfxType.ImpRaiseDead));

        if(onSpellCast.Spell == API.Constants.Spell.RaiseDead)
          oPC.LoginCreature.HP = 1;
      }
      else
      {
        oCaster.ControllingPlayer.SendServerMessage("Votre sort a bien eu l'effet escompté, cependant l'individu blessé semble encore avoir besoin de repos. Il faudra un certain temps avant de le voir se relever.");

        SqLiteUtils.UpdateQuery("playerCharacters",
          new Dictionary<string, string>() { { "areaTag", onSpellCast.TargetObject.Area.Tag }, { "position", onSpellCast.TargetObject.Position.ToString() } },
          new Dictionary<string, string>() { { "rowid", PcId.ToString() } });
      }

      ((NwPlaceable)onSpellCast.TargetObject).Inventory.Items.FirstOrDefault(c => c.Tag == "item_pccorpse").Destroy();
      onSpellCast.TargetObject.Destroy();

      PlayerSystem.DeletePlayerCorpseFromDatabase(PcId);

      NWScript.SignalEvent(onSpellCast.TargetObject, NWScript.EventSpellCastAt(oCaster, NWScript.SPELL_RAISE_DEAD, 0));
      onSpellCast.TargetObject.Location.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpRaiseDead));
    }
  }
}
