using System;
using System.Collections.Generic;
using UnityEngine;

namespace HitMe.UI
{
    public static class Strings
    {
        [Serializable] public sealed class Entry { public string key; public string value; }
        [Serializable] public sealed class Table { public Entry[] entries; }
        static readonly Dictionary<string, string> values = new Dictionary<string, string>();
        public static string Language { get; private set; } = "vi";
        public static void Load(string language)
        {
            Language = language;
            values.Clear();
            TextAsset asset = Resources.Load<TextAsset>("Localization/" + language);
            if (asset == null) throw new InvalidOperationException("Missing localization: " + language);
            foreach (Entry entry in JsonUtility.FromJson<Table>(asset.text).entries) values[entry.key] = entry.value;
        }
        public static string Get(string key) { return values.TryGetValue(key, out string value) ? value : "[" + key + "]"; }
    }
}
