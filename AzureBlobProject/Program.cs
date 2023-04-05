using Azure.Storage.Blobs;
using AzureBlobProject.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add Blob Service Client to the container
builder.Services.AddSingleton(blob => new BlobServiceClient(
        builder.Configuration.GetValue<string>("BlobConnection")));

// Add Blob Container Service to the container
builder.Services.AddSingleton<IContainerService, ContainerService>();

// Add Blob Service to the container
builder.Services.AddSingleton<IBlobService, BlobService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
