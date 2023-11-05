namespace SimpleAutoMapper.Test.Objects;

public class ScalarSrc
{
    public int MemberInterger;
    public double MemberDouble;
    public string MemberString = null!;

    public int? MemberNullableInterger;
    public double? MemberNullableDouble;
    public string? MemberNullableString;

    public int PropInterger { get; set; }
    public double PropDouble { get; set; }
    public string PropString { get; set; } = null!;

    public int? PropNullableInterger { get; set; }
    public double? PropNullableDouble { get; set; }
    public string? PropNullableString { get; set; }
}
