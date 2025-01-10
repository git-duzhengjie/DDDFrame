public class TestMiddleWare(RequestDelegate next)
{
    private readonly RequestDelegate next = next;

    public async Task Invoke(HttpContext context)
    {
        await Task.CompletedTask;
    }
}