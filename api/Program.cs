using api.src.modules.address.infrastructure.ioc;
using api.src.modules.auth.infrastructure.ioc;
using api.src.modules.user.infrastructure.ioc;
using api.src.shared.infrastructure.ioc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "config"))
                     .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                     .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                     .AddEnvironmentVariables();

// Add services to the container.

builder.Services.AddSharedModule(builder.Configuration);

builder.Services.AddUserModule(builder.Configuration);
builder.Services.AddAddressModule(builder.Configuration);
builder.Services.AddAuthModule(builder.Configuration);

builder.Services.AddControllers();
// Configurar CORS lendo origens permitidas do appsettings
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowedOrigins", policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }
        else
        {
            // Em desenvolvimento, permitir qualquer origem se não estiver configurado
            if (builder.Environment.IsDevelopment())
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            }
            else
            {
                // Política restrita por padrão (nenhuma origem) quando em produção e sem configuração
                policy.DisallowCredentials();
            }
        }
    });
});

// Configuração do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "Avaliação AEC API",
        Description = "API para gerenciamento de usuários e endereços",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Equipe de Desenvolvimento",
            Url = new Uri("https://github.com/wallacelion/avaliacao_dev")
        }
    });

    // Incluir comentários XML
    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Configuração de autenticação JWT no Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Insira o token JWT no formato: Bearer {seu token}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


//// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

builder.Logging.AddJsonConsole(options =>
{
    options.IncludeScopes = false;
    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
    options.JsonWriterOptions = new System.Text.Json.JsonWriterOptions
    {
        Indented = true
    };
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Avaliação AEC API v1");
        options.RoutePrefix = string.Empty; // Swagger na raiz (http://localhost:porta/)
    });
}

// Migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Busca o seu contexto global unificado
        var context = services.GetRequiredService<api.src.shared.infrastructure.data.AppDbContext>();

        Console.WriteLine("[Migration] Verificando atualizações do banco global...");

        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
            Console.WriteLine("[Migration] Sucesso: Banco global totalmente atualizado!");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Migration] ERRO crítico ao aplicar migrações: {ex.Message}");
    }
}

app.UseMiddleware<api.src.shared.infrastructure.middlewares.LogRequestMiddleware>();

// Middleware global de tratamento de exceções deve ser registrado antes dos demais middlewares
app.UseMiddleware<api.src.shared.infrastructure.middlewares.ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

// Habilitar CORS antes de Authentication/Authorization e MapControllers
app.UseCors("AllowedOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
