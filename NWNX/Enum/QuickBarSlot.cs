namespace NWN.NWNX.Enum {
  public class QuickBarSlot {
    public NWObject? Item { get; set; }
    public NWObject? SecondaryItem { get; set; }
    public QuickBarSlotType ObjectType { get; set; }
    public int MultiClass { get; set; }
    public string? Resref { get; set; }
    public string? CommandLabel { get; set; }
    public string? CommandLine { get; set; }
    public string? ToolTip { get; set; }
    public int INTParam1 { get; set; }
    public int MetaType { get; set; }
    public int DomainLevel { get; set; }
    public int AssociateType { get; set; }
    public NWObject? Associate { get; set; }

    public static QuickBarSlot CreateEmptyQBS()
    {
      QuickBarSlot emptyQBS = new QuickBarSlot();
      emptyQBS.ObjectType = QuickBarSlotType.Empty;

      emptyQBS.Item = NWObject.OBJECT_INVALID.AsObject();
      emptyQBS.SecondaryItem = NWObject.OBJECT_INVALID.AsObject();
      emptyQBS.MultiClass = 0;
      emptyQBS.Resref = "";
      emptyQBS.CommandLabel = "";
      emptyQBS.CommandLine = "";
      emptyQBS.ToolTip = "";
      emptyQBS.INTParam1 = 0;
      emptyQBS.MetaType = 0;
      emptyQBS.DomainLevel = 0;
      emptyQBS.AssociateType = 0;
      emptyQBS.Associate = NWObject.OBJECT_INVALID.AsObject();

      return emptyQBS;
    }
  }
}
