using NWN.API;
using NWN.API.Events;
using NWN.Core;
using System.Linq;

namespace NWN.Systems
{
  class RaiseDead
  {
    public RaiseDead(SpellEvents.OnSpellCast onSpellCast)
    {
      NwPlayer oCaster = (NwPlayer)onSpellCast.Caster;

      if (onSpellCast.TargetObject.Tag != "pccorpse")
        return;

      int PcId = onSpellCast.TargetObject.GetLocalVariable<int>("_PC_ID").Value;
      
      PlayerSystem.Player oPC = PlayerSystem.Players.Where(p => p.Value.characterId == PcId).FirstOrDefault().Value;

      if (oPC != null && NwModule.Instance.Players.Any(p => p == oPC.oid))
      {
        oPC.oid.Location = onSpellCast.TargetObject.Location;
        onSpellCast.TargetObject.ApplyEffect(EffectDuration.Instant, API.Effect.VisualEffect(API.Constants.VfxType.ImpRaiseDead));

        if(onSpellCast.Spell == API.Constants.Spell.RaiseDead)
          oPC.oid.HP = 1;
      }
      else
      {
        oCaster.SendServerMessage("Votre sort a bien eu l'effet escompté, cependant l'individu blessé semble encore avoir besoin de repos. Il faudra un certain temps avant de le voir se relever.");

        var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"UPDATE playerCharacters SET areaTag = @areaTag, position = @position WHERE characterId = @characterId");
        NWScript.SqlBindInt(query, "@characterId", PcId);
        NWScript.SqlBindString(query, "@areaTag", onSpellCast.TargetObject.Area.Tag);
        NWScript.SqlBindVector(query, "@position", onSpellCast.TargetObject.Position);
        NWScript.SqlStep(query);
      }

      ((NwPlaceable)onSpellCast.TargetObject).Inventory.Items.FirstOrDefault(c => c.Tag == "item_pccorpse").Destroy();
      onSpellCast.TargetObject.Destroy();

      PlayerSystem.DeletePlayerCorpseFromDatabase(PcId);

      NWScript.SignalEvent(onSpellCast.TargetObject, NWScript.EventSpellCastAt(oCaster, NWScript.SPELL_RAISE_DEAD, 0));
      onSpellCast.TargetObject.Location.ApplyEffect(EffectDuration.Instant, API.Effect.VisualEffect(API.Constants.VfxType.ImpRaiseDead));
    }
  }
}
