namespace LibTotModMaker.Game.Design
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using QuickType;

    public class TotGameDesignDao
    {
        public static TotGameDesignInfo LoadFromFile(string path)
        {
            var jsonString = File.ReadAllText(path);
            var totGameDesign = TotGameDesignBase.FromJson(jsonString);
            var designInfo = new TotGameDesignInfo();

            foreach (var design in totGameDesign)
            {
                ReadItems(designInfo, TotGameDesignEntry.NEntryType.Dialogs, design.Dialogs);
                ReadItems(designInfo, TotGameDesignEntry.NEntryType.InfoEvents, design.InfoEvents);
                ReadItems(designInfo, TotGameDesignEntry.NEntryType.Journals, design.Journals);
                ReadItems(designInfo, TotGameDesignEntry.NEntryType.Maps, design.Maps);
                ReadItems(designInfo, TotGameDesignEntry.NEntryType.Messages, design.Messages);
                ReadItems(designInfo, TotGameDesignEntry.NEntryType.Quests, design.Quests);
                ReadItems(designInfo, TotGameDesignEntry.NEntryType.TutrialTips, design.TutorialTips);

                //// o Phrases: 話者情報
                //// x design.Items: データなし
                //// x JournalEntries: データはあるが必要な情報なし。
                //// x ResourceRewards: データはあるが必要な情報なし。
                //// o TaskTemplates: Questの子情報
                if (design.Phrases != null)
                {
                    foreach (var phrase in design.Phrases)
                    {
                        var entry = designInfo.GetEntry(design.Id);
                        entry.AddPhraseEntry(phrase.Id, phrase.SpeakerType, phrase.CharacterKey);
                    }
                }

                if (design.TaskTemplates != null)
                {
                    foreach (var subQuestID in design.TaskTemplates)
                    {
                        var entry = designInfo.GetEntry(design.Id);
                        entry.AddSubQuest(subQuestID);
                    }
                }
            }

            return designInfo;
        }

        private static void ReadItems(TotGameDesignInfo designInfo, TotGameDesignEntry.NEntryType entryType, List<string> items)
        {
            if (items == null)
            {
                return;
            }

            foreach (var item in items)
            {
                designInfo.AddEntry(entryType, item);
            }
        }
    }
}
