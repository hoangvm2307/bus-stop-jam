#if UNITY_ADDRESSABLES && UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace CupkekGames.Systems.Test.Editor
{
    [CustomEditor(typeof(TestAddressableGameObjectPool))]
    public class TestAddressableGameObjectPoolEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement container = new VisualElement();

            TestAddressableGameObjectPool pool = (TestAddressableGameObjectPool)target;

            Button spawnButton = new Button(() =>
            {
                pool.Spawn();
            })
            {
                text = "Spawn"
            };
            spawnButton.style.flexGrow = 1;
            container.Add(spawnButton);

            Button despawnButton = new Button(() =>
            {
                pool.Despawn();
            })
            {
                text = "Despawn"
            };
            despawnButton.style.flexGrow = 1;
            container.Add(despawnButton);

            Button disposeButton = new Button(() =>
            {
                pool.Dispose();
            })
            {
                text = "Dispose"
            };
            disposeButton.style.flexGrow = 1;
            container.Add(disposeButton);

            Button recreateButton = new Button(() =>
            {
                pool.Recreate();
            })
            {
                text = "Recreate"
            };
            recreateButton.style.flexGrow = 1;
            container.Add(recreateButton);

            // Default inspector elements
            VisualElement containerDefault = new VisualElement();
            InspectorElement.FillDefaultInspector(containerDefault, serializedObject, this);
            container.Add(containerDefault);

            return container;
        }
    }
}
#endif