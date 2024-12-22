using Book_Management.DBContext;
using Book_Management.GraphQL;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
ConfigurePipeline(app, app.Environment);

app.Run();


static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Database Configuration
    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

    // GraphQL Configuration
    services
        .AddGraphQLServer()
        .AddQueryType<Query>()
        .AddMutationType<Mutation>()
        .AddType<AuthorType>()
        .AddType<BookType>()
        .AddFiltering()
        .AddSorting()
        .AddProjections();

    // CORS Configuration
    services.AddCors(options =>
    {
        options.AddPolicy("Book_Management_Frontend",
            builder => builder
                .WithOrigins("http://localhost:4200")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
    });

    // Controllers (if needed for REST endpoints)
    services.AddControllers();

    // Swagger/OpenAPI (optional, but recommended)
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();
}

static void ConfigurePipeline(WebApplication app, IWebHostEnvironment env)
{
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // Enable CORS
    app.UseCors("AllowAngularApp");

    // Ensure database is created and migrated
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate(); // Applies any pending migrations
    }

    // Enable HTTPS redirection
    app.UseHttpsRedirection();

    // Enable authorization (if using authentication)
    app.UseAuthorization();

    // Map controllers
    app.MapControllers();

    // Map GraphQL endpoint
     app.MapGraphQLHttp("/graphql");
}