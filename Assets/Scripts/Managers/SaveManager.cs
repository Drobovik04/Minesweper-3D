using Assets.Scripts.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YG;

namespace Assets.Scripts.Managers
{
    public class SaveManager
    {
        private static string PathToSave => Path.Combine(Application.persistentDataPath, "save.json");

        public void Save(AllData data)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            YG2.saves.allData = data;
            YG2.SaveProgress();

#else
            var json = JsonUtility.ToJson(data);
            File.WriteAllText(PathToSave, json);
#endif


        }

        public AllData Load()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return LoadForWebGL();
#else
            return LoadForAndroidAndEditor();
#endif
        }

        private AllData LoadForWebGL()
        {
            if (YG2.saves.allData != null)
            {
                return YG2.saves.allData;
            }
            else
            {
                Debug.Log("Save file is not exists, using default values");
                return new AllData
                {
                    GameSettings = new GameSettings { Width = 8, Height = 8, Length = 8, Bombs = 10 }
                };
            }

        }

        private AllData LoadForAndroidAndEditor()
        {
            if (!File.Exists(PathToSave))
            {
                Debug.Log("Save file is not exists, using default values");
                return new AllData
                {
                    GameSettings = new GameSettings { Width = 8, Height = 8, Length = 8, Bombs = 10 }
                };
            }

            var json = File.ReadAllText(PathToSave);
            return JsonUtility.FromJson<AllData>(json);
        }
    }
}
