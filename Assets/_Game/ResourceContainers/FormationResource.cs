public class FormationResource {
    public readonly int MaxRows;
    public readonly int MaxColumns;
    public readonly int UsedSlotsNumber;
    public readonly int[][] Values;

    public FormationResource(int maxRows, int maxColumns, int usedSlotsNumber, int[][] values) {
        MaxRows = maxRows;
        MaxColumns = maxColumns;
        UsedSlotsNumber = usedSlotsNumber;
        Values = values;
    }
}