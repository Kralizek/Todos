using System.Net.Http.Json;

using AutoFixture.NUnit3;

using Microsoft.EntityFrameworkCore;

using Todos;
using Todos.Todo;

namespace API.Integration.Tests.Todo;

[TestFixture]
[TestOf(typeof(Endpoints))]
public class EndpointsTests
{
    [TestOf(nameof(Endpoints.List))]
    public class List : TestsWithBackend
    {
        [Test]
        [AutoDataProvider]
        public async Task Returns_existing_items(HttpClient http, AppDbContext db, TodoItem item)
        {
            db.Todos.Add(item);

            await db.SaveChangesAsync();

            var items = await http.GetFromJsonAsync<TodoItem[]>("todos");

            Assert.Multiple(() =>
            {
                Assert.That(items, Has.Exactly(1).InstanceOf<TodoItem>());

                var todo = items!.First();

                Assert.That(todo.Id, Is.EqualTo(item.Id));
                Assert.That(todo.Title, Is.EqualTo(item.Title));
                Assert.That(todo.Description, Is.EqualTo(item.Description));
                Assert.That(todo.IsComplete, Is.EqualTo(item.IsComplete));
                Assert.That(todo.Priority, Is.EqualTo(item.Priority));

                Assert.That(todo, Is.Not.SameAs(item));
            });
        }

        [Test]
        [AutoDataProvider]
        public async Task Returns_items_matching_priority(HttpClient http, AppDbContext db, TodoItem item)
        {
            db.Todos.Add(item);

            await db.SaveChangesAsync();

            var items = await http.GetFromJsonAsync<TodoItem[]>($"todos?priority={item.Priority}");

            Assert.Multiple(() =>
            {
                Assert.That(items, Has.Exactly(1).InstanceOf<TodoItem>());

                var todo = items!.First();

                Assert.That(todo.Id, Is.EqualTo(item.Id));
                Assert.That(todo.Title, Is.EqualTo(item.Title));
                Assert.That(todo.Description, Is.EqualTo(item.Description));
                Assert.That(todo.IsComplete, Is.EqualTo(item.IsComplete));
                Assert.That(todo.Priority, Is.EqualTo(item.Priority));

                Assert.That(todo, Is.Not.SameAs(item));
            });
        }
    }

    [TestOf(nameof(Endpoints.Create))]
    public class Create : TestsWithBackend
    {
        [Test]
        [AutoDataProvider]
        public async Task Should_return_created_item(HttpClient http, TodoItem newItem)
        {
            newItem.Id = Guid.Empty;

            var response = await http.PostAsJsonAsync("todos", newItem);

            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.Created));

            var resultItem = await response.Content.ReadFromJsonAsync<TodoItem>();

            Assert.Multiple(() =>
            {
                Assert.That(resultItem, Is.Not.Null);
                Assert.That(resultItem!.Id, Is.Not.EqualTo(Guid.Empty));
                Assert.That(resultItem.Title, Is.EqualTo(newItem.Title));
                Assert.That(resultItem.Description, Is.EqualTo(newItem.Description));
                Assert.That(resultItem.IsComplete, Is.EqualTo(newItem.IsComplete));
                Assert.That(resultItem.Priority, Is.EqualTo(newItem.Priority));
            });
        }

        [Test]
        [AutoDataProvider]
        public async Task Should_add_item_to_database(AppDbContext db, HttpClient http, TodoItem newItem)
        {
            newItem.Id = Guid.Empty;

            var response = await http.PostAsJsonAsync("todos", newItem);

            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.Created));

            var resultItem = await response.Content.ReadFromJsonAsync<TodoItem>();

            var databaseItem = await db.Todos.FindAsync(resultItem!.Id);

            Assert.Multiple(() =>
            {
                Assert.That(databaseItem, Is.Not.Null);
                Assert.That(databaseItem!.Id, Is.EqualTo(resultItem.Id));
                Assert.That(databaseItem.Title, Is.EqualTo(newItem.Title));
                Assert.That(databaseItem.Description, Is.EqualTo(newItem.Description));
                Assert.That(databaseItem.IsComplete, Is.EqualTo(newItem.IsComplete));
                Assert.That(databaseItem.Priority, Is.EqualTo(newItem.Priority));
            });
        }
    }

    [TestOf(nameof(Endpoints.Get))]
    public class Get : TestsWithBackend
    {
        [Test]
        [AutoDataProvider]
        public async Task Should_return_Ok_with_item(HttpClient http, AppDbContext db, TodoItem item)
        {
            db.Todos.Add(item);
            await db.SaveChangesAsync();

            var response = await http.GetAsync($"todos/{item.Id}");

            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));

            var resultItem = await response.Content.ReadFromJsonAsync<TodoItem>();

            Assert.Multiple(() =>
            {
                Assert.That(resultItem, Is.Not.Null);
                Assert.That(resultItem!.Id, Is.EqualTo(item.Id));
                Assert.That(resultItem.Title, Is.EqualTo(item.Title));
                Assert.That(resultItem.Description, Is.EqualTo(item.Description));
                Assert.That(resultItem.IsComplete, Is.EqualTo(item.IsComplete));
                Assert.That(resultItem.Priority, Is.EqualTo(item.Priority));
            });
        }
    }

    [TestOf(nameof(Endpoints.Save))]
    public class Save : TestsWithBackend
    {
        [Test]
        [AutoDataProvider]
        public async Task Should_return_Ok_with_updated_item(HttpClient http, AppDbContext db, TodoItem item, TodoItem updatedItem)
        {
            db.Todos.Add(item);
            await db.SaveChangesAsync();
            
            updatedItem.Id = item.Id;

            var response = await http.PutAsJsonAsync($"todos/{item.Id}", updatedItem);

            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));

            var resultItem = await response.Content.ReadFromJsonAsync<TodoItem>();

            Assert.Multiple(() =>
            {
                Assert.That(resultItem, Is.Not.Null);
                Assert.That(resultItem!.Id, Is.EqualTo(item.Id));
                Assert.That(resultItem.Title, Is.EqualTo(updatedItem.Title));
                Assert.That(resultItem.Description, Is.EqualTo(updatedItem.Description));
                Assert.That(resultItem.IsComplete, Is.EqualTo(updatedItem.IsComplete));
                Assert.That(resultItem.Priority, Is.EqualTo(updatedItem.Priority));
            });
        }
    }
    
    [TestOf(nameof(Endpoints.Delete))]
    public class Delete : TestsWithBackend
    {
        [Test]
        [AutoDataProvider]
        public async Task Should_return_Ok(HttpClient http, AppDbContext db, TodoItem item)
        {
            db.Todos.Add(item);
            await db.SaveChangesAsync();

            var response = await http.DeleteAsync($"todos/{item.Id}");

            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        }

        [Test]
        [AutoDataProvider]
        public async Task Should_remove_item_from_database(HttpClient http, AppDbContext db, TodoItem item)
        {
            db.Todos.Add(item);
            await db.SaveChangesAsync();

            var response = await http.DeleteAsync($"todos/{item.Id}");

            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));

            var databaseItem = await db.Todos
                .AsNoTracking()
                .SingleOrDefaultAsync(i => i.Id == item.Id);

            Assert.That(databaseItem, Is.Null);
        }
    }

}