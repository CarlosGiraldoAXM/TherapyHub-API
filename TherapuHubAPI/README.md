dotnet ef dbcontext scaffold "Server=PC-CARLOS\SQLEXPRESS;Database=THERAPYHUB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer -c ContextDB -o Models --use-database-names --no-pluralize --force

//AMBIENTE PUBLICADO 
dotnet ef dbcontext scaffold "Server=tcp:sql-therapyhub-dev-001.database.windows.net,1433;Database=THERAPYHUB;User ID=adminsql;Password=SqlDev#2026!;Encrypt=True;TrustServerCertificate=False;MultipleActiveResultSets=True;" Microsoft.EntityFrameworkCore.SqlServer -c ContextDB -o Models --use-database-names --no-pluralize --force
