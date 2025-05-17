using Aspire.Hosting;
using Aspire.Hosting.Testing;

namespace EndToEnd.Tests;

[TestFixture]
public class HomePageTests : PageTest
{
    [Test]
    [AutoDataProvider]
    public async Task HomePage_is_loaded(DistributedApplication app)
    {
        var baseAddress = app.GetEndpoint("web");

        await Page.GotoAsync(baseAddress.ToString());

        await Expect(Page.Locator(".add-todo")).ToBeVisibleAsync();
        
        await Expect(Page.Locator(".add-todo input[name='title']")).ToBeVisibleAsync();
        await Expect(Page.Locator(".add-todo textarea[name='description']")).ToBeVisibleAsync();
        await Expect(Page.Locator(".add-todo input[name='priority']")).ToBeVisibleAsync();
        
        await Expect(Page.Locator(".add-todo button[type='submit']")).ToBeVisibleAsync();
    }
    
    [Test]
    [AutoDataProvider]
    public async Task AddTodo_creates_new_todo_item(DistributedApplication app, ToDoItem item)
    {
        var baseAddress = app.GetEndpoint("web");

        await Page.GotoAsync(baseAddress.ToString());

        const int priority = 3;

        await Page.FillAsync("input[name='title']", item.Title);
        await Page.FillAsync("textarea[name='description']", item.Description);
        await Page.FillAsync("input[name='priority']", priority.ToString());

        await Page.ClickAsync("button[type='submit']");

        var todoTitle = Page.Locator(".todo-card h3", new() { HasTextString = item.Title });
        await Expect(todoTitle).ToBeVisibleAsync();
        
        var newTodo = todoTitle.Locator("..").Locator("..");
        
        await Expect(newTodo).ToBeVisibleAsync();
        await Expect(newTodo.Locator("p").Filter(new() { HasTextString = item.Description })).ToBeVisibleAsync();
        await Expect(newTodo.Locator(".priority")).ToContainTextAsync($"Priority: {priority}");
    }
}

public record ToDoItem(string Title, string Description);