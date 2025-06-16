using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using CupkekGames.Core;

namespace CupkekGames.Luna
{
    public class TooltipContainer
    {
        // Settings
        private static UIColor DEFAULT_BG = new UIColor(UIColorName.BASE, UIColorValue.V_900);
        private static UIColor DEFAULT_BORDER = new UIColor(UIColorName.BASE, UIColorValue.V_400);
        // UI Elements
        private VisualElement _container;
        public VisualElement Container => _container;
        private VisualElement _image;
        public VisualElement Image => _image;
        private VisualElement _title;
        public VisualElement Title => _title;
        private VisualElement _body;
        public VisualElement Body => _body;
        private VisualElement _bottom;
        public VisualElement Bottom => _bottom;
        public TooltipContainer(VisualElement container)
        {
            _container = container;
            _image = container.Q<VisualElement>("TooltipImageContainer");
            _title = container.Q<VisualElement>("TooltipTitle");
            _body = container.Q<VisualElement>("TooltipBody");
            _bottom = container.Q<VisualElement>("TooltipBottom");
        }

        public void OnBeforeShow(TooltipContainerSetup setup)
        {
            if (setup == null)
            {
                return;
            }

            ApplyTooltipStyle(setup.ColorBackground, setup.ColorBorder);
            ApplyMaxWidth(setup.MaxWidth);

            if (setup.Image == null)
            {
                _image.style.display = DisplayStyle.None;
            }
            else
            {
                _image.Clear();
                _image.Add(setup.Image);
                _image.style.display = DisplayStyle.Flex;
            }

            if (setup.Title == null)
            {
                _title.style.display = DisplayStyle.None;
            }
            else
            {
                _title.Clear();
                _title.Add(setup.Title);
                _title.style.display = DisplayStyle.Flex;
            }

            if (setup.Body == null)
            {
                _body.style.display = DisplayStyle.None;
            }
            else
            {
                _body.Clear();
                _body.Add(setup.Body);
                _body.style.display = DisplayStyle.Flex;
            }

            if (setup.Bottom == null)
            {
                _bottom.style.display = DisplayStyle.None;
            }
            else
            {
                _bottom.Clear();
                _bottom.Add(setup.Bottom);
                _bottom.style.display = DisplayStyle.Flex;
            }

            Show();
        }

        public void ApplyTooltipStyle(UIColor bg, UIColor border)
        {
            _container.ClearClassList();
            _container.AddToClassList((bg ?? DEFAULT_BG).GetUssClassBG());
            _container.AddToClassList((border ?? DEFAULT_BORDER).GetUssClassBorder());
        }

        public void ApplyMaxWidth(StyleLength maxWidth)
        {
            _container.style.maxWidth = maxWidth;
        }

        public void Hide()
        {
            _container.style.display = DisplayStyle.None;
        }

        public void Show()
        {
            _container.style.display = DisplayStyle.Flex;
        }
    }
}
