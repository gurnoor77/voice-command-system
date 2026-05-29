# Voice Command System

A full-stack AI-powered voice command system built with C#, ASP.NET Core, React, and SQL Server.

## Tech Stack

| Layer | Technology |
|-------|------------|
| Desktop App | C# Windows Forms, System.Speech.Recognition |
| Backend API | ASP.NET Core 8, REST API, SignalR |
| AI Layer | Groq AI LLaMA 3.3, Natural Language Intent Recognition |
| Frontend | React 18, TypeScript, Vite, TailwindCSS, Recharts |
| Database | SQL Server Express |

## Features

- Voice recognition with natural language understanding via Groq AI
- Say anything naturally, AI detects the intent automatically
- Real-time web dashboard with SignalR WebSockets
- Command history with search and delete
- REST API with Swagger documentation
- ACID-compliant database transactions
- Rate limiting 10 requests per 10 seconds
- Memory caching 30 second TTL
- Structured logging with Serilog

## How It Works

User speaks → System.Speech captures → Groq AI identifies intent → Command executes → SignalR updates React dashboard

## Project Structure

| Folder | Description |
|--------|-------------|
| VoiceCommandApp | C# Windows Forms desktop app |
| VoiceCommandAPI | ASP.NET Core 8 REST API |
| voice-command-dashboard | React TypeScript frontend |

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/commands | Get all commands |
| POST | /api/commands | Save a command |
| POST | /api/commands/recognize | AI intent recognition |
| DELETE | /api/commands | Delete all commands |
| DELETE | /api/commands/{id} | Delete one command |
| GET | /api/commands/stats | Get usage statistics |

## Setup

1. Create SQL Server database
2. Update connection string in App.config and appsettings.json
3. Add Groq API key to appsettings.Development.json
4. Run VoiceCommandAPI
5. Run VoiceCommandApp
6. Run voice-command-dashboard with npm run dev

## Developer

**Gurnoor Singh** - B.E. Computer Science, Chandigarh University 2023-2027

GitHub: https://github.com/gurnoor77
