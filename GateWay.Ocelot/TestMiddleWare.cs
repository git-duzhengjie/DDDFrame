public class TestMiddleWare(RequestDelegate next)
{
    private readonly RequestDelegate next = next;

    public async Task Invoke(HttpContext context)
    {
        Console.WriteLine(context.Request.Host.Host);
        await Task.CompletedTask;
    }
}