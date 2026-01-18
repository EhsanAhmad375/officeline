namespace officeline.ErrorExceptions
{
public class ApiException : Exception
{
    public string FieldName { get; }
    public ApiException(string fieldName, string message) : base(message)
    {
        FieldName = fieldName;
    }
}}