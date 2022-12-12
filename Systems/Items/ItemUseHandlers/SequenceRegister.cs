using System;
using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  class SequenceRegister
  {
    public static async void HandleCastSequence(PlayerSystem.Player player, NwItem oRegister, NwGameObject oTarget)
    {
      if (oTarget == null)
      {
        player.oid.SendServerMessage("Il faut impérativement sélectionner une cible pour pouvoir lancer la séquence de sorts.", ColorConstants.Red);
        return;
      }

      string[] spellList = oRegister.GetObjectVariable<LocalVariableString>("_REGISTERED_SEQUENCE").Value.Split("_");

      float posX = player.oid.ControlledCreature.Position.X;
      float posY = player.oid.ControlledCreature.Position.Y;

      foreach (string spellId in spellList)
      {
        if (player.oid == null || player.oid.ControlledCreature == null || oTarget == null || posX != player.oid.ControlledCreature.Position.X || posY != player.oid.ControlledCreature.Position.Y)
          return;

        int castTime = player.oid.ControlledCreature.ActiveEffects.Any(e => e.EffectType == EffectType.Haste) ? 3 : 6;

        await player.oid.ControlledCreature.ActionCastSpellAt((Spell)int.Parse(spellId), oTarget);
        await NwTask.Delay(TimeSpan.FromSeconds(castTime));
      }
    }
  }
}
