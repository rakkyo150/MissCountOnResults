﻿using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TMPro;

namespace MissCountOnResults.HarmonyPatches
{
    [HarmonyPatch(typeof(ResultsViewController))]
    [HarmonyPatch("SetDataToUI", MethodType.Normal)]
    class ResultsViewPatches : ResultsViewController
    {
        static void Postfix(IDifficultyBeatmap ____difficultyBeatmap, LevelCompletionResults ____levelCompletionResults, ref TextMeshProUGUI ____goodCutsPercentageText)
        {
            if (PluginConfig.Instance.Enable)
            {
                IBeatmapDataBasicInfo GetBeatmapDataTaskResult=____difficultyBeatmap.GetBeatmapDataBasicInfoAsync().Result;
                int cuttableNotesCount = GetBeatmapDataTaskResult.cuttableNotesCount;
                int thisPlayMissCount = ____levelCompletionResults.missedCount + ____levelCompletionResults.badCutsCount;
                bool fullCombo = ____levelCompletionResults.fullCombo;
                string thisPlayMissCountStr;
                string comboColor;
                var dataHistory = new List<MissCountOnResultsMethods.Record>();
                string BestMissCountInHistory = "?";
                int missCountDifference = 0;
                string goodCutsText = "";
                string goodCutsColor = "";

                string colorPositive = "#00B300";
                string colorNegative = "#FF0000";
                string colorNeutral = "#FFFFFF";


                if (fullCombo)
                {
                    thisPlayMissCountStr = "FC";
                    comboColor = "#E6B422";
                }
                else
                {
                    thisPlayMissCountStr = thisPlayMissCount.ToString() + "<size=40%>miss";
                    comboColor = "#FFFFFF";
                }

                // This list includes the newest play data
                dataHistory = MissCountOnResultsMethods.GetRecords(____difficultyBeatmap);

                if (dataHistory.Count() == 0)
                {
                    Plugin.Log?.Debug("Play Cancelled? or No SongPlayHistory");
                    goodCutsText = $"<line-height=27.5%><size=80%><color={comboColor}>" + thisPlayMissCountStr
                            + "</color><size=45%>" + "/" + cuttableNotesCount.ToString();
                }
                else if (dataHistory.Count() == 1)
                {
                    Plugin.Log?.Debug("First Play");
                    goodCutsText = $"<line-height=27.5%><size=80%><color={comboColor}>" + thisPlayMissCountStr
                            + "</color><size=45%>" + "/" + cuttableNotesCount.ToString();
                }
                else if (dataHistory.Count() >= 2)
                {
                    // Remove the newest play data
                    dataHistory = dataHistory.OrderByDescending(s => s.Date).ToList();
                    dataHistory.RemoveAt(0);
                    dataHistory = dataHistory.OrderByDescending(s => s.ModifiedScore).ToList();

                    if (dataHistory.First().Miss == "FC")
                    {
                        BestMissCountInHistory = "0";
                    }
                    else
                    {
                        BestMissCountInHistory = dataHistory.First().Miss;
                    }

                    if (BestMissCountInHistory != "?")
                    {
                        Plugin.Log.Debug(BestMissCountInHistory);
                        missCountDifference = thisPlayMissCount - Int32.Parse(BestMissCountInHistory);


                        if (missCountDifference == 0) goodCutsColor = colorNeutral;
                        else if (missCountDifference > 0) goodCutsColor = colorNegative;
                        else if (missCountDifference < 0) goodCutsColor = colorPositive;

                        if (missCountDifference > 0)
                        {
                            goodCutsText = $"<line-height=27.5%><size=80%><color={comboColor}>" + thisPlayMissCountStr
                                + "</color><size=60%>" + "(" + "<color=" + goodCutsColor + ">" + "+" + missCountDifference.ToString()
                                + "</color>" + ")" + "<size=45%>" + "/" + cuttableNotesCount.ToString();
                        }
                        else
                        {
                            goodCutsText = $"<line-height=27.5%><size=80%><color={comboColor}>" + thisPlayMissCountStr
                                + "</color><size=60%>" + "(" + "<color=" + goodCutsColor + ">" + missCountDifference.ToString()
                                + "</color>" + ")" + "<size=45%>" + "/" + cuttableNotesCount.ToString();
                        }
                    }
                    else
                    {
                        goodCutsText = $"<line-height=27.5%><size=80%><color={comboColor}>" + thisPlayMissCountStr
                                + "</color><size=45%>" + "/" + cuttableNotesCount.ToString();
                    }
                }           
                
                ____goodCutsPercentageText.text = goodCutsText;
            }
        }
    }
}
