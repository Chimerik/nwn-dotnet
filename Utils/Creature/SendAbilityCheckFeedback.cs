using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static void SendSkillCheckFeedback(NwCreature caster, NwCreature target, int saveRoll, int proficiencyBonus, int advantage, int DC, int totalSave, bool saveFailed, string skill)
    {
      string advantageString = advantage == 0 ? "" : advantage > 0 ? " (Avantage)".ColorString(StringUtils.gold) : " (Désavantage)".ColorString(ColorConstants.Red);
      string hitString = saveFailed ? "ECHEC".ColorString(ColorConstants.Red) : "REUSSI".ColorString(StringUtils.brightGreen);
      Color hitColor = saveFailed ? ColorConstants.Red : StringUtils.brightGreen;

      string rollString = $"Jet de {skill}{advantageString} {StringUtils.IntToColor(saveRoll, hitColor)} + {StringUtils.IntToColor(proficiencyBonus, hitColor)} = {StringUtils.IntToColor(totalSave, hitColor)} vs DD {StringUtils.IntToColor(DC, hitColor)}";

      StringUtils.DisplayStringToAllPlayersNearTarget(target, $"{target.Name.ColorString(ColorConstants.Cyan)} - {advantageString}{rollString} {hitString}", ColorConstants.Orange, true, true);
    }
    public static void SendSkillCheckFeedback(NwCreature target, int saveRoll, int proficiencyBonus, int advantage, int DC, int totalSave, bool saveFailed, string skill)
    {
      string advantageString = advantage == 0 ? "" : advantage > 0 ? " (Avantage)".ColorString(StringUtils.gold) : " (Désavantage)".ColorString(ColorConstants.Red);
      string hitString = saveFailed ? "ECHEC".ColorString(ColorConstants.Red) : "REUSSI".ColorString(StringUtils.brightGreen);
      Color hitColor = saveFailed ? ColorConstants.Red : StringUtils.brightGreen;

      string rollString = $"Jet de {skill}{advantageString} {StringUtils.IntToColor(saveRoll, hitColor)} + {StringUtils.IntToColor(proficiencyBonus, hitColor)} = {StringUtils.IntToColor(totalSave, hitColor)} vs DD {StringUtils.IntToColor(DC, hitColor)}";

      StringUtils.DisplayStringToAllPlayersNearTarget(target, $"{target.Name.ColorString(ColorConstants.Cyan)} - {advantageString}{rollString} {hitString}", ColorConstants.Orange, true, true);
    }
  }
}
