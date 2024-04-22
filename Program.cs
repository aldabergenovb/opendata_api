using System.Net.Http.Headers;
using opendata_api.Services;
using opendata_api.Services.Policies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<HttpClientPolicy>();

builder.Services.AddHttpClient<IDataEgovHttpService, DataEgovHttpService>("DataEgovHttpService", client =>
{
    string uri = builder.Configuration["StatGov:HttpClientBaseAddress"]!;
    client.BaseAddress = new Uri(uri);
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    client.DefaultRequestHeaders.Add("User-Agent", "api");
    client.Timeout = TimeSpan.FromMinutes(10);
});

builder.Services.AddHttpClient<IGovHttpService, GovHttpService>("GovNewsService", client =>
{
    string uri = builder.Configuration["GovNews:HttpClientBaseAddress"]!;
    client.BaseAddress = new Uri(uri);
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    client.DefaultRequestHeaders.Add("User-Agent", "api");
    client.Timeout = TimeSpan.FromMinutes(10);
});

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        corsbuilder => corsbuilder
            .WithOrigins(builder.Configuration.GetSection("Origins").Get<string[]>() ?? Array.Empty<string>())
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();
app.UseCors("CorsPolicy");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();
app.Run();