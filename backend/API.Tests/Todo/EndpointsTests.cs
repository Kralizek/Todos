using Microsoft.AspNetCore.Http.HttpResults;

using Todos.Todo;

namespace Tests.Todo;

[TestFixture]
[TestOf(typeof(Endpoints))]
public class EndpointsTests
{
    [TestOf(nameof(Endpoints.List))]
    public class List
    {
        [Test, AutoDataProvider]
        public async Task Should_return_all_items(ITodoRepository repository, TodoItem[] items)
        {
            A.CallTo(() => repository.ListAsync()).Returns(items.ToAsyncEnumerable());

            var resultItems = await Endpoints.List(repository).ToArrayAsync();
            
            Assert.That(resultItems, Is.EquivalentTo(items));
        }
    }

    [TestOf(nameof(Endpoints.Create))]
    public class Create
    {
        [Test, AutoDataProvider]
        public async Task Should_add_item_to_database(ITodoRepository repository, TodoItem newItem)
        {
            newItem.Id = Guid.Empty;

            A.CallTo(() => repository.AddAsync(newItem, A<CancellationToken>.Ignored)).Returns(newItem);
            
            _ = await Endpoints.Create(newItem, repository, default);
            
            A.CallTo(() => repository.AddAsync(newItem, A<CancellationToken>.Ignored)).MustHaveHappened();
        }
        
        [Test, AutoDataProvider]
        public async Task Should_return_Created(ITodoRepository repository, TodoItem newItem)
        {
            newItem.Id = Guid.Empty;
            
            A.CallTo(() => repository.AddAsync(newItem, A<CancellationToken>.Ignored)).Returns(newItem);
            
            var result = await Endpoints.Create(newItem, repository, default);
            
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.InstanceOf<Created<TodoItem>>());
                
                var resultItem = (result.Result as Created<TodoItem>)?.Value ?? throw new Exception();
                
                Assert.That(resultItem.Description, Is.EqualTo(newItem.Description));
                Assert.That(resultItem.IsComplete, Is.EqualTo(newItem.IsComplete));
                Assert.That(resultItem.Priority, Is.EqualTo(newItem.Priority));
                Assert.That(resultItem.Title, Is.EqualTo(newItem.Title));
            });
        }
    }

    [TestOf(nameof(Endpoints.Get))]
    public class Get
    {
        [Test, AutoDataProvider]
        public async Task Should_return_Ok_with_item(ITodoRepository repository, TodoItem item)
        {
            A.CallTo(() => repository.GetAsync(item.Id, A<CancellationToken>.Ignored)).Returns(item);
            
            var result = await Endpoints.Get(item.Id, repository, default);
            
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.InstanceOf<Ok<TodoItem>>());
                
                var resultItem = (result.Result as Ok<TodoItem>)?.Value ?? throw new Exception();
                
                Assert.That(resultItem.Description, Is.EqualTo(item.Description));
                Assert.That(resultItem.IsComplete, Is.EqualTo(item.IsComplete));
                Assert.That(resultItem.Priority, Is.EqualTo(item.Priority));
                Assert.That(resultItem.Title, Is.EqualTo(item.Title));
            });
        }
        
        [Test, AutoDataProvider]
        public async Task Should_fetch_item_from_database(ITodoRepository repository, TodoItem item)
        {
            A.CallTo(() => repository.GetAsync(item.Id, A<CancellationToken>.Ignored)).Returns(item);
            
            _ = await Endpoints.Get(item.Id, repository, default);
            
            A.CallTo(() => repository.GetAsync(item.Id, A<CancellationToken>.Ignored)).MustHaveHappened();
        }
    }

    [TestOf(nameof(Endpoints.Save))]
    public class Save
    {
        [Test, AutoDataProvider]
        public async Task Should_return_Ok_with_item(ITodoRepository repository, TodoItem item)
        {
            var result = await Endpoints.Save(item.Id, item, repository, default);
            
            Assert.Multiple(() =>
            {
                Assert.That(result.Result, Is.InstanceOf<Ok<TodoItem>>());
                
                var resultItem = (result.Result as Ok<TodoItem>)?.Value ?? throw new Exception();
                
                Assert.That(resultItem.Description, Is.EqualTo(item.Description));
                Assert.That(resultItem.IsComplete, Is.EqualTo(item.IsComplete));
                Assert.That(resultItem.Priority, Is.EqualTo(item.Priority));
                Assert.That(resultItem.Title, Is.EqualTo(item.Title));
            });
        }
        
        [Test, AutoDataProvider]
        public async Task Should_update_item_on_database(ITodoRepository repository, TodoItem item)
        {
            _ = await Endpoints.Save(item.Id, item, repository, default);
            
            A.CallTo(() => repository.UpdateAsync(item, A<CancellationToken>.Ignored)).MustHaveHappened();
        }
    }

    [TestOf(nameof(Endpoints.Delete))]
    public class Delete
    {
        [Test, AutoDataProvider]
        public async Task Should_return_Ok_with_item(ITodoRepository repository, TodoItem item)
        {
            var result = await Endpoints.Delete(item.Id, repository, default);
            
            Assert.That(result.Result, Is.InstanceOf<Ok>());
        }
        
        [Test, AutoDataProvider]
        public async Task Should_update_item_on_database(ITodoRepository repository, TodoItem item)
        {
            _ = await Endpoints.Delete(item.Id, repository, default);
            
            A.CallTo(() => repository.DeleteAsync(item.Id, A<CancellationToken>.Ignored)).MustHaveHappened();
        }
    }
}
