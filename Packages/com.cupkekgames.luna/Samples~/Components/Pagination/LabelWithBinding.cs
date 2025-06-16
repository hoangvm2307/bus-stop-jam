using UnityEngine.UIElements;

namespace CupkekGames.Luna.Demo.Components
{
    public class LabelWithBinding
    {
        private Label _label;

        public LabelWithBinding(Label label)
        {
            _label = label;
        }

        public void Bind(string data)
        {
            _label.text = data;
        }

        public void Unbind()
        {
            _label.text = "";
        }
    }
}