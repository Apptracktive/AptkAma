using Microsoft.WindowsAzure.MobileServices.Files.Sync.Triggers;
using Newtonsoft.Json;

namespace Aptk.Plugins.AptkAma.Data
{
    public abstract class FileSyncEntityData : EntityData, IFileSyncTableData
    {
        /// <summary>
        /// [Optional] A trigger to manage this specific table file sync
        /// </summary>
        [JsonIgnore]
        public virtual IFileSyncTriggerFactory SpecificFileSyncTriggerFactory { get; }
    }
}