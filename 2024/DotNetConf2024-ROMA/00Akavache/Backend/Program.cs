using GenoeseExpenseMng.Backend.Simple.ServiceExtension;
using Mapster;
using MapsterMapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IMapper, ServiceMapper>();
builder.Services.AddSingleton(GetConfiguredMappingConfig());

builder.Services.ConfigureOData();

builder.Services.AddPersistenceServices(builder.Configuration);






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


static TypeAdapterConfig GetConfiguredMappingConfig()
{

    var config = new TypeAdapterConfig();

    ////TypeAdapterConfig.GlobalSettings.AllowImplicitSourceInheritance = true;

    ////var config = new TypeAdapterConfig
    ////{
    ////    Compiler = exp => exp.CompileWithDebugInfo(new ExpressionCompilationOptions { EmitFile = true, ThrowOnFailedCompilation = true })
    ////};
    //#region 
    //#endregion
    //#region ContrattoMac
    ////config.NewConfig<AnagBadante, AnagBadanteDTO>()
    ////    .Map(dest => dest.CodFiscaleNomeCognome,

    ////        src => src.CodiceFiscale + "-" + src.Nome + " " + src.Cognome



    ////    );

    ////Func<AnagDatoreDiLavoro, string> func = (a) =>
    ////{
    ////    StringBuilder retString = new();
    ////    if (string.IsNullOrWhiteSpace(a.RagioneSociale))
    ////    {
    ////        retString.Append(a.Nome + " " + a.Cognome);

    ////    }
    ////    else
    ////    {
    ////        retString.Append(a.RagioneSociale);
    ////    }

    ////    retString.Append(" (");
    ////    if (!string.IsNullOrWhiteSpace(a.PartitaIva))
    ////    {
    ////        retString.Append("PI: " + a.PartitaIva);

    ////    }
    ////    if (!string.IsNullOrWhiteSpace(a.CodiceFiscale))
    ////    {
    ////        retString.Append("CF: " + a.CodiceFiscale);

    ////    }
    ////    retString.Append(")");
    ////    return retString.ToString();

    ////};

    ////Expression<Func<AnagDatoreDiLavoro, string>> getAnagDatoreDiLavoroDTO_RagSocialePartitaIva =
    ////    Expression.Lambda<Func<AnagDatoreDiLavoro, string>>(Expression.Call(func.Method));

    ////config.NewConfig<AnagDatoreDiLavoro, AnagDatoreDiLavoroDTO>()
    ////    .Map(dest => dest.RagSocialePartitaIva,
    ////    (src => (string.IsNullOrEmpty(src.RagioneSociale) ? src.Nome + " " + src.Cognome : src.RagioneSociale) +
    ////    " (" + (string.IsNullOrEmpty(src.PartitaIva) ? string.Empty : "PI: " + src.PartitaIva) +
    ////        (string.IsNullOrEmpty(src.CodiceFiscale) ? string.Empty :
    ////            (string.IsNullOrEmpty(src.PartitaIva) ? string.Empty : " - ") +
    ////            "CF: " + src.CodiceFiscale
    ////        )
    ////        + ")"
    ////    )
    ////    );

    ////#endregion
    ////config.NewConfig<ContrattoMac, ContrattoMacDTO>()
    ////  .Map(dest => dest.DescrizioneAssistito,
    ////      src => (src.AnagAssistito != null ? src.AnagAssistito.CodFiscaleNomeCognome : string.Empty))
    ////    .Map(dest => dest.NumeroMatchingOrigine,
    ////      src => (src.Matching != null ? src.Matching.NumeroCompletoMatching : string.Empty))
    ////     .Map(dest => dest.DescrizioneAssistito, src => src.AnagAssistito.CodFiscaleNomeCognome, srcCond => srcCond.AnagAssistito != null);



    ////config.NewConfig<ContrattoMacBadante, ContrattoMacBadanteDTO>()
    ////    .Map(dest => dest.CodFiscaleNomeCognome,
    ////        src => (src.Badante != null ? src.Badante.CodFiscaleNomeCognome : string.Empty));

    ////config.NewConfig<Fattura, FatturaDTO>()
    ////    .Map(dest => dest.DescrizioneOrigineFattura, src => "Matching numero " + src.Matching.NumeroCompletoMatching, srcCond => srcCond.Matching != null)
    ////    ;

    ////config.NewConfig<Matching, MatchingDTO>()
    ////    .Map(dest => dest.DescrizioneDatoreDiLavoro, src => src.AnagDatoreDiLavoro.RagSocialePartitaIva, srcCond => srcCond.AnagDatoreDiLavoro != null)
    ////    .Map(dest => dest.DescrizioneAssistito, src => src.AnagAssistito.CodFiscaleNomeCognome, srcCond => srcCond.AnagAssistito != null)
    ////    .Map(dest => dest.DescrizioneBadante, src => src.AnagBadante.CodFiscaleNomeCognome, srcCond => srcCond.AnagBadante != null)
    ////        .Map(dest => dest.DescrizioneTipoStatoMatching, src => "In Corso", srcCond => srcCond.TipoStatoMatching != null && srcCond.TipoStatoMatching.IdStatoMatching == (int)EnumStatoMatching.InCorso)
    ////        .Map(dest => dest.DescrizioneTipoStatoMatching, src => "Terminato", srcCond => srcCond.TipoStatoMatching != null && srcCond.TipoStatoMatching.IdStatoMatching == (int)EnumStatoMatching.Terminato)
    ////        .Map(dest => dest.DescrizioneTipoStatoMatching, src => "Fattura Emessa", srcCond => srcCond.TipoStatoMatching != null && srcCond.TipoStatoMatching.IdStatoMatching == (int)EnumStatoMatching.FatturaEmessa)
    ////        .Map(dest => dest.DescrizioneTipoStatoMatching, src => "Fattura Pagata", srcCond => srcCond.TipoStatoMatching != null && srcCond.TipoStatoMatching.IdStatoMatching == (int)EnumStatoMatching.FatturaPagata)
    ////    ;
    ////config.NewConfig<Matching, MatchingDTO>()






    ////    .Map(dest => dest.DescrizioneDatoreDiLavoro, src => src.AnagDatoreDiLavoro.RagSocialePartitaIva, srcCond => srcCond.AnagDatoreDiLavoro != null)
    ////    .Map(dest => dest.DescrizioneAssistito, src => src.AnagAssistito.CodFiscaleNomeCognome, srcCond => srcCond.AnagAssistito != null)
    ////    .Map(dest => dest.DescrizioneBadante, src => src.AnagBadante.CodFiscaleNomeCognome, srcCond => srcCond.AnagBadante != null)

    ////    ;


    ////config.NewConfig<MatchingForEditing, Matching>()

    ////    .Ignore(dest => dest.AnagDatoreDiLavoro)
    ////    .Ignore(dest => dest.AnagAssistito)
    ////    .Ignore(dest => dest.AnagBadante);

    ////config.NewConfig<TipoStatoMatching, Matching>()

    ////    .Ignore(dest => dest.AnagDatoreDiLavoro)
    ////    .Ignore(dest => dest.AnagAssistito)
    ////    .Ignore(dest => dest.AnagBadante);

    ////config.ForType<TipoStatoMatching, TipoStatoMatching>().MapToConstructor(true);
    ////ConstructUsing

    ////config.NewConfig<MatchingForEditing, Matching>()
    ////    .AfterMappingAsync(async dto =>
    ////    {
    ////        var context = MapContext.Current.GetService<SchoolContext>();
    ////        var course = await context.Courses.FindAsync(dto.CourseID);
    ////        if (course != null)
    ////            dto.CourseTitle = course.Title;
    ////        var student = await context.Students.FindAsync(dto.StudentID);
    ////        if (student != null)
    ////            dto.StudentName = MapContext.Current.GetService<NameFormatter>().Format(student.FirstMidName, student.LastName);
    ////    });
    ////config.NewConfig<Student, StudentDto>()
    ////    .Map(dest => dest.Name, src => MapContext.Current.GetService<NameFormatter>().Format(src.FirstMidName, src.LastName));
    ////config.NewConfig<Course, CourseDto>()
    ////    .Map(dest => dest.CourseIDDto, src => src.CourseID)
    ////    .Map(dest => dest.CreditsDto, src => src.Credits)
    ////    .Map(dest => dest.TitleDto, src => src.Title)
    ////    .Map(dest => dest.EnrollmentsDto, src => src.Enrollments);

    return config;
}