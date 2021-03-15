using NWN.API;
using NWN.Services;

namespace NWN.Systems
{
  public static partial class CommandSystem
  {
    private static void ExecuteRecycleCommand(ChatSystem.Context ctx, Options.Result options)
    {
      ctx.oSender.SendServerMessage("Veuillez maintenant sélectionnner l'objet que vous souhaitez recycler.", Color.ROSE);
      PlayerSystem.cursorTargetService.EnterTargetMode(ctx.oSender, OnItemSelected, API.Constants.ObjectTypes.Item, API.Constants.MouseCursor.Kill);
    }
    private static void OnItemSelected(CursorTargetData selection)
    {
      if (selection.TargetObj is null || !(selection.TargetObj is NwItem))
        return;

      new Recycler(selection.Player, (NwItem)selection.TargetObj);
    }
  }
}
