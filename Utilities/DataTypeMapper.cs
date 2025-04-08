namespace genapi_api.Utilities
{
    public static class DataTypeMapper
    {
        public static string MapDataType(string value)
        {
            ArgumentNullException.ThrowIfNull(value);

            string valueToLower = value.ToLower();

            switch (valueToLower)
            {
                case "string":
                case "varchar":
                    return "String";
                case "int":
                    return "int";
                case "decimal":
                    return "decimal";
                case "datetime":
                    return "DateTime";
                case "date":
                    return "DateOnly";
                default:
                    throw new ArgumentException($"Invalid data type value. Value was '{valueToLower}'");
            }
        }
    }
}
