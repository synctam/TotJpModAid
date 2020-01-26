namespace LibTotModMaker.TransSheet
{
    /// <summary>
    /// 翻訳シートエントリーの基底クラス
    /// </summary>
    public abstract class TotTransSheetEntryBase
    {
        public TotTransSheetEntryBase() { }

        public TotTransSheetEntryBase(string key, string text)
        {
            this.Key = key;
            this.English = text;
        }

        public string FileID { get; set; }

        public string Key { get; set; } = string.Empty;

        public string English { get; set; }

        public string Japanese { get; set; }

        public string MachineTranslation { get; set; }

        public string Translate(string text, bool useMTrans)
        {
            //// en | jp | mt | result
            ////  y |  y |  y | 1 jp
            ////  y |  y |  n | 2 jp
            ////  y |  n |  y | 3 mt
            ////  y |  n |  n | 4 en
            ////  n |  y |  y | 5 en
            ////  n |  y |  n | 6 en
            ////  n |  n |  y | 7 en
            ////  n |  n |  n | 8 en
            var en = !string.IsNullOrEmpty(text);
            var jp = !string.IsNullOrEmpty(this.Japanese);
            var mt = !string.IsNullOrEmpty(this.MachineTranslation);
            if (en && jp && mt) //// 1
            {
                return this.Japanese;
            }
            else if (en && jp && !mt) //// 2
            {
                return this.Japanese;
            }
            else if (en && !jp && mt) //// 3
            {
                if (this.IsContainsAttributes(text))
                {
                    //// タグや置換文字がある場合は、トラブル防止のため原文を返す。
                    return text;
                }
                else
                {
                    if (useMTrans)
                    {
                        //// 機械翻訳を返す。
                        return this.MachineTranslation;
                    }
                    else
                    {
                        return text;
                    }
                }
            }
            else
            {
                return text;
            }
        }

        private bool IsContainsAttributes(string text)
        {
            //// {0} {1}
            //// [+] [o]
            //// <br> <style="gold"> <newline>
            if (text.Contains("{") && text.Contains("}"))
            {
                return true;
            }

            if (text.Contains("<") && text.Contains(">"))
            {
                return true;
            }

            if (text.Contains("[") && text.Contains("]"))
            {
                return true;
            }

            if (this.Key.Contains("_key_code"))
            {
                return true;
            }

            return false;
        }
    }
}
