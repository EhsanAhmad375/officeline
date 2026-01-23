using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using officeline.Data; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models; // Security models ke liye
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.DependencyInjection;
var builder = WebApplication.CreateBuilder(args);

// 1. Database Connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// 2. Authentication Setup
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                // Default behavior ko skip karein
                context.HandleResponse();

                // Custom response set karein
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                
                var result = System.Text.Json.JsonSerializer.Serialize(new { 
                    success = false, 
                    message = "Unauthorized " 
                });

                return context.Response.WriteAsync(result);
            },
            OnForbidden = context =>
    {
        context.Response.StatusCode = 403;
        context.Response.ContentType = "application/json";
        var result = System.Text.Json.JsonSerializer.Serialize(new { 
            success = false, 
            message = "You are not authorized to access this resource."

        });
        return context.Response.WriteAsync(result);
    }
            
        };
        
    
    });

    
builder.Services.AddHttpContextAccessor();


builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(er => er.ErrorMessage).FirstOrDefault()
                );
            return new BadRequestObjectResult(new { success = false, errors });
        };
    });

// 4. Swagger with JWT Support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "OfficeLine API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Enter 'token' only."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});

// 5. Dependency Injection
builder.Services.AddScoped<officeline.repo.IUserRepo, officeline.repo.UserRepo>();
builder.Services.AddScoped<officeline.repo.ICompanyRepo, officeline.repo.CompanyRepo>();
builder.Services.AddScoped<officeline.Services.ICompanyServices, officeline.Services.CompanyServices>();
builder.Services.AddScoped<officeline.Services.IUsers, officeline.Services.Users>(); 
builder.Services.AddControllers().AddNewtonsoftJson(); 



var app = builder.Build();
app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "uploads")),
    RequestPath = "/uploads"
});

// --- MIDDLEWARE PIPELINE (ORDER IS CRITICAL) ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); 
}

// Order: Routing -> Authentication -> Authorization -> Map
app.UseRouting(); 

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers(); 

app.MapGet("/", () => "OfficeLine API is Fixed and Secured!");

app.Run();