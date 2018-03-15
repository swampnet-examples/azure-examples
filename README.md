# azure-examples

Add user-secrets:

- powershell (admin) in project directory
- >dotnet user-secrets set name "value"
- Creates secrets.json in %APPDATA%\microsoft\UserSecrets\<userSecretsId>\secrets.json  Where <userSecretsId> is the guid defined in the .csproj
- eg: %APPDATA%\microsoft\UserSecrets\26A8EF48-294D-4C99-A15E-EB796BF91056\secrets.json

