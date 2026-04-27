# XRechnung Validator

Eine C# Webapp zur Validierung von XRechnungen (deutsche E-Rechnungen) mit dem KoSIT Validator.

## Voraussetzungen

- Docker Desktop
- Git

## Installation und Start

1. Repository klonen:
```bash
git clone https://github.com/RhonyEbanga/xrechnung-validator.git
cd xrechnung-validator
```

2. KoSIT Dateien herunterladen und in `kosit-config/` ablegen:
   - [validator-1.6.2-standalone.jar](https://github.com/itplr-kosit/validator/releases)
   - [xrechnung-3.0.2-validator-configuration-2026-01-31.zip](https://github.com/itplr-kosit/validator-configuration-xrechnung/releases) entpacken

3. App starten:
```bash
docker-compose up --build
```

4. Browser öffnen: http://localhost:5000
## Funktionen

- XML Datei hochladen
- Validierung starten
- Ergebnis anzeigen (gültig/ungültig)
- Drei Validierungsbereiche anzeigen:
  - Syntaxschema
  - EN 16931 Rules
  - XRechnung Rules
- Validierungsbericht herunterladen

## Architektur

Browser (Frontend)
↕ HTTP
ASP.NET Core Backend (C#) - Port 5000
↕ HTTP
KoSIT Validator (Java, Docker) - Port 8080

## Technologien

- C# / ASP.NET Core 9.0
- Docker / Docker Compose
- KoSIT Validator 1.6.2
- XRechnung Konfiguration 3.0.2
- HTML / CSS / JavaScript