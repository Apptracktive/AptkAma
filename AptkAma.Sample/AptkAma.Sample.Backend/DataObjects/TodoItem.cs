using Microsoft.Azure.Mobile.Server;

namespace AptkAma.Sample.Backend.DataObjects
{
    public class TodoItem : EntityData
    {
        public string Text { get; set; }

        public bool Complete { get; set; }
    }
}