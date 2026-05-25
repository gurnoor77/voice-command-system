# Voice Command System

A full-stack voice command system built with C#, ASP.NET Core, React, and SQL Server.

## Tech Stack

- **Desktop App** — C# Windows Forms, System.Speech.Recognition
- **Backend API** — ASP.NET Core 8, REST API
- **Frontend** — React 18, TypeScript, Vite, TailwindCSS, Recharts
- **Database** — SQL Server Express
- **Features** — ACID Transactions, Memory Caching, Rate Limiting, Serilog Logging, CORS, Swagger

## Features

- Voice recognition with 12 built-in commands
- Real-time web dashboard with analytics
- Command history with search and delete
- REST API with Swagger documentation
- ACID-compliant database transactions
- Rate limiting (10 requests per 10 seconds)
- Memory caching (30 second TTL)
- Structured logging with Serilog

## Project Structure

- VoiceCommandApp — C# Windows Forms desktop app
- VoiceCommandAPI — ASP.NET Core 8 REST API
- voice-command-dashboard — React + TypeScript frontend

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/commands | Get all commands |
| POST | /api/commands | Save a command |
| DELETE | /api/commands | Delete all commands |
| DELETE | /api/commands/{id} | Delete one command |
| GET | /api/commands/stats | Get usage statistics |

## Setup

1. Create SQL Server database
2. Update connection string in App.config and appsettings.json
3. Run VoiceCommandAPI (ASP.NET Core)
4. Run VoiceCommandApp (Windows Forms)
5. Run voice-command-dashboard (npm run dev)

## Developer

**Gurnoor Singh** — B.E. Computer Science, Chandigarh University (2023-2027)

- LeetCode Rating: 1687
- GitHub: github.com/gurnoor77