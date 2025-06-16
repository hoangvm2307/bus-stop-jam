using System;

namespace CupkekGames.Luna
{
    [Serializable]
    public class ChoicePopupChoice
    {
        public string Text;
        public UIColorName Color;
        public ChoicePopupChoice()
        {

        }
        public ChoicePopupChoice(string text, UIColorName color)
        {
            Text = text;
            Color = color;
        }
    }
}