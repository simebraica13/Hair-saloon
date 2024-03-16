using Microsoft.EntityFrameworkCore;
using Hair_saloon.Models;
using Microsoft.AspNetCore.Identity;
using System.Configuration;
using Microsoft.AspNetCore.Authentication;


var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("HairSaloonConnectionString") ?? throw new InvalidOperationException("Connection string 'TasksUsersContextConnection' not found.");

builder.Services.AddControllers();


builder.Services.AddCors(options => {
    options.AddPolicy(MyAllowSpecificOrigins,
                          policy => {
                              policy.WithOrigins("http://localhost:5173")
                                    .AllowAnyHeader()
                                    .AllowAnyMethod();
                          });
});




builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<HairSaloonContext>();


var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseCors(MyAllowSpecificOrigins);

app.MapControllers();

app.Run();
