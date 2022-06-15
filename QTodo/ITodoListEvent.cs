namespace QTodo
{
    public interface ITodoListEvent { }

    public class ItemCreatedEvent : ITodoListEvent
    {
        public Guid OriginalEventId { get; internal set; }
        public Guid ItemId { get; internal set; }
    }

    public class ItemUpdatedEvent : ITodoListEvent
    {

    }
}
