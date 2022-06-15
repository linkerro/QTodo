using QTodo;
using FluentAssertions;
using System.Reactive.Linq;

namespace QTodoSpecs
{
    public class TodoListSpecs
    {
        AddTodoEvent[] defaultTasks = new AddTodoEvent[]
        {
            new AddTodoEvent()
            {
                CreationDate = DateTimeOffset.Now.Subtract(TimeSpan.FromMinutes(5))
            },
            new AddTodoEvent()
            {
                CreationDate = DateTimeOffset.Now.Subtract(TimeSpan.FromMinutes(4))
            }
        };

        [Fact]
        public void ShouldAcceptTaskAddEvents()
        {
            var todoList = new TodoList();
            var action = () => todoList.Apply(defaultTasks);

            action
                .Should()
                .NotThrow();
        }

        [Fact]
        public void ShouldEmitItemCreationEvents()
        {
            var todoList = new TodoList();
            var counter = 0;
            todoList.EventStream.Subscribe(x => counter += 1);
            todoList.Apply(defaultTasks);

            counter
                .Should()
                .Be(2);
        }

        [Fact]
        public void ShouldListCorrectAmmountOfItems()
        {
            var todoList = new TodoList();
            todoList.Apply(defaultTasks);

            todoList.GetItems().Count()
                .Should()
                .Be(2);
        }

        [Fact]
        public void ShouldListItemsWithIds()
        {
            var todoList = new TodoList();
            todoList.Apply(defaultTasks);

            todoList.GetItems().Where(x => x.Id != Guid.Empty).Count()
                .Should()
                .Be(2);
        }

        [Fact]
        public void ShouldListItemsWithCorrectCreationDate()
        {
            var todoList = new TodoList();
            var defaultEvent = defaultTasks.First();
            todoList.Apply(new AddTodoEvent[] { defaultEvent });

            todoList.GetItems().First()
                .CreationDate
                .Should()
                .Be(defaultEvent.CreationDate);
        }

        [Fact]
        public void ShouldAcceptUpdateEvents()
        {
            var todoList = new TodoList();
            var itemIds = new List<Guid>();
            todoList.EventStream
                .Where(x => x is ItemCreatedEvent)
                .Subscribe(x => itemIds.Add(x.As<ItemCreatedEvent>().ItemId));
            todoList.Apply(defaultTasks);

            var action = () => todoList.Apply(GetDefaultUpdateEvents(itemIds));

            action.Should().NotThrow();
        }

        [Fact]
        public void ShouldUpdateTheCorrectItems()
        {
            var todoList = new TodoList();
            var itemGuids = new List<Guid>();
            todoList.EventStream
                .Where(x => x is ItemCreatedEvent)
                .Subscribe(x => itemGuids.Add(x.As<ItemCreatedEvent>().ItemId));

            todoList.Apply(defaultTasks);

            todoList.Apply(GetDefaultUpdateEvents(itemGuids));

            var result = todoList.GetItems();

            result.Where(x => x.CreationDate == x.UpdatedDate).Count()
                .Should()
                .Be(0);
        }

        [Fact]
        public void ShouldThrowIfUpdateEventHasBadItemId()
        {
            var todoList = new TodoList();
            todoList.Apply(defaultTasks);
            var action = () => todoList.Apply(new ITodoEvent[] { new UpdateTodoEvent { ItemId = Guid.NewGuid() } });
            action.Should().Throw<Exception>();
        }

        [Fact]
        public void ShouldAcceptDeleteEvents()
        {
            var todoList = new TodoList();
            todoList.Apply(defaultTasks);
            var action = () => todoList.Apply(new ITodoEvent[] { new DeleteTodoEvent() });

            action.Should().NotThrow();
        }

        private IEnumerable<UpdateTodoEvent> GetDefaultUpdateEvents(IEnumerable<Guid> itemGuids)
        {
            return itemGuids
                .Select(g => new UpdateTodoEvent
                {
                    ItemId = g,
                    UpdateDate = DateTimeOffset.Now
                }).ToArray();
        }
    }
}