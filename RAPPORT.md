# Teknisk rapport – De viktigaste säkerhetsvalen i inlämningsuppgift för inloggnings-/registreringssida (SecureLoginApi)

## Live demo

API:t är deployat till Azure App Service och kan testas live via Swagger:
https://secureloginapi-b9fyfqafacg2decx.swedencentral-01.azurewebsites.net/swagger

## Hur skyddas användarnas lösenord?

Lösenord hashas med **BCrypt** (`BCrypt.Net-Next`) innan de sparas i databasen (**OWASP A02**). BCrypt genererar automatiskt ett unikt, slumpmässigt **salt** per lösenord (**OWASP A02**), vilket gör att samma lösenord aldrig ger samma hash två gånger. Det skyddar mot rainbow table-attacker. BCrypt är dessutom medvetet långsam, vilket gör brute force-attacker opraktiska. Lösenord i klartext sparas aldrig, varken i databasen eller i loggar.

## Hur förhindras att en användare registreras flera gånger?

Innan en ny användare skapas kontrollerar `AuthService` om användarnamnet redan finns via `IUserRepository.Exists()`. Sökningen sker via Entity Framework Core, som automatiskt genererar parametriserade SQL-frågor och därmed skyddar mot SQL injection (**OWASP A03**). Om användarnamnet redan finns kastas ett `InvalidOperationException`, som fångas i `AuthController` och returneras som **409 Conflict** med meddelandet "Användarnamnet finns redan" (**OWASP A07**). Detta hanteras utan att applikationen kraschar.

## Hur hanteras fel vid inloggning utan att avslöja för mycket information?

Vid misslyckad inloggning – oavsett om det är användarnamnet eller lösenordet som är fel – returneras samma generiska svar: **401 Unauthorized**, "Fel användarnamn eller lösenord" (**OWASP A07**). Detta förhindrar att en angripare kan avgöra om ett specifikt användarnamn existerar i systemet (user enumeration). Inloggningsendpointen är dessutom skyddad med rate limiting, som tillåter max 5 inloggningsförsök per minut per klient och därefter returnerar 429 Too Many Requests, vilket motverkar brute force-attacker (**OWASP A07**). Alla misslyckade inloggningsförsök loggas via `ILogger` för spårbarhet (**OWASP A09**).

## Vilka sårbarheter (OWASP Top 10) har undvikits?

| Kategori | Åtgärd | ISO 27001/27002 |
|---|---|---|
| **A02 – Cryptographic Failures** | Lösenord hashas med BCrypt och unikt salt, sparas aldrig i klartext | Cryptography |
| **A03 – Injection** | EF Core genererar parametriserade SQL-frågor automatiskt, vilket skyddar mot SQL injection | Secure Coding |
| **A05 – Security Misconfiguration** | Connection string till Azure SQL hanteras via `dotnet user-secrets` lokalt, sparas aldrig i kod eller Git | Data Leakage Prevention |
| **A06 – Vulnerable Components** | `dotnet list package --vulnerable` bekräftar att inga använda NuGet-paket har kända sårbarheter | Vulnerability Management |
| **A07 – Authentication Failures** | Dubblettkontroll vid registrering, generiska felmeddelanden vid inloggning, samt rate limiting (max 5 inloggningsförsök/minut) mot brute force | Authentication |
| **A09 – Logging Failures** | Misslyckade inloggningsförsök loggas via `ILogger` | Logging and Monitoring |

### Delvis uppfyllda sårbarheter enligt OWASP Top 10

| Kategori | Vad som är gjort | Vad som saknas för full efterlevnad |
|---|---|---|
| **A04 – Insecure Design** | Lagerindelad arkitektur (Controller → Service → Repository) med interface och Dependency Injection, vilket ger säkra, utbytbara komponenter och tydlig ansvarsfördelning | Ingen formell hotmodellering (threat modeling) har genomförts, och säkra designmönster har inte tillämpats systematiskt genom hela utvecklingsprocessen |

### Övriga sårbarheter enligt OWASP Top 10

| Kategori | Ej tillämplig – motivering |
|---|---|
| **A01 – Broken Access Control** | API:t har ännu inga skyddade resurser utöver inloggning |
| **A08 – Data Integrity Failures** | Applikationen varken deserialiserar opålitlig data eller hanterar automatiska uppdateringar |
| **A10 – Server-Side Request Forgery** | API:t gör inga externa anrop baserat på användarinput |

## Arkitektur

Arkitekturen bygger på separation of concerns genom lagerindelning med Dependency Injection och är uppbyggd med utbytbara komponenter via `IUserRepository`, vilket gjorde det möjligt att gå från minneshanterad lagring till databaslagring i Azure SQL.

Koden är genomgående kommenterad i större utsträckning än vad som normalt görs i produktionskod. Det är ett medvetet val eftersom detta är ett skolarbete där koden förklaras mer detaljerat – i ett verkligt projekt hade kommentarerna varit betydligt mer sparsamma i main/master.