namespace QTodo
{
    public interface ITodoEvent
    {

    }

    public class AddTodoEvent:ITodoEvent
    {
        public DateTimeOffset CreationDate { get; set; }
        public Guid Id { get; internal set; }
    }

    public class UpdateTodoEvent : ITodoEvent
    {
        public DateTimeOffset UpdateDate { get; set; }
        public Guid ItemId { get; set; }
    }

    public class DeleteTodoEvent : ITodoEvent
    {
    }
}
