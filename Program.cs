var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Correct base URL for legacy Web Tools (XML APIs)
builder.Services.AddHttpClient("usps", client =>
{
    client.BaseAddress = new Uri("https://secure.shippingapis.com/ShippingAPI.dll");
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddTransient<PermitApplication.Services.UspsService>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    var client = factory.CreateClient("usps");
    var config = sp.GetRequiredService<IConfiguration>();

    // This is the correct way
    var userId = config["Usps:UserId"];

    if (string.IsNullOrWhiteSpace(userId))
        throw new InvalidOperationException("USPS UserId is missing from configuration.");

    return new PermitApplication.Services.UspsService(client, config);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
