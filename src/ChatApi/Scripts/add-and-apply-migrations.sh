cd /app/Migrations

if ! compgen -G "*${MIGRATIONS_NAME}.cs" > /dev/null ; then
    cd ..
    echo "No migrations found. Creating new migration..."
    dotnet ef migrations add ${MIGRATIONS_NAME} --project ${PROJECT} --startup-project ${PROJECT} --context ${SQL_CONTEXT_CLASS} --verbose
else
    echo "There is already a migration with that name. Skipping..."
	cd ..
fi

dotnet ef database update --project ${PROJECT} --startup-project ${PROJECT} --context ${SQL_CONTEXT_CLASS} --verbose
