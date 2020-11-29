using System;
using System.Text;

namespace HangulLib
{
    public class Hangul
    {
        private ushort HANGUL_START = 0xAC00;
        private ushort HANGUL_END   = 0xD79F;
        private readonly string chosung = "ㄱㄲㄴㄷㄸㄹㅁㅂㅃㅅㅆㅇㅈㅉㅊㅋㅌㅍㅎ";
        private readonly string jungsung = "ㅏㅐㅑㅒㅓㅔㅕㅖㅗㅘㅙㅚㅛㅜㅝㅞㅟㅠㅡㅢㅣ";
        private readonly string jongsung = " ㄱㄲㄳㄴㄵㄶㄷㄹㄺㄻㄼㄽㄾㄿㅀㅁㅂㅄㅅㅆㅇㅈㅊㅋㅌㅍㅎ";
        
        public string ReplaceHangulSeparated(string msg)
        {
            StringBuilder repString = new StringBuilder();
            foreach (char c in msg)
            {
                ushort code = Convert.ToUInt16(c);
                if(code < HANGUL_START || code > HANGUL_END)
                {
                    repString.Append(c);
                    continue;
                }
                int iCode = code - HANGUL_START;

                int Cjongsung = iCode % 28;
                iCode = (iCode - Cjongsung) / 28;

                int Cjungsung = iCode % 21;
                iCode = (iCode - Cjungsung) / 21;

                int Cchosung = iCode;
                repString.Append(chosung[Cchosung]);
                repString.Append(jungsung[Cjungsung]);
                if (Cjongsung != 0) repString.Append(jongsung[Cjongsung]);
            }
            return repString.ToString();
        }
    }
}
