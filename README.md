# Semicolon.OnlineJudge
[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2FRanzeplay%2FSemicolon.OnlineJudge.svg?type=shield)](https://app.fossa.com/projects/git%2Bgithub.com%2FRanzeplay%2FSemicolon.OnlineJudge?ref=badge_shield)


---

## How to run the project

> You need to install `gcc` compiler first

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

- Support `Linux`
- Management dashboard

### Completed Tasks

- Integrated authentication
- Add problem (`Markdown` support)
- Solve problem (`C`,  `C++`, you can even setup your own compiler)
- Code judgement (Real-time feedback with `SingalR`)

### Database

Using `SQLite` with `EntityFramework Core` currently

## Activity

![Alt](https://repobeats.axiom.co/api/embed/e0e20f1ce7bed699711bde11ede4f6c1422421b5.svg "Repobeats analytics image")

## License

[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2FRanzeplay%2FSemicolon.OnlineJudge.svg?type=large)](https://app.fossa.com/projects/git%2Bgithub.com%2FRanzeplay%2FSemicolon.OnlineJudge?ref=badge_large)
