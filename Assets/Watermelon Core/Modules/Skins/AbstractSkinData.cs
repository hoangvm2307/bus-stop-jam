using UnityEngine;

namespace Watermelon
{
    public abstract class AbstractSkinData : ISkinData
    {
        [SerializeField, UniqueID] string id;
        public string ID => id;
        public int Hash { get; private set; }

        public AbstractSkinDatabase SkinsProvider { get; private set; }

        public bool IsUnlocked => save.IsUnlocked;

        private SkinSave save;

        public virtual void Init(AbstractSkinDatabase provider)
        {
            save = new SkinSave(this.id);
            save.Load();
            SaveManager.Register(save);

            Hash = id.GetHashCode();
            SkinsProvider = provider;
        }

        public void Unlock()
        {
            save.IsUnlocked = true;
            save.Save();
        }
    }
}
