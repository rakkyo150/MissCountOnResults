using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MissCountOnResults
{
    class MissCountOnResultsMethods
    {
        internal class Record
        {
            public long Date = 0L;
            public int ModifiedScore = 0;
            public int RawScore = 0;
            public int LastNote = 0;
            public int Param = 0;
            public string Miss = "?";
        }

        public static readonly string DataFile= Path.Combine(Environment.CurrentDirectory, "UserData", "SongPlayData.json");
        public static Dictionary<string, IList<Record>> Records { get; set; } = new Dictionary<string, IList<Record>>();

        public static void InitializeRecords()
            {
                if (!File.Exists(DataFile))
                {
                    return;
                }

                // Read records from a data file.
                var text = File.ReadAllText(DataFile);
                try
                {
                    Records = JsonConvert.DeserializeObject<Dictionary<string, IList<Record>>>(text);
                    if (Records == null)
                    {
                        throw new JsonReaderException("Unable to deserialize an empty JSON string.");
                    }
                }
                catch (JsonException ex)
                {
                    // The data file is corrupted.
                    Plugin.Log?.Error(ex.ToString());

                    // Try to restore from a backup.
                    var backup = new FileInfo(Path.ChangeExtension(DataFile, ".bak"));
                    if (backup.Exists && backup.Length > 0)
                    {
                        Plugin.Log?.Info("Restoring from a backup...");
                        text = File.ReadAllText(backup.FullName);

                        Records = JsonConvert.DeserializeObject<Dictionary<string, IList<Record>>>(text);
                        if (Records == null)
                        {
                            // Fail hard to prevent overwriting any previous data or breaking the game.
                            throw new Exception("Failed to restore data.");
                        }
                    }
                    else
                    {
                        // There's nothing more we can try.
                        Records = new Dictionary<string, IList<Record>>();
                    }
                }
            }
        public static void ScanNewJson()
            {
                if (!File.Exists(DataFile))
                {
                    Plugin.Log?.Info("No json file");
                    return;
                }

                // Read records from a data file.
                var text = File.ReadAllText(DataFile);
                try
                {
                    Records = JsonConvert.DeserializeObject<Dictionary<string, IList<Record>>>(text);
                    if (Records == null)
                    {
                        throw new JsonReaderException("Unable to deserialize an empty JSON string.");
                    }
                }
                catch
                {
                    Plugin.Log?.Info("Failed to scan the new data");
                    return;
                }
            }
        public static List<Record> GetRecords(IDifficultyBeatmap beatmap)
            {
                var config = PluginConfig.Instance;
                var beatmapCharacteristicName = beatmap.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName;
                var difficulty = $"{beatmap.level.levelID}___{(int)beatmap.difficulty}___{beatmapCharacteristicName}";

                if (Records.TryGetValue(difficulty, out IList<Record> records))
                {
                    return records.ToList();
                }

                return new List<Record>();
            }
    }
}
