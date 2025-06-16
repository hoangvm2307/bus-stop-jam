using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna
{
  public static class TooltipCoordinateCalculator
  {
    public static (TooltipCoordinates coordinates, bool didSwitch) CalculateOptimalPosition(
      TooltipPosition initialPosition,
      VisualElement target,
      VisualElement tooltipElement,
      VisualElement parentElement,
      Vector2? mousePosition = null,
      bool followMouse = false)
    {
      if (target == null) 
        return (new TooltipCoordinates(), false);
        
      Rect tooltipBounds = tooltipElement.worldBound;
      Rect parentBounds = parentElement.worldBound;
      
      // Calculate initial position
      var (initialCoordinates, overflowInfo) = CalculatePositionCoordinates(
        initialPosition, 
        target, 
        tooltipBounds, 
        parentBounds, 
        mousePosition, 
        followMouse
      );

      // If fits perfectly, return as is
      if (overflowInfo.Fits())
        return (initialCoordinates, false);
        
      // Try alternative positions
      var alternativePositions = GetAlternativePositions(initialPosition, overflowInfo);
      TooltipPosition bestPosition = initialPosition;
      TooltipCoordinates bestCoordinates = initialCoordinates;
      OverflowInfo bestOverflowInfo = overflowInfo;
      
      foreach (var position in alternativePositions)
      {
        var (coordinates, posOverflow) = CalculatePositionCoordinates(
          position, target, tooltipBounds, parentBounds, mousePosition, followMouse
        );
        
        // If perfect fit found, use immediately
        if (posOverflow.Fits())
        {
          return (coordinates, true);
        }
        
        // Otherwise track best option
        if (posOverflow.IsBetterThan(bestOverflowInfo))
        {
          bestPosition = position;
          bestCoordinates = coordinates;
          bestOverflowInfo = posOverflow;
        }
      }
      
      // Return best found position
      return (bestCoordinates, bestPosition != initialPosition);
    }

    private static List<TooltipPosition> GetAlternativePositions(
      TooltipPosition currentPosition, 
      OverflowInfo overflowInfo)
    {
      var alternatives = new List<TooltipPosition>();
      
      // For horizontal overflow, prioritize horizontal adjustments
      if (overflowInfo.HasHorizontalOverflow)
      {
        if (currentPosition is TooltipPosition.Left or TooltipPosition.Right)
        {
          // Try opposite horizontal position first
          alternatives.Add(currentPosition.GetOpposite());
          alternatives.Add(TooltipPosition.Top);
          alternatives.Add(TooltipPosition.Bottom);
        }
        else // Top or Bottom
        {
          // Pick the side with more space 
          if (overflowInfo.LeftOverflow > overflowInfo.RightOverflow)
          {
            alternatives.Add(TooltipPosition.Right);
            alternatives.Add(TooltipPosition.Left);
          }
          else
          {
            alternatives.Add(TooltipPosition.Left);
            alternatives.Add(TooltipPosition.Right);
          }
          
          alternatives.Add(currentPosition.GetOpposite());
        }
      }
      // For vertical overflow, prioritize vertical adjustments
      else if (overflowInfo.HasVerticalOverflow)
      {
        if (currentPosition is TooltipPosition.Top or TooltipPosition.Bottom)
        {
          // Try opposite vertical position first
          alternatives.Add(currentPosition.GetOpposite());
          alternatives.Add(TooltipPosition.Left);
          alternatives.Add(TooltipPosition.Right);
        }
        else // Left or Right
        {
          // Pick the side with more space
          if (overflowInfo.TopOverflow > overflowInfo.BottomOverflow)
          {
            alternatives.Add(TooltipPosition.Bottom);
            alternatives.Add(TooltipPosition.Top);
          }
          else
          {
            alternatives.Add(TooltipPosition.Top);
            alternatives.Add(TooltipPosition.Bottom);
          }
          
          alternatives.Add(currentPosition.GetOpposite());
        }
      }
      else
      {
        // Fallback: try all positions
        alternatives.Add(currentPosition.GetOpposite());
        
        foreach (TooltipPosition position in Enum.GetValues(typeof(TooltipPosition)))
        {
          if (position != currentPosition && position != currentPosition.GetOpposite())
            alternatives.Add(position);
        }
      }
      
      return alternatives;
    }

    private static (TooltipCoordinates coordinates, OverflowInfo overflowInfo) 
      CalculatePositionCoordinates(
        TooltipPosition position,
        VisualElement target,
        Rect tooltipBounds,
        Rect parentBounds,
        Vector2? mousePosition,
        bool followMouse)
    {
      float? top = null, left = null, right = null, bottom = null;
      
      // Calculate coordinates based on position
      if (followMouse && mousePosition.HasValue)
      {
        // Position relative to mouse
        switch (position)
        {
          case TooltipPosition.Left:
            right = parentBounds.width - mousePosition.Value.x + 16;
            top = mousePosition.Value.y;
            break;
            
          case TooltipPosition.Bottom:
            left = mousePosition.Value.x;
            top = mousePosition.Value.y + 32;
            break;
            
          case TooltipPosition.Right:
            left = mousePosition.Value.x + 32;
            top = mousePosition.Value.y;
            break;
            
          default: // Top
            left = mousePosition.Value.x;
            top = mousePosition.Value.y - tooltipBounds.height - 16;
            break;
        }
      }
      else
      {
        // Position relative to target element
        if (target == null) 
          return (new TooltipCoordinates(), new OverflowInfo());

        Rect targetBounds = target.worldBound;
        
        switch (position)
        {
          case TooltipPosition.Left:
            right = parentBounds.width - targetBounds.xMin + 16;
            top = targetBounds.yMin;
            break;
            
          case TooltipPosition.Bottom:
            left = targetBounds.xMin;
            top = targetBounds.yMax + 16;
            break;
            
          case TooltipPosition.Right:
            left = targetBounds.xMax + 16;
            top = targetBounds.yMin;
            break;
            
          default: // Top
            left = targetBounds.xMin;
            top = targetBounds.yMin - tooltipBounds.height - 16;
            break;
        }
      }

      var coordinates = new TooltipCoordinates { 
        left = left, 
        right = right, 
        top = top, 
        bottom = bottom 
      };
      
      // Check for overflow
      var overflowInfo = CheckForOverflow(coordinates, tooltipBounds, parentBounds);
      
      return (coordinates, overflowInfo);
    }
    
    private static OverflowInfo CheckForOverflow(
      TooltipCoordinates coordinates, 
      Rect tooltipBounds, 
      Rect parentBounds)
    {
      var overflowInfo = new OverflowInfo();

      // Horizontal overflow check
      if (coordinates.left.HasValue)
      {
        if (coordinates.left < parentBounds.xMin)
        {
          overflowInfo.HasHorizontalOverflow = true;
          overflowInfo.LeftOverflow = parentBounds.xMin - coordinates.left.Value;
        }
        
        if (coordinates.left + tooltipBounds.width > parentBounds.xMax)
        {
          overflowInfo.HasHorizontalOverflow = true;
          overflowInfo.RightOverflow = (coordinates.left.Value + tooltipBounds.width) - parentBounds.xMax;
        }
      }
      
      if (coordinates.right.HasValue)
      {
        if (coordinates.right < 0)
        {
          overflowInfo.HasHorizontalOverflow = true;
          overflowInfo.RightOverflow = -coordinates.right.Value;
        }
        
        float leftEdgePos = parentBounds.width - coordinates.right.Value - tooltipBounds.width;
        if (leftEdgePos < 0)
        {
          overflowInfo.HasHorizontalOverflow = true;
          overflowInfo.LeftOverflow = -leftEdgePos;
        }
      }
      
      // Vertical overflow check
      if (coordinates.top.HasValue)
      {
        if (coordinates.top < parentBounds.yMin)
        {
          overflowInfo.HasVerticalOverflow = true;
          overflowInfo.TopOverflow = parentBounds.yMin - coordinates.top.Value;
        }
        
        if (coordinates.top + tooltipBounds.height > parentBounds.yMax)
        {
          overflowInfo.HasVerticalOverflow = true;
          overflowInfo.BottomOverflow = (coordinates.top.Value + tooltipBounds.height) - parentBounds.yMax;
        }
      }
      
      if (coordinates.bottom.HasValue)
      {
        if (coordinates.bottom < 0)
        {
          overflowInfo.HasVerticalOverflow = true;
          overflowInfo.BottomOverflow = -coordinates.bottom.Value;
        }
        
        float topEdgePos = parentBounds.height - coordinates.bottom.Value - tooltipBounds.height;
        if (topEdgePos < 0)
        {
          overflowInfo.HasVerticalOverflow = true;
          overflowInfo.TopOverflow = -topEdgePos;
        }
      }
      
      // Calculate total overflow
      overflowInfo.TotalOverflow = 
        overflowInfo.LeftOverflow + 
        overflowInfo.RightOverflow + 
        overflowInfo.TopOverflow + 
        overflowInfo.BottomOverflow;

      return overflowInfo;
    }
    
    private class OverflowInfo
    {
      public bool HasHorizontalOverflow { get; set; }
      public bool HasVerticalOverflow { get; set; }
      public float LeftOverflow { get; set; }
      public float RightOverflow { get; set; }
      public float TopOverflow { get; set; }
      public float BottomOverflow { get; set; }
      public float TotalOverflow { get; set; }
      
      public bool Fits() => !HasHorizontalOverflow && !HasVerticalOverflow;
      
      public bool IsBetterThan(OverflowInfo other)
      {
        // First prioritize positions that fit
        if (Fits() && !other.Fits()) return true;
        if (!Fits() && other.Fits()) return false;
        
        // If neither fits, prefer the one with less total overflow
        return TotalOverflow < other.TotalOverflow;
      }
    }
  }
} 