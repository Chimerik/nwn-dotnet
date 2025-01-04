using System;
using System.Collections.Generic;
using Anvil.API;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> InvocationDespritDragon(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      List<NwGameObject> targetList = new();

      if (oCaster is NwCreature caster)
      {
        bool compagnonDraconique = false;

        if (caster.KnowsFeat((Feat)CustomSkill.EnsoCompagnonDraconique))
        {
          if (caster.GetObjectVariable<PersistentVariableString>("_COMPAGNON_DRACONIQUE_COOLDOWN").HasNothing
            || (DateTime.TryParse(caster.GetObjectVariable<PersistentVariableString>("_COMPAGNON_DRACONIQUE_COOLDOWN").Value, out var cooldown)
                && cooldown < DateTime.Now))
            compagnonDraconique = true;
        }

        if (!compagnonDraconique)
        {
          if (caster.Gold < 500)
          {
            caster.LoginPlayer?.SendServerMessage("Ce sort nécessite des composants d'une valeur de 500 similis", ColorConstants.Red);
            return targetList;
          }
          else
            caster.Gold -= 500;
        }

        caster.LoginPlayer?.SendServerMessage("Sort non implémenté pour le moment");

        DelayVfx(targetLocation);
        targetLocation.ApplyEffect(EffectDuration.Temporary, Effect.SummonCreature("x2_s_drgred001", VfxType.FnfSummondragon, appearType:1), SpellUtils.GetSpellDuration(oCaster, spellEntry));
        
        if(!compagnonDraconique)
          targetList.Add(UtilPlugin.GetLastCreatedObject(NWNXObjectType.Creature).ToNwObject<NwCreature>());
      }

      return targetList;
    }
    private static async void DelayVfx(Location targetLocation)
    {
      await NwTask.Delay(TimeSpan.FromSeconds(1));
      targetLocation.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpDustExplosion));
    }
  }
}
