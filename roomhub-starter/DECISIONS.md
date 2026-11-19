## 1. A rule you moved
Am ales R7 (două rezervări active nu se pot suprapune). În starter era în `Services/BookingService.cs`, într-un `foreach` din metoda `Create`, și era greșită (folosea `<=`). Acum comparația e în `Domain/Bookings/DateRange.cs` (`Overlaps`), iar decizia „sala e ocupată” e în `Domain/Rooms/Room.cs` (`Book`). Am pus-o acolo pentru că `Room` are rezervările sale și știe cel mai bine dacă un interval nou intră în conflict. Așa regula nu poate fi ocolită de un alt endpoint sau handler, iar handlerul doar o apelează.

## 2. Result instead of an exception
„Sala e ocupată” nu e o eroare de program, ci un caz normal de business, care se întâmplă des. O excepție ar ascunde asta: semnătura metodei nu arată că poate eșua, iar fluxul normal ar deveni un flux de eroare. Cu `Result<Booking>`, metoda `Room.Book` întoarce fie rezervarea, fie `BookingErrors.RoomOccupied`. `CreateBookingHandler` vede `IsFailure` și întoarce eroarea fără să salveze nimic. Endpoint-ul apelează `result.Error.ToProblem()`, care transformă tipul `Conflict` în 409 cu `ProblemDetails`.

## 3. Who knows about EF Core?
Despre EF Core știu doar clasele din `Infrastructure/Persistence`: `RoomHubDbContext`, configurațiile `RoomConfiguration` și `BookingConfiguration`, `RoomRepository`, `BookingRepository`, `UtcDateTimeConverter`, `DatabaseInitializer`. Mai știu de el și query handler-ele (`GetRoomByIdHandler`, `GetBookingByIdHandler`), pentru că pe partea de citire folosim direct `DbContext` cu `AsNoTracking`. Domeniul (`Room`, `Booking`, `DateRange`) și command handler-ele nu știu de EF: folosesc doar `IRoomRepository`, `IBookingRepository` și `IUnitOfWork`, definite în `Domain`. Câștigul: handler-ele pot fi testate cu fake-uri scrise de mână, iar regulile nu depind de baza de date. Costul: mai mult cod (interfețe + implementări) și riscul unui repository care doar pasează apeluri, așa că am pus în el doar metodele necesare.

## 4. The flow of a request
```
POST /api/rooms/{roomId}/bookings
  -> Features/Bookings/BookingsEndpoints.cs (MapGroup "/api/rooms/{roomId:guid}/bookings")
  -> CreateBookingEndpoint.Handle (Request -> CreateBookingCommand)
  -> ICommandHandler<CreateBookingCommand, CreateBookingResult> = CreateBookingHandler.Handle
       -> IRoomRepository.GetByIdAsync (RoomRepository -> RoomHubDbContext.Rooms + rezervările active)
            null -> RoomErrors.NotFound -> 404
       -> IBookingRepository.CountActiveForPersonOnDayAsync (R9)
       -> Room.Book(..., TimeProvider.GetUtcNow(), activeThatDay)
            -> DateRange.Create (R3), R2, R4, R5, R10, R6, DateRange.Overlaps (R7), R9
            -> Booking.Create (internal) -> adăugată în Room._bookings
            eroare -> Result.Failure -> ErrorHttpExtensions.ToProblem -> 400 / 409
       -> IUnitOfWork.SaveChangesAsync (RoomHubDbContext) -> INSERT în tabela Bookings (roomhub.db)
  -> TypedResults.Created("/api/bookings/{id}", Response { id, roomId, status }) -> 201
```

## 5. What you rejected from what the AI gave you
<!-- ATENȚIE: scrie aici un exemplu REAL, al tău. Variante care chiar s-au întâmplat în sesiune: -->
<!-- - AI-ul voia să instaleze .NET SDK în mediul lui ca să compileze; am refuzat și am verificat eu cu `dotnet build` în Rider. -->
<!-- - AI-ul a comparat `bookedBy` exact (case-sensitive) pentru R9; am decis/verificat dacă "Ana" și "ana" sunt aceeași persoană. -->
<!-- - `dotnet build` din folderul rădăcină nu mergea (nu e .csproj acolo); am rulat `dotnet build src/RoomHub.Api`. -->
AI-ul a vrut să instaleze .NET SDK în mediul lui ca să compileze proiectul. Am refuzat, pentru că pierdeam timp din examen, și am verificat eu build-ul în Rider cu `dotnet build src/RoomHub.Api`. Am observat și că pentru R9 compară `bookedBy` exact (case-sensitive), deci „Ana” și „ana” sunt persoane diferite. Am lăsat așa pentru că enunțul spune „același bookedBy”, dar știu că e o decizie discutabilă.

## 6. If you had 30 more minutes
Aș scrie teste: unit tests pentru `Room.Book`, `Booking.Cancel` și `DateRange` cu un `TimeProvider` fals, apoi teste de integrare cu `WebApplicationFactory<Program>` și SQLite în memorie. Riscul principal rămas e concurența: două cereri simultane pot citi aceleași rezervări și pot trece amândouă de R7 sau R9, pentru că nu am un concurrency token sau o tranzacție serializabilă. Am folosit și `EnsureCreated` în loc de migrații, deci la o schimbare de model trebuie ștearsă baza `roomhub.db`.
