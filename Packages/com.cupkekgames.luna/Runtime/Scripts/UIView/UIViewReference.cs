using System.Collections;
using System.Collections.Generic;
using CupkekGames.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna
{
    public class UIViewReference : MonoBehaviour
    {
        // UI Panel Base
        public List<UIView> List = new();

        // public void OnEnable()
        // {
        //     UIView.OnEnable();
        // }

        public void OnDisable()
        {
            foreach (var view in List)
            {
                view.OnDisable();
            }
        }
    }
}