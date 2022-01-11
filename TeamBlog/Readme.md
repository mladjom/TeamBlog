# TeamBlog - A blog using ASP.NET Core 6.0 Razor Pages

A blog using Razor Pages in ASP.NET Core 6.0 with Entity Framework and Core Identity. It is connected to SQL server. 
DefaultConnection in appsettings.json needs to be updated

I already implementented authentication and authorization. There a three user rolls: administrator, editor and registered user. Registered users can create new article which by default get status submited. Editors can approve or reject articles.

Articles belong to categories.

On first run program will seed database with admin and editor users, default categories and dummy articles.

### Default users
admin: admin@site.test pass: Ab123_
editor: editor@site.test pass: Ab123_

### TODO

- [ ] Finish views 
- [x] Upload and display featured image 
- [ ] Display articles per category
- [ ] Update seed


## Usefull commands

```cmd
Add-Migration Init
```

```cmd
Update-Database
```

```cmd
Remove-Migration
```

```cmd
Drop-Database
```

```cmd
dotnet watch
```

```cmd
dotnet build
```
