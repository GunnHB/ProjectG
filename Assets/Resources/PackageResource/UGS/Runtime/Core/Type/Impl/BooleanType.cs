using GoogleSheet.Type;

[Type(Type: typeof(bool), TypeName: new string[] { "bool", "boolean", "Boolean" })]
public class BooleanType : IType
{
    public object DefaultValue => false;

    public object Read(string value)
    {
        if (string.IsNullOrEmpty(value))
            throw new UGSValueParseException("Parse Faield => " + value + " To " + this.GetType().Name);

        bool boolean = false;
        var b = System.Boolean.TryParse(value, out boolean);

        if (b == false)
            throw new UGSValueParseException("Parse Faield => " + value + " To " + this.GetType().Name);

        return boolean;
    }

    public string Write(object value)
    {
        return value.ToString();
    }
}
