namespace QTodo
{
    public class TodoItem
    {
        public DateTimeOffset CreationDate { get; set; }
        public Guid Id { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}