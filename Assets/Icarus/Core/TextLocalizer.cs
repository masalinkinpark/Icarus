using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Icarus.Core
{
    public class LocalizedModel
    {
        private readonly ReadOnlyDictionary<string, string> _localizedText;

        public LocalizedModel(ReadOnlyDictionary<string, string> localizedText)
        {
            _localizedText = localizedText;
        }

        public string GetText(string key)
        {
            return TextLocalizer.GetText(_localizedText, key);
        }

        public string GetText(string key, params object[] args)
        {
            return TextLocalizer.GetText(_localizedText, key, args);
        }
    }

    public static partial class TextLocalizer
    {
        private static Dictionary<int, LocalizedModel> _localizeSet = new Dictionary<int, LocalizedModel>();

        public static void Clear()
        {
            _localizeSet = new Dictionary<int, LocalizedModel>();
        }

        public static void Set(int localizedFileKey, string defaultLanguage, string targetLanguage, string targetRawText)
        {
            if (_localizeSet.ContainsKey(localizedFileKey))
            {
                Debug.Log($"Key ({localizedFileKey}) duplicated. Update Localize Text.");
                return;
            }

            var dic = TextLocalizer.GetLocalizedText(defaultLanguage, targetLanguage, targetRawText);
            _localizeSet.Add(localizedFileKey, new LocalizedModel(dic));
        }

        public static void Update(int localizedFileKey, string defaultLanguage, string targetLanguage, string targetRawText)
        {
            if (!_localizeSet.ContainsKey(localizedFileKey))
            {
                Debug.LogWarning($"Key ({localizedFileKey}) not Found.");
                return;
            }

            _localizeSet.Remove(localizedFileKey);
            var dic = TextLocalizer.GetLocalizedText(defaultLanguage, targetLanguage, targetRawText);
            _localizeSet.Add(localizedFileKey, new LocalizedModel(dic));
        }

        public static string GetText(int localizedFileKey, string key)
        {
            return _localizeSet[localizedFileKey].GetText(key);
        }

        public static string GetText(int localizedFileKey, string key, params object[] args)
        {
            return _localizeSet[localizedFileKey].GetText(key, args);
        }

        public static LocalizedModel CreateLocalizedModel(string defaultLanguage, string targetLanguage, string targetRawText)
        {
            var dic = TextLocalizer.GetLocalizedText(defaultLanguage, targetLanguage, targetRawText);
            return new LocalizedModel(dic);
        }

        public static string GetText(ReadOnlyDictionary<string, string> localizedText, string key)
        {
            string text;
            localizedText.TryGetValue(key, out text);
            if (text == null)
            {
                throw new KeyNotFoundException($"Key Not Found : {key}");
            }
            return text;
        }

        public static string GetText(ReadOnlyDictionary<string, string> localizedText, string key, params object[] args)
        {
            string value = string.Empty;
            try
            {
                value = string.Format(localizedText[key], args);
            }
            catch (FormatException)
            {
                string debugOutput = string.Empty;

                foreach (var arg in args)
                {
                    debugOutput += arg + ",";
                }

                var substring = debugOutput.Substring(0, debugOutput.Length - 1);
                throw new FormatException($"Body の可変長引数に対して、param の可変長引数が足りない Key : {key} , Body : {localizedText[key]} , params : {substring}");
            }

            return value;
        }

        private static ReadOnlyDictionary<string, string> GetLocalizedText(string defaultLanguage, string targetLanguage, string text)
        {
            var dic = new Dictionary<string, string>();
            var keyValueLine = text.Split(new[] { "\n", "\r", "\r\n" }, StringSplitOptions.None);
            var langAttribute = keyValueLine[0].Split(',');
            int defaultLanguageIndex = 0;
            int targetLanguageIndex = 0;

            for (int i = 0; i < langAttribute.Length; i++)
            {
                if (defaultLanguage == langAttribute[i])
                {
                    defaultLanguageIndex = i;
                }

                if (targetLanguage == langAttribute[i])
                {
                    targetLanguageIndex = i;
                }
            }

            if (defaultLanguageIndex == 0)
            {
                throw new Exception($"Language \"{defaultLanguage}\" not Exists");
            }

            if (targetLanguageIndex == 0)
            {
                throw new Exception($"Language \"{targetLanguage}\" not Exists");
            }

            for (int rawTextLine = 1; rawTextLine < keyValueLine.Length; rawTextLine++)
            {
                // "//" : Comment out
                if (keyValueLine[rawTextLine].StartsWith("//"))
                {
                    continue;
                }

                // keyValue.Length が 1 以下だと、正しく設定されていないもしくは空白行部分なので避ける
                if (keyValueLine[rawTextLine].Split(',').Length <= 1)
                {
                    continue;
                }

                var lineModel = new LineModel(defaultLanguageIndex, targetLanguageIndex, keyValueLine[rawTextLine].Split(','));
                dic.Add(lineModel.Key, lineModel.Value);
            }

            return new ReadOnlyDictionary<string, string>(dic);
        }
    }
}