using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder();

builder.Services.AddAntiforgery();

var app = builder.Build();

app.MapGet("/antiforgery", (HttpContext context, IAntiforgery antiforgery) =>
{
    var token = antiforgery.GetAndStoreTokens(context);
    return Results.Content(MyHtml.html("/todo", token), "text/html");
});

app.MapGet("/non-antiforgery", () =>
{
    Console.WriteLine("nonantiforgery");
    return Results.Content(MyHtml.html("/todo2"), "text/html");
});

app.MapPost("/todo", ([FromForm] Todo todo) => Results.Ok(todo));

app.MapPost("/todo2", ([FromForm] Todo todo) => Results.Ok(todo))
                                                .DisableAntiforgery();

app.Run();

class Todo
{
    public required string Name { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime DueDate { get; set; }
}

public static class MyHtml
{

    public static string html(string action, AntiforgeryTokenSet token = null)
    {
        var forgeryInputField = token is null ?
        "" : generateTokenField(token);
        return $"""
    <html><body>
        <form action="{action}" method="POST" enctype="multipart/form-data">
            {forgeryInputField}
            <input type="text" name="name" />
            <input type="date" name="dueDate" />
            <input type="checkbox" name="isCompleted" />
            <input type="submit" />
        </form>
    </body></html>
""";
    }

    public static string generateTokenField(AntiforgeryTokenSet token) => $"""
        <input name="{token.FormFieldName}" 
                      type="hidden" value="{token.RequestToken}" />
    """;
}

