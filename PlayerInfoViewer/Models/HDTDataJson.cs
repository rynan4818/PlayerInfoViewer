using Newtonsoft.Json;
using System.IO;
using System;
using UnityEngine;

namespace PlayerInfoViewer.Models
{
    public class HDTDataJson
    {
        public float HeadDistanceTravelled { get; set; }
        public DateTime lastWriteTime;
        public bool hdtEnable = false;
        public void Load()
        {
            var filePath = Path.Combine(Application.persistentDataPath, "HMDDistance.dat");
            if (!File.Exists(filePath))
                return;
            var fileLastWriteTime = File.GetLastWriteTime(filePath);
            if (lastWriteTime == fileLastWriteTime)
                return;
            lastWriteTime = fileLastWriteTime;
            try
            {
                var text = File.ReadAllText(filePath);
                var data = JsonConvert.DeserializeObject<HDTDataJson>(text);
                this.HeadDistanceTravelled = data.HeadDistanceTravelled;
                hdtEnable = true;
            }
            catch (Exception e)
            {
                hdtEnable = false;
                Plugin.Log.Error(e);
            }
        }
    }
}
