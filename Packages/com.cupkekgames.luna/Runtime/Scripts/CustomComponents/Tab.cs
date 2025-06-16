using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna
{
    [UxmlElement]
    public partial class Tab : VisualElement
    {
        private string _label;
        [UxmlAttribute]
        public string Label
        {
            get
            {
                return _label;
            }
            set
            {
                _label = value;
            }
        }
        private Sprite _iconImage;
        [UxmlAttribute]
        public Sprite IconImage
        {
            get
            {
                return _iconImage;
            }
            set
            {
                _iconImage = value;
            }
        }
        private VisualTreeAsset _tabButtonContent;
        [UxmlAttribute]
        public VisualTreeAsset TabButtonContent
        {
            get
            {
                return _tabButtonContent;
            }
            set
            {
                _tabButtonContent = value;
            }
        }
        private Vector2Int _contentSizePercent = new Vector2Int(100, 100);
        [UxmlAttribute]
        public Vector2Int ContentSizePercent
        {
            get
            {
                return _contentSizePercent;
            }
            set
            {
                _contentSizePercent = value;
            }
        }
        private bool _overflow = false;
        [UxmlAttribute]
        public bool Overflow
        {
            get
            {
                return _overflow;
            }
            set
            {
                _overflow = value;
            }
        }
        private Vector2Int _headerMinSize = new Vector2Int(0, 0);
        [UxmlAttribute]
        public Vector2Int HeaderMinSize
        {
            get
            {
                return _headerMinSize;
            }
            set
            {
                _headerMinSize = value;
            }
        }
    }
}