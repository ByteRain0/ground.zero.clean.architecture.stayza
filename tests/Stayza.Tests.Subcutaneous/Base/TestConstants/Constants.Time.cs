namespace Stayza.Tests.Subcutaneous.Base.TestConstants;

public static partial class Constants
{
    public static class Time
    {
        public static readonly DateTimeOffset TestUtcNow = new(
            year: 2024,
            month: 12,
            day: 25,
            hour: 15,
            minute: 30,
            second: 0,
            offset: TimeSpan.Zero);
    }
}