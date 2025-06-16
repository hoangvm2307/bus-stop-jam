#pragma warning disable 0414

using UnityEngine;

namespace Watermelon
{
    [RegisterModule("Save Controller", core: true, order: 900)]
    public class SaveInitModule : InitModule
    {
        public override string ModuleName => "Save Controller";

        [SerializeField] float autoSaveDelay = 0;
        [SerializeField] bool cleanSaveStart = false;

        [Space]
        [SerializeField] string webGLPrefix = "gameName";

        public override void CreateComponent()
        { 
            if (cleanSaveStart)
            {
                ES3.DeleteFile();
                Debug.LogWarning("[SaveInitModule]: All save data has been cleared!");
            }
 
            GameObject saveManagerObject = new GameObject("[SAVE MANAGER]"); 
            saveManagerObject.AddComponent<SaveManager>();
             
        }
    }
}
