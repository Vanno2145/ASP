using Microsoft.AspNetCore.Http;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using System.Xml.Linq;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseStaticFiles();

app.Run(async context =>
{
    var responce = context.Response;
    var request = context.Request;

    string len = request.Query["api"];
    int lengthApi = 0;

    if (!string.IsNullOrEmpty(len))
    {
        lengthApi = len.Length;
    }

    await responce.WriteAsync($"{lengthApi}");

});

app.Run();
