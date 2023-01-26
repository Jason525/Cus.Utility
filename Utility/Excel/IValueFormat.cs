namespace Utility.Excel
{
    public interface IValueFormat
    {
        string Pattern { get; set; }
        string Format(object value);
    }
}
