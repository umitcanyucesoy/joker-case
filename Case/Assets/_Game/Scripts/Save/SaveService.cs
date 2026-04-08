using UnityEngine;

namespace Save
{
    public class SaveService : ISaveService
    {
        public void SaveInt(string key, int value)   => PlayerPrefs.SetInt(key, value);
        public int LoadInt(string key, int defaultValue = 0) => PlayerPrefs.GetInt(key, defaultValue);
        public void Save() => PlayerPrefs.Save();
    }
}

