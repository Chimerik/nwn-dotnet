using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static void SendSaveAutoFailMessage(NwGameObject oCaster, NwCreature target, string spellName, string abilityName)
    {
      if(oCaster is NwCreature caster)
        caster.LoginPlayer?.SendServerMessage($"{target.Name.ColorString(ColorConstants.Cyan)} - JDS {abilityName} vs {spellName} : {"ECHEC AUTOMATIQUE".ColorString(ColorConstants.Red)}".ColorString(ColorConstants.Orange));

      if (target != oCaster)
        target.LoginPlayer?.SendServerMessage($"{oCaster.Name.ColorString(ColorConstants.Cyan)} - JDS {abilityName} vs {spellName} : {"ECHEC AUTOMATIQUE".ColorString(ColorConstants.Red)}".ColorString(ColorConstants.Orange));
    }
  }
}
