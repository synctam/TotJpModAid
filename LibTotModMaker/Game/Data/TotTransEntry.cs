namespace LibTotModMaker.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TotTransEntry
    {
        public TotTransEntry(string key, string text)
        {
            this.Key = key;
            this.Text = text;
        }

        public string Key { get; } = string.Empty;

        public string Text { get; } = string.Empty;

        public override string ToString()
        {
            return $"Key({this.Key}) Text({this.Text})";
        }
    }
}
