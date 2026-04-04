using BLL.Services.Auth;
using BLL.Services.BouquetGeneration;
using BLL.Services.BouquetGeneration.BouquetPlanner;
using BLL.Services.BouquetGeneration.BouquetPlanner.FlowerComposition;
using BLL.Services.Bouquets;
using BLL.Services.FileStorage;
using BLL.Services.Flowers;
using BLL.Services.OpenAi;
using BLL.Services.OpenAi.OpenAiClient;
using BLL.Services.OrderBouquets;
using BLL.Services.Orders;
using BLL.Services.Users;
using BLL.Services.WrappingPapers;
using BLL.Services.BouquetAssembly;
using BLL.Services.BouquetAssembly.FlowerProcessingStep;
using BLL.Services.BouquetAssembly.FlowersProcessingStep;
using BLL.Services.BouquetAssembly.WrappingStep;
using DAL.Data;
using DAL.Data.UnitOfWork;
using DAL.Models;
using DAL.Repositories.BouquetFlowers;
using DAL.Repositories.Bouquets;
using DAL.Repositories.Flowers;
using DAL.Repositories.OrderBouquets;
using DAL.Repositories.Orders;
using DAL.Repositories.WrappingPapers;
using FlowerShopApi.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        corsBuilder =>
        {
            corsBuilder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
        });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), a => a.MigrationsAssembly("DAL"));
});
builder.Services
    .AddIdentity<User, IdentityRole<int>>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

//builder.Services.AddAuthorization();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IFlowerRepository, FlowerRepository>();
builder.Services.AddScoped<IBouquetRepository, BouquetRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IBouquetFlowerRepository, BouquetFlowerRepository>();
builder.Services.AddScoped<IOrderBouquetRepository, OrderBouquetRepository>();
builder.Services.AddScoped<IWrappingPaperRepository, WrappingPaperRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderBouquetService, OrderBouquetService>();
builder.Services.AddScoped<IFlowerService, FlowerService>();
builder.Services.AddScoped<IBouquetService, BouquetService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBouquetGenerationService, BouquetGenerationService>();
builder.Services.AddScoped<IFlowerCompositionBuilder, FlowerCompositionBuilder>();
builder.Services.AddScoped<IOpenAIService, OpenAIService>();
builder.Services.AddHttpClient<IOpenAiClient, OpenAiClient>();
builder.Services.AddScoped<IWrappingPaperService, WrappingPaperService>();
builder.Services.AddScoped<IBouquetPlanner, BouquetPlanner>();
builder.Services.AddScoped<IFlowerProcessingStep, BLL.Services.BouquetAssembly.FlowersProcessingStep.FlowerProcessingStep>();
builder.Services.AddScoped<IBouquetWrappingStep, BouquetWrappingStep>();
builder.Services.AddScoped<IBouquetAssembly, BLL.Services.BouquetAssembly.BouquetAssembly>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
var uploadPath = Path.Combine(builder.Environment.ContentRootPath, "UploadedFiles");
builder.Services.AddScoped<IFileStorage>(_ => new FileStorage(uploadPath));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,

        ValidAudience = builder.Configuration.GetSection("AppSettings:ValidAudience").Value,
        ValidIssuer = builder.Configuration.GetSection("AppSettings:ValidIssuer").Value,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value))
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();

app.UseAuthorization();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "UploadedFiles")),
    RequestPath = "/uploads"
});

app.MapControllers();

app.Run();
