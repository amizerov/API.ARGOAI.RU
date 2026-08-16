#region initapp

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin() // Разрешить запросы с любых источников
              .AllowAnyHeader() // Разрешить любые заголовки
              .AllowAnyMethod(); // Разрешить любые HTTP-методы
    });
});

// Добавление Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Включение CORS
app.UseCors("AllowAll");

// Включение Swagger в режиме разработки
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
#endregion

#region mapping
    // API метод для получения фич
    app.MapGet("/features", async () =>
    {
        var res = await Features.LoadFromDb();
        return Results.Json(res);
    })
    .WithName("GetFeatures") // Добавление имени для Swagger
    .WithOpenApi(); // Генерация документации для этого метода

    app.MapPost("/chat", async (ChatRequest request) =>
    {
        try
        {
            var res = await request.GetAiAnswer();
            return Results.Json(res);              // 200 OK
        }
        catch (Exception ex)
        {
            // читаемое сообщение клиенту, но не 500
            return Results.BadRequest(new
            {
                status = "error",
                message = ex.Message
            });
        }
    })
    .WithName("PostChatAnswer")
    .WithOpenApi();

    app.MapGet("/chat_face", () =>
    {
        var imagePath = AmSecrets.Secrets.PathToChatFace;
        if (!File.Exists(imagePath))
        {
            return Results.NotFound(new { message = "Image not found" });
        }

        return Results.File(imagePath, "image/jpeg");
    })
    .WithName("GetFaceImage")
    .WithOpenApi();

    app.MapGet("/chat_title", () =>
    {
        var chatTitle = AmSecrets.Secrets.ChatTitle;
        return Results.Text(chatTitle, "text/plain");
    })
    .WithName("GetChatTitle")
    .WithOpenApi();

// API метод для отправки сообщения по почте
app.MapPost("/mail", async (MailRequest formData) =>
        {
            var res = await formData.SendMail();
            return Results.Json(res);
        })
    .WithName("SendMail")
    .WithOpenApi();
#endregion

app.Run();