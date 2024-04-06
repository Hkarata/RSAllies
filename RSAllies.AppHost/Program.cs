var builder = DistributedApplication.CreateBuilder(args);

var apiservice = builder.AddProject<Projects.RSAllies_Api>("apiservice");

builder.AddProject<Projects.RSAllies>("web-admin")
    .WithReference(apiservice);

builder.Build().Run();
