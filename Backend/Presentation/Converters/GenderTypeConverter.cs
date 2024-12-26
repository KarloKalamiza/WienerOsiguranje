using Backend.Core.Domain.Enums;

namespace Backend.Presentation.Converters;

public static class GenderTypeConverter
{
    public static string ToDatabaseValue(this GenderType gender)
    {
        return gender switch
        {
            GenderType.M => "M",
            GenderType.F => "F",
            GenderType.N => "N",
            _ => throw new ArgumentException("Invalid GenderType value.")
        };
    }

    public static GenderType FromDatabaseValue(string gender)
    {
        return gender switch
        {
            "M" => GenderType.M,
            "F" => GenderType.F,
            "N" => GenderType.N,
            _ => throw new ArgumentException("Invalid gender value from database.")
        };
    }
}
