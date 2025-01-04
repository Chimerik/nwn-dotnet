using System;
using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static NwCreature SummonAssociate(NwCreature creature, AssociateType associateType, string resRef)
    {
      if (string.IsNullOrEmpty(creature.AnimalCompanionName))
        creature.AnimalCompanionName = $"Compagnon de {creature.Name}";

      if (string.IsNullOrEmpty(creature.FamiliarName))
        creature.FamiliarName = $"Familier de {creature.Name}";

      switch (associateType)
      {
        case AssociateType.AnimalCompanion: creature.SummonAnimalCompanion(resRef); break;
        default: creature.SummonFamiliar(resRef); break;
      }

      NwCreature companion = UtilPlugin.GetLastCreatedObject(NWNXObjectType.Creature).ToNwObject<NwCreature>();

      companion.SetEventScript(EventScriptType.CreatureOnBlockedByDoor, "nw_ch_ace");
      companion.SetEventScript(EventScriptType.CreatureOnEndCombatRound, "nw_ch_ac3");
      companion.SetEventScript(EventScriptType.CreatureOnDamaged, "nw_ch_ac6");
      companion.SetEventScript(EventScriptType.CreatureOnDeath, "nw_ch_ac7");
      companion.SetEventScript(EventScriptType.CreatureOnDisturbed, "nw_ch_ac8");
      companion.SetEventScript(EventScriptType.CreatureOnHeartbeat, "nw_ch_ac1");
      companion.SetEventScript(EventScriptType.CreatureOnNotice, "nw_ch_ac2");
      companion.SetEventScript(EventScriptType.CreatureOnMeleeAttacked, "nw_ch_ac5");
      companion.SetEventScript(EventScriptType.CreatureOnRested, "nw_ch_aca");
      companion.SetEventScript(EventScriptType.CreatureOnSpawnIn, "nw_ch_acani9");
      companion.SetEventScript(EventScriptType.CreatureOnSpellCastAt, "nw_ch_acb");
      companion.SetEventScript(EventScriptType.CreatureOnUserDefinedEvent, "nw_ch_acd");
      companion.SetEventScript(EventScriptType.CreatureOnDialogue, "nw_ch_ac4");

      GC.SuppressFinalize(companion);

      return companion;
    }
  }
}
