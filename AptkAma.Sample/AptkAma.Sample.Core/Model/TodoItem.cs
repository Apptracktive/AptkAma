using Aptk.Plugins.AptkAma.Data;

namespace AptkAma.Sample.Core.Model
{
    // Could inherit from EntityData if you don't need any file sync on this table
    public class TodoItem : FileSyncEntityData
    {
        public string Text { get; set; }
        public bool Complete { get; set; }

        // If you want to set specific file sync trigger strategy for this table
        //public override IFileSyncTriggerFactory SpecificFileSyncTriggerFactory
        //{
        //    get { return new FileSyncTriggerFactory(); }
        //}
    }
}
