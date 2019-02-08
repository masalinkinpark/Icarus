using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Icarus.Core
{
    public static class TextLocalizer
    {
        public static string GetText(ReadOnlyDictionary<string, string> localizedDictionary, string localizeKey)
        {
            string text;
            localizedDictionary.TryGetValue(localizeKey, out text);
            if (text == null)
            {
                throw new KeyNotFoundException($"Key Not Found : {localizeKey}");
            }
            return text;
        }

        public static string GetText(ReadOnlyDictionary<string, string> localizedDictionary, string localizeKey, params object[] args)
        {
            string value = string.Empty;
            try
            {
                value = string.Format(localizedDictionary[localizeKey], args);
            }
            catch (FormatException)
            {
                string debugOutput = string.Empty;

                foreach (var arg in args)
                {
                    debugOutput += arg + ",";
                }

                var substring = debugOutput.Substring(0, debugOutput.Length - 1);
                throw new FormatException($"Body の可変長引数に対して、param の可変長引数が足りない Key : {localizeKey} , Body : {localizedDictionary[localizeKey]} , params : {substring}");
            }

            return value;
        }

        internal static ReadOnlyDictionary<string, string> GetLocalizedTextDictionary(string defaultLanguage, string targetLanguage, string rawText)
        {
            var dic = new Dictionary<string, string>();
            var keyValueLine = rawText.Split(new[] { "\n", "\r", "\r\n" }, StringSplitOptions.None);
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