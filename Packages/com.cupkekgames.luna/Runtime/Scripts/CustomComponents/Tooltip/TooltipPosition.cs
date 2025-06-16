namespace CupkekGames.Luna
{
  public enum TooltipPosition
  {
    Top,
    Bottom,
    Left,
    Right
  }

  public static class TooltipPositionExtensions
  {
    public static string GetUssClass(this TooltipPosition tooltipPosition)
    {
      if (tooltipPosition == TooltipPosition.Bottom)
      {
        return "tooltip-bottom";
      }
      else if (tooltipPosition == TooltipPosition.Top)
      {
        return "tooltip-top";
      }
      else if (tooltipPosition == TooltipPosition.Left)
      {
        return "tooltip-left";
      }

      return "tooltip-right";
    }
    public static TooltipPosition GetOpposite(this TooltipPosition tooltipPosition)
    {
      switch (tooltipPosition)
      {
        case TooltipPosition.Top: return TooltipPosition.Bottom;
        case TooltipPosition.Bottom: return TooltipPosition.Top;
        case TooltipPosition.Left: return TooltipPosition.Right;
        case TooltipPosition.Right: return TooltipPosition.Left;
      }

      return TooltipPosition.Top;
    }
  }
}
