using AutocleanManager.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// A hospedagem (Render) informa a porta pela variavel PORT.
// Localmente isso fica vazio e usamos a porta definida no Dockerfile.
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

// Configurar DbContext com PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(ResolverConnectionString(builder.Configuration)));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Aplicar migrations automaticamente
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();

// Em produção o HTTPS é tratado pela hospedagem; o redirecionamento só faz sentido local.
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Servir a interface web (pasta wwwroot)
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.Run();

// Usa a DATABASE_URL (formato postgres://...) quando hospedado;
// caso contrário, a connection string local do appsettings/compose.
static string ResolverConnectionString(IConfiguration config)
{
    var url = Environment.GetEnvironmentVariable("DATABASE_URL");
    if (string.IsNullOrWhiteSpace(url))
    {
        return config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string nao configurada.");
    }

    var uri = new Uri(url);
    var credenciais = uri.UserInfo.Split(':', 2);
    var usuario = Uri.UnescapeDataString(credenciais[0]);
    var senha = credenciais.Length > 1 ? Uri.UnescapeDataString(credenciais[1]) : string.Empty;
    var porta = uri.Port > 0 ? uri.Port : 5432;
    var banco = uri.AbsolutePath.TrimStart('/');

    return $"Host={uri.Host};Port={porta};Database={banco};Username={usuario};Password={senha};" +
           "SSL Mode=Prefer;Trust Server Certificate=true";
}
