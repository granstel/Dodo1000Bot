namespace Dodo1000Bot.Services;

public static class CheckHelper
{
    /// <summary>
    /// Check remainder of value divided by given value
    /// </summary>
    /// <param name="value">Any value</param>
    /// <param name="given">Given value</param>
    /// <returns><see langword="true"/> when the <paramref name="value"/> is over than 0 and remainder of division by
    /// <paramref name="given"/> equals 0</returns>
    public static bool CheckRemainder(int value, int given)
    {
        return value > 0 && value % given == 0;
    }

    /// <summary>
    /// Check the value is greater than or equal to the given value
    /// </summary>
    /// <param name="value">Any value</param>
    /// <param name="given">Given value</param>
    /// <returns><see langword="true"/> when the <paramref name="value"/> is greater than or equal to <paramref name="given"/> value</returns>
    public static bool CheckGreaterOrEqualGivenValue(decimal value, int given)
    {
        return value >= given;
    }
}