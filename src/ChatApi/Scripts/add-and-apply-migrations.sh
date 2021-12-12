dotnet ef migrations add ${MIGRATIONS_NAME} --project ${PROJECT} --startup-project ${PROJECT} --context ${SQL_CONTEXT_CLASS} --verbose

dotnet ef database update --project ${PROJECT} --startup-project ${PROJECT} --context ${SQL_CONTEXT_CLASS} --verbose
