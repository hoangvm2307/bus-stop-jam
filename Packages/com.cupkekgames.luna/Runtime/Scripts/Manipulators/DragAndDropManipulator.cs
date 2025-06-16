using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System;

namespace CupkekGames.Luna
{
  public abstract class DragAndDropManipulator : PointerManipulator
  {
    // References
    protected LunaUIManager _uiElementManager;
    protected List<VisualElement> _dropSlots;
    protected Func<List<VisualElement>> _getDropSlots;
    // Proporties
    protected int _key;
    protected VisualElement _dragArea;
    protected List<string> _dropSlotClasses;
    // State
    protected bool _enabled;
    protected VisualElement _clone = null;
    // Events
    public event Action<Vector3> OnMove;
    public event Action<Vector3> OnStart;
    public event Action<int, int, VisualElement> OnDrop; // key, dropSlotIndex, dropSlotVE
    public event Action OnCancel;
    public DragAndDropManipulator(
      LunaUIManager uiElementManager,
      VisualElement dragArea,
      int key,
      List<VisualElement> dropSlots,
      Func<List<VisualElement>> getDropSlots = null
    )
    {
      _uiElementManager = uiElementManager;
      _dragArea = dragArea;
      _key = key;
      _dropSlots = dropSlots;
      _getDropSlots = getDropSlots;
    }
    public abstract VisualElement CreateDragElement();
    public abstract void DisposeDragElement();

    public void SetDropSlotClasses(List<string> classes)
    {
      _dropSlotClasses = classes;
    }

    public void ApplyDropSlotClasses(bool apply)
    {
      if (_dropSlotClasses == null || _dropSlotClasses.Count == 0)
      {
        return;
      }

      foreach (var dropSlot in GetDropSlots())
      {
        foreach (var className in _dropSlotClasses)
        {
          if (apply)
          {
            dropSlot.AddToClassList(className);
          }
          else
          {
            dropSlot.RemoveFromClassList(className);
          }
        }
      }
    }

    protected override void RegisterCallbacksOnTarget()
    {
      // Register the four callbacks on target.
      target.RegisterCallback<PointerDownEvent>(PointerDownHandler, TrickleDown.TrickleDown);
      target.RegisterCallback<PointerMoveEvent>(PointerMoveHandler);
      target.RegisterCallback<PointerUpEvent>(PointerUpHandler);
      target.RegisterCallback<PointerCaptureOutEvent>(PointerCaptureOutHandler);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
      // Un-register the four callbacks from target.
      target.UnregisterCallback<PointerDownEvent>(PointerDownHandler, TrickleDown.TrickleDown);
      target.UnregisterCallback<PointerMoveEvent>(PointerMoveHandler);
      target.UnregisterCallback<PointerUpEvent>(PointerUpHandler);
      target.UnregisterCallback<PointerCaptureOutEvent>(PointerCaptureOutHandler);

      if (_clone != null)
      {
        _dragArea.Remove(_clone);
        _clone = null;
      }
    }

    // This method stores the starting position of target and the pointer,
    // makes target capture the pointer, and denotes that a drag is now in progress.
    private void PointerDownHandler(PointerDownEvent evt)
    {
      target.CapturePointer(evt.pointerId);
      _enabled = true;
    }

    // This method checks whether a drag is in progress and whether target has captured the pointer.
    // If both are true, calculates a new position for target within the bounds of the window.
    private void PointerMoveHandler(PointerMoveEvent evt)
    {
      if (_enabled && target.HasPointerCapture(evt.pointerId))
      {
        Vector3 pointerPosition = evt.position;

        if (_clone == null)
        {
          ApplyDropSlotClasses(true);

          // Clone the target and add it to the root.
          _clone = CreateDragElement();
          _clone.style.position = Position.Absolute;

          _dragArea.Add(_clone);

          target.Blur();

          OnStart?.Invoke(pointerPosition);

          if (_uiElementManager != null)
          {
            _uiElementManager.AudioHandler.DisableAudio = true;
          }
        }

        _clone.transform.position = pointerPosition;
        OnMove?.Invoke(pointerPosition);

        // Vector3 pointerDelta = evt.position - _pointerStartPosition;

        // _clone.transform.position = new Vector2(
        //     Mathf.Clamp(_targetStartPosition.x + pointerDelta.x, 0, target.panel.visualTree.worldBound.width),
        //     Mathf.Clamp(_targetStartPosition.y + pointerDelta.y, 0, target.panel.visualTree.worldBound.height));
      }
    }

    // This method checks whether a drag is in progress and whether target has captured the pointer.
    // If both are true, makes target release the pointer.
    private void PointerUpHandler(PointerUpEvent evt)
    {
      if (_enabled && target.HasPointerCapture(evt.pointerId))
      {
        target.ReleasePointer(evt.pointerId);
      }
    }

    // This method checks whether a drag is in progress. If true, queries the root
    // of the visual tree to find all slots, decides which slot is the closest one
    // that overlaps target, and sets the position of target so that it rests on top
    // of that slot. Sets the position of target back to its original position
    // if there is no overlapping slot.
    private void PointerCaptureOutHandler(PointerCaptureOutEvent evt)
    {
      if (_enabled && _clone != null)
      {
        // UQueryBuilder<VisualElement> overlappingSlots = slots.Where(OverlapsTarget);
        int overlappingSlot = -1;

        List<VisualElement> dropSlots = GetDropSlots();

        for (int i = 0; i < dropSlots.Count; i++)
        {
          if (OverlapsTarget(dropSlots[i]))
          {
            overlappingSlot = i;
            break;
          }
        }

        if (overlappingSlot != -1)
        {
          OnDrop?.Invoke(_key, overlappingSlot, dropSlots[overlappingSlot]);
        }
        else
        {
          OnCancel?.Invoke();
        }
        
        if (_uiElementManager != null)
        {
          _uiElementManager.AudioHandler.DisableAudio = false;
        }

        // Vector3 closestPos = Vector3.zero;
        // if (closestOverlappingSlot != null)
        // {
        //   closestPos = RootSpaceOfSlot(closestOverlappingSlot);
        //   closestPos = new Vector2(closestPos.x - 5, closestPos.y - 5);
        // }
        // _clone.transform.position =
        //     closestOverlappingSlot != null ?
        //     closestPos :
        //     _targetStartPosition;

        if (_clone != null)
        {
          _dragArea.Remove(_clone);
          _clone = null;
        }

        _enabled = false;
        DisposeDragElement();
        ApplyDropSlotClasses(false);
      }
    }

    private bool OverlapsTarget(VisualElement slot)
    {
      return slot.worldBound.Contains(_clone.transform.position);
    }

    private int FindClosestSlot(List<int> slots, List<VisualElement> dropSlots)
    {
      Dictionary<int, VisualElement> slotsList = new();

      for (int i = 0; i < slots.Count; i++)
      {
        int index = slots[i];
        slotsList.Add(index, dropSlots[index]);
      }

      float bestDistanceSq = float.MaxValue;
      int closest = -1;
      Vector3 clonePosition = _clone.transform.position;

      foreach (var pair in slotsList)
      {
        Vector3 displacement = RootSpaceOfSlot(pair.Value) - clonePosition;
        float distanceSq = displacement.sqrMagnitude;
        if (distanceSq < bestDistanceSq)
        {
          bestDistanceSq = distanceSq;
          closest = pair.Key;
        }
      }
      return closest;
    }

    private Vector3 RootSpaceOfSlot(VisualElement slot)
    {
      Vector2 slotWorldSpace = slot.parent.LocalToWorld(slot.layout.position);
      return _dragArea.WorldToLocal(slotWorldSpace);
    }

    public List<VisualElement> GetDropSlots()
    {
      List<VisualElement> dropSlots = new();

      if (_dropSlots != null)
      {
        dropSlots.AddRange(_dropSlots);
      }

      if (_getDropSlots != null)
      {
        dropSlots.AddRange(_getDropSlots());
      }

      return dropSlots;
    }
  }
}
