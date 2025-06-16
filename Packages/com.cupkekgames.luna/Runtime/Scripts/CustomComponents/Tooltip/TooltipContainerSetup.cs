using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using CupkekGames.Core;

namespace CupkekGames.Luna
{
    public class TooltipContainerSetup
    {
        // Settings
        private VisualElement _image;
        public VisualElement Image => _image;
        private VisualElement _title;
        public VisualElement Title => _title;
        private VisualElement _body;
        public VisualElement Body => _body;
        private VisualElement _bottom;
        public VisualElement Bottom => _bottom;
        private UIColor _colorBackground;
        public UIColor ColorBackground => _colorBackground;
        private UIColor _colorBorder;
        public UIColor ColorBorder => _colorBorder;
        private StyleLength _maxWidth;
        public StyleLength MaxWidth => _maxWidth;
        public TooltipContainerSetup(VisualElement image, VisualElement title, VisualElement body, VisualElement bottom,
            UIColor colorBackground = null, UIColor colorBorder = null, int maxWidth = 512)
        {
            _image = image;
            _title = title;
            _body = body;
            _bottom = bottom;
            _colorBackground = colorBackground;
            _colorBorder = colorBorder;
            _maxWidth = new StyleLength(new Length(maxWidth, LengthUnit.Pixel));

            if (_image != null)
            {
                _image.pickingMode = PickingMode.Ignore;
            }
            if (_title != null)
            {
                _title.pickingMode = PickingMode.Ignore;
            }
            if (_body != null)
            {
                _body.pickingMode = PickingMode.Ignore;
            }
            if (_bottom != null)
            {
                _bottom.pickingMode = PickingMode.Ignore;
            }
        }
    }
}
