namespace CupkekGames.Luna
{
  public enum UIColorValue
  {
    V_50,
    V_100,
    V_200,
    V_300,
    V_400,
    V_500,
    V_600,
    V_700,
    V_800,
    V_900,
    V_950
  }

  public static class UIColorValueExtensions
  {
    public static string GetUSSClass(this UIColorValue uiColorValue)
    {
      // Convert enum to string and remove the "V_" prefix
      return uiColorValue.ToString().Substring(2);
    }
  }
}
