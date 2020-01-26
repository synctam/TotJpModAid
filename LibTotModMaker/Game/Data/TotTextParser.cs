namespace LibTotModMaker.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TotTextParser
    {
        public static (string key, string value) Parse(string text)
        {
            if (!IsValid(text))
            {
                return (null, null);
            }

            bool isValue = false;
            StringBuilder keyBuff = new StringBuilder();
            StringBuilder valueBuff = new StringBuilder();
            foreach (var c in text)
            {
                if (c == '=')
                {
                    if (isValue)
                    {
                        //// value内のイコールは有効データとして扱う。
                        valueBuff.Append(c);
                    }
                    else
                    {
                        isValue = true;
                    }
                }
                else
                {
                    if (isValue)
                    {
                        valueBuff.Append(c);
                    }
                    else
                    {
                        keyBuff.Append(c);
                    }
                }
            }

            return (keyBuff.ToString(), valueBuff.ToString());
        }

        private static bool IsValid(string text)
        {
            if (IsBlankLine(text) || IsComment(text))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private static bool IsBlankLine(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool IsComment(string text)
        {
            if (text.TrimStart().StartsWith("#"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
