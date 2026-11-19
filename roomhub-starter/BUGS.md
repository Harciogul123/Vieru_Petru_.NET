# BUGS

The starter contains behaviors that contradict `ASSIGNMENT.md`. Write here every discrepancy you found.
Fill it in yourself, in your own words. There is no required number: what counts is that they are real.

| Which rule from ASSIGNMENT.md is violated (R1–R8) | Where the problem was in the starter (file, approximate line) | How you discovered it (test, reading, AI) | What you changed |
|---|---|---|---|
| R7 (intervalele care doar se ating nu se suprapun) | `Services/BookingService.cs`, ~linia 28: `existing.StartUtc <= endUtc && startUtc <= existing.EndUtc` | Citind codul, apoi request în `roomhub.http`: o rezervare 10–12 și una 12–13 dădeau 409 | `DateRange.Overlaps` folosește `<` strict (interval semi-deschis). Regula e în `Domain/Bookings/DateRange.cs` și e apelată din `Room.Book` |
| R7 + R8 (o rezervare anulată eliberează intervalul) | `Services/BookingService.cs`, ~linia 26: `ListForRoom` întorcea și rezervările anulate, iar bucla nu verifica `Status` | Citind codul: după cancel nu mai puteam rezerva același interval | `Room.Book` verifică doar `b.IsActive`, iar `RoomRepository.GetByIdAsync` încarcă doar rezervările `Active` |
| R8 (anularea unei rezervări deja anulate dă 409) | `Services/BookingService.cs`, ~liniile 56–60: `Cancel` seta `Status = Cancelled` fără nicio verificare, deci a doua oară dădea tot 204 | Request dublu la `/cancel` în `roomhub.http` | `Booking.Cancel` întoarce `BookingErrors.AlreadyCancelled` (Conflict → 409) |
| R5 (nu poate începe în trecut, "acum" din `TimeProvider`) | `Services/BookingService.cs`, metoda `Create` (~liniile 8–45): regula lipsea complet | Citind regulile și comparând cu codul | Verificare în `Room.Book` cu `nowUtc`, primit din `CreateBookingHandler` prin `TimeProvider` |
| R6 (maximum 4 ore, exact 4 ore e permis) | `Services/BookingService.cs`, metoda `Create`: regula lipsea | Citind codul | `Booking.MaxDuration` și verificarea `range.Duration > MaxDuration` în `Room.Book` |
| Persistența (nivelul C: datele trebuie să supraviețuiască unui restart) | `Repositories/RoomRepository.cs` și `BookingRepository.cs`, linia 5: liste în memorie, înregistrate ca Singleton în `Program.cs` (~liniile 9–10) | Comentariul din cod + am repornit aplicația și camera dispăruse | EF Core + SQLite (`RoomHubDbContext`, fișierul `roomhub.db`) prin `IRoomRepository`, `IBookingRepository` și `IUnitOfWork` |
| Erorile de business ca excepții (cerința 3 din assignment) | `Services/BusinessException.cs` + `Middleware/ExceptionMiddleware.cs` | Citind codul | Le-am șters; acum se folosesc `Result<T>` și `Error`, iar `ErrorHttpExtensions.ToProblem` mapează la 400/404/409 |

