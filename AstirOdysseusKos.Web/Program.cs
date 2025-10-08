using AstirOdysseusKos.Web.Models;
using AstirOdysseusKos.Web.Services;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Options;
using System.IO.Compression;


var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
IConfigurationBuilder config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true)
    .AddEnvironmentVariables();

ConfigurationManager _config = builder.Configuration; // allows both to access and to set up the config

builder.Services.AddOptions<EmailSettings>().Bind(_config.GetSection("EmailSettings"));

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = new[] { "text/plain", "text/html", "text/css", "application/javascript", "text/javascript", "image/svg+xml" };
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.SmallestSize;
});

builder.Services.AddTransient<IConfigureOptions<StaticFileOptions>, ConfigureStaticFileOptions>();

builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddDeliveryApi()
    .AddComposers()
    .Build();

WebApplication app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseResponseCompression();
    /*   app.UseXfo(options => options.SameOrigin());
       app.UseXContentTypeOptions();
       app.UseCsp(opts => opts.UpgradeInsecureRequests());
       app.UseHsts(options => options.MaxAge(days: 30).IncludeSubdomains());
       app.UseXXssProtection(options => options.EnabledWithBlockMode());
       app.UseReferrerPolicy(opts => opts.StrictOriginWhenCrossOrigin());*/
}

await app.BootUmbracoAsync();

app.UseHttpsRedirection();

app.UseUmbraco()
    .WithMiddleware(u =>
    {
        u.UseBackOffice();
        u.UseWebsite();
    })
    .WithEndpoints(u =>
    {
        u.UseInstallerEndpoints();
        u.UseBackOfficeEndpoints();
        u.UseWebsiteEndpoints();
    });

await app.RunAsync();
