# **C#2 - Projekt:** Evidence Coworkingových center

**Toto je projekt do předmětu [Programování v C# 2](https://www.fei.vsb.cz/460/cs/kontakt/lide/michal-radecky)**

**Autor:** Jan Bojko 3. Ročník, obor Informatika. [VŠB-TUO](https://www.vsb.cz/cs/) 2025

**Téma:** Webová + Desktopová aplikace pro **Evidenci Coworkingových center** _(Bližší specifikace zadaného téma níže)_

## Jaký tech stack projekt používá?

V tomto projektu jsem se rozhodl kromě povinného **C# a ASP.NET** použít další technologie.

- [C#](https://learn.microsoft.com/dotnet/csharp/)
- [ASP.NET MVC](https://dotnet.microsoft.com/apps/aspnet)
- [EntityFramework](https://learn.microsoft.com/en-us/ef/)
- [Bootstrap](https://getbootstrap.com/)
- [SQLite](https://sqlite.org/)
- [JavaScript](https://www.javascript.com/)

## 1. Část - Webová aplikace

- **Bude poskytovat API**, které bude pokrývat kompletní funkcionalitu potřebnou pro webovou i desktopovou část. **Komunikace bude využívat formát JSON**. Veškeré implementace ve vazbě na API budou využívat **asynchronní přístup**.

- Bude zajišťovat přístup k databázi a práci s daty, a to s využitím dostupné vrstvy ORM (např. Dapper nebo EntityFramework).

- Bude nabízet komplexní funkcionalitu pro zajištění funkčnosti dle odpovídajícího tématu, viz níže.

- Bude implementována problematika **registrace uživatelů** a jejich **přihlašování**. Typy uživatelských rolí a konkrétní oprávnění budou minimálně **_"správce" (přístup ke všemu)_** a **_"jen pro čtení" (pouze nahlížení na data)_**.

- Veškeré vstupní/editační formuláře budou implementovat funkcionalitu **validace dat**, a to přinejmenším na straně serveru.

- Webová aplikace bude obsahovat funkcionalitu pro vizualizaci dat s využitím komponent třetích stran např. grafy či mapa.

- Součást implementace bude **API endpoint**, která bude vracet **_popis rozhraní API ve formátu JSON (inspirace v rámci Open API)_**. K tomuto musí být využita reflexe. Popis API musí obsahovat vše potřebné pro dokumentaci komunikace s tímto API (adresa, HTTP metoda, parametry volání, popis návratových hodnot atd.).

- Aplikace bude vyvinuta v rámci platformy **ASP.NET MVC.**

## 2. Část - Desktopová aplikace

- Aplikace bude z pohledu funkčnosti poskytovat **stejné možnosti jako webová aplikace** _(nebude však obsahovat vizualizaci dat formou grafů či mapy)_, a to v roli správce. Není třeba řešit přihlašování, registraci a uživatelské role.

- Aplikace nebude přímo přistupovat k databázi, ale bude využívat API vyvinuté v rámci webové části aplikace. Veškeré implementace ve vazbě na API budou využívat asynchronní přístup.

- Aplikace bude vyvinuta v rámci platformy **WPF** (_případně MAUI_).

- Aplikace bude u všech formulářů řešit odpovídající **validaci zadaných hodnot**.

## Bližší Specifikace Zadání

### **Evidence coworkingových prostor**

#### **Evidované informace**:

- **Coworkingový prostor** vč. GPS souřadnic
- Jednotlivá **pracovní místa** _(jeden prostor může obsahovat N pracovních míst)_
- **Stav pracovního místa** _(dostupné, obsazené, v údržbě)_ a **historie změn stavů v čase**
- **Obsazení pracovního místa** _(email zákazníka, čas začátku a konce, délka, cena)_

#### **Specifická funkcionalita**:

- **Zobrazení Coworkingových center na mapě** včetně aktuální dostupnosti pracovních míst (Jen ve webové applikaci)
- **Možnost provést obsazení konkrétního místa** (speciální stránka/dialog pro zaznamenání obsazení a jejího průběhu)
- Stav obsazení lze měnit přímo pouze pokud je "dostupné" a "v údržbě", nelze měnit stav bez vazby na proces obsazení
- Již ukončené obsazení nelze zpětně upravovat ani mazat (kontrola omezení jak v UI, tak na straně dat)
- Historie stavů pracovních míst se zaznamenává automaticky při změně aktuálního stavu, není nutná plná editace, pouze náhled na data.
- **Zobrazení statistiky**, která ukáže počet ukončených využití pracovních míst za poslední měsíc (příp. volitelně) pro jednotlivá coworkingová centra
