using DDDFrameSample;
using UniversalRPC.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.UseDefault<DependencyInjection>(args);
var app = builder.Build();
app.UseDefault();
//�����Ҫ����URPC������ȡ��ע������Ĵ���
//app.UseURPCService();
app.Run();
