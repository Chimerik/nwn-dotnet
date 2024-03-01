using Anvil.API;

namespace NWN.Systems
{
  public static partial class TrapUtils
  {
    public static void SendSavingThrowFeedbackMessage(NwCreature target, int saveRoll, int proficiencyBonus, int advantage, int spellDC, int totalSave, bool saveFailed, Ability ability)
    {
      string advantageString = advantage == 0 ? "" : advantage > 0 ? " (Avantage)".ColorString(StringUtils.gold) : " (Désavantage)".ColorString(ColorConstants.Red);
      string hitString = saveFailed ? "ECHEC".ColorString(ColorConstants.Red) : "REUSSI".ColorString(StringUtils.brightGreen);
      Color hitColor = saveFailed ? ColorConstants.Red : StringUtils.brightGreen;

      string rollString = $"JDS {StringUtils.TranslateAttributeToFrench(ability)}{advantageString} {StringUtils.IntToColor(saveRoll, hitColor)} + {StringUtils.IntToColor(proficiencyBonus, hitColor)} = {StringUtils.IntToColor(totalSave, hitColor)} vs DD {StringUtils.IntToColor(spellDC, hitColor)}";

      target.LoginPlayer?.SendServerMessage($"{rollString} {hitString}".ColorString(ColorConstants.Orange));
      LogUtils.LogMessage($"{rollString} {hitString}".StripColors(), LogUtils.LogType.Combat);
    }
  }
}
