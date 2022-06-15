using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace QTodo
{
    public class TodoList
    {
        List<TodoItem> _items = new List<TodoItem>();
        Subject<ITodoListEvent> _outGoingEventStream = new Subject<ITodoListEvent>();

        public IObservable<ITodoListEvent> EventStream => _outGoingEventStream;

        public void Apply(IEnumerable<ITodoEvent> tasks)
        {
            foreach (var task in tasks)
            {
                switch (task)
                {
                    case AddTodoEvent addEvent:
                        var item = new TodoItem
                        {
                            CreationDate = addEvent.CreationDate,
                            UpdatedDate = addEvent.CreationDate,
                            Id = Guid.NewGuid()
                        };
                        _items.Add(item);
                        _outGoingEventStream.OnNext(new ItemCreatedEvent
                        {
                            OriginalEventId = addEvent.Id,
                            ItemId = item.Id
                        });
                        break;
                    case UpdateTodoEvent updateEvent:
                        var updateItem = _items.Where(i => i.Id == updateEvent.ItemId).Single();
                        updateItem.UpdatedDate = updateEvent.UpdateDate;
                        break;
                }
            }
        }

        public IEnumerable<TodoItem> GetItems()
        {
            return _items;
        }
    }
}
