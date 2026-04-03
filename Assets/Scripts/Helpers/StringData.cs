
using UnityEngine;

namespace Helpers
{
    public class StringData
    {
        public string mainTitle;
        public string dispatcher;
        public string think;
        public string mean;
        public string choose;
        public string[] bigText;

        public static StringData Load()
        {
            TextAsset file = Resources.Load<TextAsset>("Strings/Strings");
            return JsonUtility.FromJson<StringData>(file.text);
        }
        public string Get(string key)
        {
            var field = typeof(StringData).GetField(key);
            if (field != null)
            {
                return field.GetValue(this) as string;
            }

            return null;
        }

        public string GetBigText()
        {
            if (bigText == null || bigText.Length == 0)
                return string.Empty;

            return string.Join("\n", bigText);
        }

    }
}