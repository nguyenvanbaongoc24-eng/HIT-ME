using System;
using UnityEngine;

namespace HitMe.UI
{
    public static class FoundationFonts
    {
        public static Font Text => Resources.Load<Font>("Fonts/Nunito");
        public static Font Symbols => Resources.Load<Font>("Fonts/NotoSansSymbols2-Regular");
        public static void Validate()
        {
            if (Text == null || Symbols == null) throw new InvalidOperationException("Missing bundled OFL fonts.");
            foreach (char ch in "SẴN SÀNGĐẤUƯỜƠẠỆ")
                if (!Text.HasCharacter(ch)) throw new InvalidOperationException("Missing Vietnamese glyph U+" + ((int)ch).ToString("X4"));
            if (!Symbols.HasCharacter('♥')) throw new InvalidOperationException("Missing health symbol.");
        }
    }
}
