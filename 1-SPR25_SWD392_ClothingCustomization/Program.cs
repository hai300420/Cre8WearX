using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repository.IRepository;
using Repository.Repository;
using Service;
using Service.Service;
using System.Security.Claims;
using System.Text;
using BusinessObject.Mapper;
using BusinessObject.Model;
using Microsoft.EntityFrameworkCore;
using _2_Service.Service;
using _3_Repository.Repository;
using _3_Repository.IRepository;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using _2_Service.Vnpay;
using VNPAY.NET;
using _4_BusinessObject.RequestDTO;
using _2_Service.ThirdPartyService;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient(); // Needed for HttpClientFactory


// Đăng ký Authentication với JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                RoleClaimType = ClaimTypes.Role,
                ValidAudience = builder.Configuration["Jwt:Audience"],
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding
                    .UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
            };
        });

// Đăng ký Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddEndpointsApiExplorer();

// Allow cross platform
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();

        });
});

// Cấu hình Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SWD392", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Please enter a valid token",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };
    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type=ReferenceType.SecurityScheme,
                        Id="Bearer"
                    }
                },
                new string[]{}
            }
        });
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<ClothesCusShopContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DB")));

// Đăng ký DI (Dependency Injection)
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IJWTService, JWTService>();

//Đăng ký Repository
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ICustomizeProductRepository, CustomizeProductRepository>();
builder.Services.AddScoped<IDesignAreaRepository, DesignAreaRepository>();
builder.Services.AddScoped<IDesignElementRepository, DesignElementRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderStageRepository, OrderStageRepository>();

// Đăng ký Service
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICustomizeProductService, CustomizeProductService>();
builder.Services.AddScoped<IDesignAreaService, DesignAreaService>();
builder.Services.AddScoped<IDesignElementService, DesignElementService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderStageService, OrderStageService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IPaymentMappingService, PaymentMappingService>();


// Đăng ký VnPay Service
builder.Services.AddScoped<IVnPayService, VnPayService>();
// Add VNPAY service to the container.
builder.Services.AddSingleton<IVnpay, Vnpay>();
// Register MoMo configuration
builder.Services.Configure<MoMoConfig>(builder.Configuration.GetSection("MoMo"));
// Add Cloudinary
builder.Services.AddSingleton<CloudinaryService>();


// Đăng ký AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Load Firebase Private Key JSON File
var firebaseJsonPath = Path.Combine(builder.Environment.WebRootPath, "firebase", "clothescustom.json");

FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile(firebaseJsonPath)
});

// Xây dựng ứng dụng
var app = builder.Build();


// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
if (app.Environment.IsDevelopment() || app.Environment.IsProduction() || app.Environment.IsStaging())
{
    // Enable Swagger for API testing
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SWD392 v1");
        c.InjectStylesheet("/static/css/swaggerui-dark.css"); // Optional: Custom styling for Swagger
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAll");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
