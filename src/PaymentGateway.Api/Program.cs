using PaymentGateway.Api.Repositories;
using PaymentGateway.Api.Services;
using PaymentGateway.Api.Validation.Requests;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<IAcquiringBankService, AcquiringBankService>(client =>
{
    var baseUri = builder.Configuration.GetValue<string>("Services:AcquiringBankService:BaseUri") ?? throw new InvalidOperationException("Services:AcquiringBankService:BaseUri is null");
    client.BaseAddress = new Uri(baseUri);
});
builder.Services.AddSingleton<PostPaymentRequestValidator>();
builder.Services.AddSingleton<IPaymentsRepository, PaymentsRepository>();
builder.Services.AddSingleton<IPaymentService, PaymentService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
