global using MyAspNetCore8App.Common;
global using static MyAspNetCore8App.Common.Constants;
using Microsoft.AspNetCore.Authentication.Negotiate;
using MyAspNetCore8App.Domain;
using MyAspNetCore8App.Logging;
using MyAspNetCore8App.MssqlDataAccess;
using MyAspNetCore8App.MssqlDataAccess.Utilities;
using Serilog;

// 起動時のエラーを記録するためのブートストラップロガー
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
       .AddNegotiate();

    builder.Services.AddAuthorization(options =>
    {
        options.FallbackPolicy = options.DefaultPolicy;
    });
    builder.Services.AddRazorPages();

    // ロギング関連の依存性注入
    builder.Services.AddTransient<HttpContextEnricher>();
    builder.Services.AddHttpContextAccessor();
    builder.Host
        .UseSerilog((hostBuilderContext, serviceProvider, loggerConfiguration) =>
            loggerConfiguration
                .Enrich.FromLogContext()
                .Enrich.With(serviceProvider.GetService<HttpContextEnricher>()
                    ?? throw new InvalidOperationException("HttpContextEnricherの取得に失敗しました"))
                .ReadFrom.Configuration(builder.Configuration));

    // ドメインロジックの依存性注入
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionString")
        ?? throw new InvalidOperationException("DefaultConnectionStringの取得に失敗しました");
    builder.Services.AddSingleton(new MssqlContext(connectionString));
    builder.Services.AddSingleton<IDepartmentRepository, MssqlDepartmentRepository>();
    builder.Services.AddSingleton<IDepartmentService, DepartmentService>();
    builder.Services.AddSingleton<IMemberRepository, MssqlMemberRepository>();
    builder.Services.AddSingleton<IMemberService, MemberService>();

    // Excel出力関連の依存性注入
    var excelSettings = builder.Configuration.GetSection("ExcelSettings").Get<ExcelSettings>()
        ?? throw new InvalidOperationException("ExcelSettingsの取得に失敗しました");
    builder.Services.AddSingleton(excelSettings);
    builder.Services.AddSingleton<IExcelCreator, EpplusExcelCreator>();

    // js/cssのミニファイの依存性注入
    builder.Services.AddWebOptimizer();

    var app = builder.Build();

    if (app.Environment.IsEnvironment("LocalDebug"))
    {
        // ローカルデバッグ時は開発者用エラーページを有効化
        app.UseDeveloperExceptionPage();
    }
    else
    {
        // 通常のエラーページを有効化
        app.UseExceptionHandler("/Error");
        // HTTP Strict Transport Security プロトコルヘッダーを有効化
        app.UseHsts();
    }

    // js/cssのミニファイを有効化
    app.UseWebOptimizer();

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapRazorPages();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "起動時に予期しないエラーが発生しました");
}
finally
{
    Log.CloseAndFlush();
}
