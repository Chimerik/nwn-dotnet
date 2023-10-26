using Anvil.API;

namespace NWN.Systems
{
  public static partial class SpellUtils
  {
    public static void SendSaveAutoFailMessage(NwCreature caster, NwCreature target, string spellName, string abilityName)
    {
      string autoFail = "ECHEC AUTOMATIQUE".ColorString(ColorConstants.Red);
      caster.LoginPlayer?.SendServerMessage($"{target.Name.ColorString(ColorConstants.Cyan)} - JDS {abilityName} vs {spellName} : {autoFail}".ColorString(ColorConstants.Orange));

      if (target != caster)
        target.LoginPlayer?.SendServerMessage($"{caster.Name.ColorString(ColorConstants.Cyan)} - JDS {abilityName} vs {spellName} : {autoFail}".ColorString(ColorConstants.Orange));
    }
  }
}
