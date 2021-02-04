namespace Crunch.NET.Request.Type
{
    public interface IRequestTypeConverter
    {
        bool CanConvert(string requestType);
        Request Convert(string requestType);
    }
}
