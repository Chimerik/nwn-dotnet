using NWN.API;
using NWN.Core;
using NWN.Services;
using System.Linq;

namespace NWN.Systems
{
    public partial class SpellSystem
    {
        [ScriptHandler("nw_s0_raisdead")]
        private void HandleRaiseDead(CallInfo callInfo)
        {
            NwPlaceable oTarget = NWScript.GetSpellTargetObject().ToNwObject<NwPlaceable>();

            if (oTarget.Tag != "pccorpse")
                return;

            int PcId = oTarget.GetLocalVariable<int>("_PC_ID").Value;
            PlayerSystem.Player oPC = PlayerSystem.Players.Where(p => p.Value.characterId == PcId).FirstOrDefault().Value;

            if (oPC != null && oPC.isConnected)
            {
                oPC.oid.ClearActionQueue();
                oPC.oid.JumpToObject(oTarget);
                oTarget.ApplyEffect(EffectDuration.Instant, API.Effect.Resurrection());
                oPC.oid.HP = 1;
            }
            else
            {
                NWScript.SendMessageToPC(callInfo.ObjectSelf, "Vous sentez une forme de résistance : cette âme met du temps à regagner son enveloppe corporelle. Votre sort a bien eu l'effet escompté, mais il faudra un certain temps avant de voir le corps s'animer.");

                var query = NWScript.SqlPrepareQueryCampaign(Config.database, $"UPDATE playerCharacters SET areaTag = @areaTag, position = @position WHERE characterId = @characterId");
                NWScript.SqlBindInt(query, "@characterId", PcId);
                NWScript.SqlBindString(query, "@areaTag", oTarget.Area.Tag);
                NWScript.SqlBindVector(query, "@position", oTarget.Position);
                NWScript.SqlStep(query);
            }

            oTarget.Items.Where(c => c.Tag == "item_pccorpse").FirstOrDefault().Destroy();
            oTarget.Destroy();

            PlayerSystem.DeletePlayerCorpseFromDatabase(PcId);

            NWScript.SignalEvent(oTarget, NWScript.EventSpellCastAt(callInfo.ObjectSelf, NWScript.SPELL_RAISE_DEAD, 0));
            oTarget.Location.ApplyEffect(EffectDuration.Instant, API.Effect.VisualEffect(API.Constants.VfxType.ImpRaiseDead));
        }
    }
}