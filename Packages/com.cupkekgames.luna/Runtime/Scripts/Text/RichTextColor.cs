namespace CupkekGames.Luna
{
  public static class RichTextColor
  {
    public const string CLOSING_TAG = "</color>";
    public const string TRANSPARENT = "<color=#0000>";
    public const string WHITE = "<color=#FFFFFF>";
    public const string PURPLE = "<color=#a280e5>";
    public const string AQUA = "<color=#91ebd9>";
    public const string LIME = "<color=#a3e635>";
    public const string RED = "<color=#ea5252>";
    public const string YELLOW = "<color=#fff75a>";
    public const string ORANGE = "<color=#f1b83b>";

    public static string Colorize(string text, string color)
    {
      return $"{color}{text}{CLOSING_TAG}";
    }
  }
}
