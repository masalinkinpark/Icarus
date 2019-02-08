using System.Collections.ObjectModel;

namespace Icarus.Core
{
    public class LocalizationModel
    {
        private readonly ReadOnlyDictionary<string, string> _localizedText;

        public LocalizationModel(ReadOnlyDictionary<string, string> localizedText)
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
}