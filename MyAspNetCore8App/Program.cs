global using MyAspNetCore8App.Common;
global using static MyAspNetCore8App.Common.Constants;
using Microsoft.AspNetCore.Authentication.Negotiate;
using MyAspNetCore8App.Domain;
using MyAspNetCore8App.Logging;
using MyAspNetCore8App.MssqlDataAccess;
using MyAspNetCore8App.Utilities;

using Serilog;

// �N�����̃G���[���L�^���邽�߂̃u�[�g�X�g���b�v���K�[
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

    // ���M���O�֘A�̈ˑ�������
    builder.Services.AddTransient<HttpContextEnricher>();
    builder.Services.AddHttpContextAccessor();
    builder.Host
        .UseSerilog((hostBuilderContext, serviceProvider, loggerConfiguration) =>
            loggerConfiguration
                .Enrich.FromLogContext()
                .Enrich.With(serviceProvider.GetService<HttpContextEnricher>()
                    ?? throw new InvalidOperationException("HttpContextEnricher�̎擾�Ɏ��s���܂���"))
                .ReadFrom.Configuration(builder.Configuration));

    // �h���C�����W�b�N�̈ˑ�������
    var connectionString = builder.Configuration.GetConnectionString("MyDatabaseConnectionString")
        ?? throw new InvalidOperationException("�ڑ������񂪐ݒ肳��Ă��܂���");
    var context = new MyDatabaseContext(connectionString);
    builder.Services.AddSingleton(context);
    builder.Services.AddSingleton<IDepartmentRepository, MssqlDepartmentRepository>();
    builder.Services.AddSingleton<IDepartmentService, DepartmentService>();
    builder.Services.AddSingleton<IMemberRepository, MssqlMemberRepository>();
    builder.Services.AddSingleton<IMemberService, MemberService>();

    // Excel�o�͊֘A�̈ˑ�������
    var excelSettings = builder.Configuration.GetSection("ExcelSettings").Get<ExcelSettings>()
        ?? throw new InvalidOperationException("ExcelSettings�̎擾�Ɏ��s���܂���");
    builder.Services.AddSingleton(excelSettings);
    builder.Services.AddSingleton<IExcelCreator, EpplusExcelCreator>();

    // js/css�̃~�j�t�@�C�̈ˑ�������
    builder.Services.AddWebOptimizer();

    var app = builder.Build();

    if (app.Environment.IsEnvironment("LocalDebug"))
    {
        // ���[�J���f�o�b�O���͊J���җp�G���[�y�[�W��L����
        app.UseDeveloperExceptionPage();
    }
    else
    {
        // �ʏ�̃G���[�y�[�W��L����
        app.UseExceptionHandler("/Error");
        // HTTP Strict Transport Security �v���g�R���w�b�_�[��L����
        app.UseHsts();
    }

    // js/css�̃~�j�t�@�C��L����
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
    Log.Fatal(ex, "�N�����ɗ\�����Ȃ��G���[���������܂���");
}
finally
{
    Log.CloseAndFlush();
}
