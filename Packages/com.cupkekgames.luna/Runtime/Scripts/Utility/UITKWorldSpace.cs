using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna
{
    public class UITKWorldSpace : MonoBehaviour
    {
        private UIDocument _uiDocument;
        private RenderTexture _renderTexture;
        private Renderer _renderer;

        void Awake()
        {
            // Get the UIDocument component
            _uiDocument = GetComponentInChildren<UIDocument>();

            // Create a new PanelSettings instance and assign it to the UIDocument
            _uiDocument.panelSettings = Instantiate(_uiDocument.panelSettings);

            // Create a new RenderTexture with appropriate dimensions and format
            _renderTexture = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
            _renderTexture.Create();

            // Assign the RenderTexture to the PanelSettings' targetTexture
            _uiDocument.panelSettings.targetTexture = _renderTexture;

            // Find the renderer in the child
            _renderer = GetComponentInChildren<Renderer>();
            if (_renderer != null)
            {
                // Create a MaterialPropertyBlock
                MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

                // Set the texture property on the MaterialPropertyBlock
                propertyBlock.SetTexture("_BaseMap", _renderTexture);

                // Apply the MaterialPropertyBlock to the renderer
                _renderer.SetPropertyBlock(propertyBlock);
            }
            else
            {
                Debug.LogError("Renderer component not found in child GameObjects.");
            }
        }
    }
}