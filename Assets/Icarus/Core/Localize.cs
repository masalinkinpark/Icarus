using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Icarus.Core
{
    public static class Localize
    {
        private static Dictionary<int, LocalizationModel> _localizedSet = new Dictionary<int, LocalizationModel>();

        public static LocalizationModel CreateLocalizationModel(string defaultLanguage, string targetLanguage, string targetRawText)
        {
            var dic = TextLocalizer.GetLocalizedTextDictionary(defaultLanguage, targetLanguage, targetRawText);
            return new LocalizationModel(dic);
        }

        public static void Clear()
        {
            _localizedSet = new Dictionary<int, LocalizationModel>();
        }

        public static void Set(int localizedFileKey, string defaultLanguage, string targetLanguage, string targetRawText)
        {
            if (_localizedSet.ContainsKey(localizedFileKey))
            {
                Debug.Log($"Key ({localizedFileKey}) duplicated. Update Localize Text.");
                return;
            }

            var dic = TextLocalizer.GetLocalizedTextDictionary(defaultLanguage, targetLanguage, targetRawText);
            _localizedSet.Add(localizedFileKey, new LocalizationModel(dic));
        }

        public static void Update(int localizedFileKey, string defaultLanguage, string targetLanguage, string targetRawText)
        {
            if (!_localizedSet.ContainsKey(localizedFileKey))
            {
                Debug.LogWarning($"Key ({localizedFileKey}) not Found.");
                return;
            }

            _localizedSet.Remove(localizedFileKey);
            var dic = TextLocalizer.GetLocalizedTextDictionary(defaultLanguage, targetLanguage, targetRawText);
            _localizedSet.Add(localizedFileKey, new LocalizationModel(dic));
        }

        public static string GetText(int localizedFileKey, string key)
        {
            return _localizedSet[localizedFileKey].GetText(key);
        }

        public static string GetText(int localizedFileKey, string key, params object[] args)
        {
            return _localizedSet[localizedFileKey].GetText(key, args);
        }
    }
}