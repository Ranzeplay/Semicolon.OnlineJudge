# Semicolon.OnlineJudge

---

## How to run the project

### Run locally

- Clone the project to your computer
- Run `dotnet restore` to restore the project
- Install EntityFrameworkCore by running command `dotnet tool install dotnet-ef -g`
- Run `dotnet ef database update` to create database from existing configuration
- Run `dotnet run` to run code on debug mode locally

---

## Working Progress

Almost done. 

Issues will be fixed *as fast as I can* if there is.

### Planned Tasks

- Use [TailwindCSS](https://github.com/tailwindlabs/tailwindcss) as its web UI
- ~~Judge the code in a safe way (`Docker` or `WebAssembly`)~~
- Deploy an example
- Support `Linux`
- Management dashboard

### Completed Tasks

- Integrated authentication
- Add problem (`Markdown` support)
- Solve problem (`C` and `C++`)
- Code judgement (Real-time feedback with `SingalR`)

### Database

Using `SQLite` with `EntityFramework Core` currently
