using Hadddock.Email;

string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

string? smtpClientServer = builder.Configuration.GetSection("smtpClientServer").Value;
if (smtpClientServer == null)
    throw new KeyNotFoundException("smtpClientServer key not found in appsettings.json");
string? smtpClientPort = builder.Configuration.GetSection("smtpClientPort").Value;
if (smtpClientPort == null)
    throw new KeyNotFoundException("smtpClientPort key not found in appsettings.json");
string? smtpClientLogin = builder.Configuration.GetSection("smtpClientLogin").Value;
if (smtpClientLogin == null) 
    throw new KeyNotFoundException("smtpClientLogin key not found in appsettings.json");
string? smtpClientPassword = builder.Configuration.GetSection("smtpClientPassword").Value;
if (smtpClientPassword == null) 
    throw new KeyNotFoundException("smtpClientPassword key not found in appsettings.json");

builder.Services.AddSingleton<IEmailService>(emailService => new EmailSender
    (
        smtpClientServer,
        smtpClientPort,
        smtpClientLogin,
        smtpClientPassword
    ));
builder.Services.AddCors(options =>
    {

        options.AddPolicy(name: MyAllowSpecificOrigins,
            policy =>
            {
                policy.WithOrigins("https://hadddock.github.io",
                                    "http://hadddock.github.io")
                .AllowAnyHeader()
                .AllowAnyMethod();

            });
    });
var app = builder.Build();
app.UseCors();

// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors(MyAllowSpecificOrigins);

app.MapControllers();

app.Run();