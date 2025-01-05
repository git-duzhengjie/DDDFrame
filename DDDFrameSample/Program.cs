using DDDFrameSample;
using UniversalRPC.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.UseDefault<DependencyInjection>(args);
var app = builder.Build();
app.UseDefault();
//如果需要开启URPC服务，请取消注释下面的代码
//app.UseURPCService();
app.Run();
