namespace genapi_api.Data
{
    public class Property
    {
        public string Type { get; set; } = String.Empty;
        public string Name { get; set; } = String.Empty;
        public bool Nullable { get; set; } = false;
        public bool PrimaryKey { get; set; } = false;
        public bool ForeignKey { get; set; } = false;
        public string ForeignKeyReference { get; set; } = String.Empty;
        public int? MaxLength { get; set; }
        public int? DecimalIntegerPart { get; set; }
        public int? DecimalFractionPart { get; set; }
    }
}
