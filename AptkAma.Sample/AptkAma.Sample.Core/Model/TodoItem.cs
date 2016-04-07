using Aptk.Plugins.AptkAma.Data;

namespace AptkAma.Sample.Core.Model
{
    public class TodoItem : EntityData
    {
        public string Text { get; set; }
        public bool Complete { get; set; }
    }
}
