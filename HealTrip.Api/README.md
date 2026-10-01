# HealTrip AI — Backend

HealTrip AI is a prototype patient decision-support assistant built with ASP.NET Core and OpenAI.

The backend demonstrates how an AI assistant can understand a patient's request, ask for clarification when needed, call controlled backend tools, retrieve real data from PostgreSQL, and return grounded responses without inventing doctors or hospitals.

> This project is a technical-assessment prototype and is not intended for real-world medical diagnosis or clinical decision-making.

---

## Overview

The backend provides:

- AI-powered conversational patient decision support
- OpenAI tool/function calling
- Doctor search
- Hospital search
- PostgreSQL database
- Entity Framework Core
- Conversation context
- Arabic / English responses
- RTL / LTR-ready frontend support
- Basic emergency-symptom safety guardrail
- Database-grounded responses
- Prevention of hallucinated doctors and hospitals

---

## Architecture

```text
                    ┌──────────────────┐
                    │   Angular UI     │
                    │ Arabic / English │
                    └────────┬─────────┘
                             │
                             ▼
                    ┌──────────────────┐
                    │ ASP.NET Core API │
                    └────────┬─────────┘
                             │
                             ▼
                    ┌──────────────────┐
                    │  AI Orchestrator │
                    └───────┬──────────┘
                            │
                  ┌─────────┴─────────┐
                  ▼                   ▼
          ┌──────────────┐    ┌──────────────┐
          │  OpenAI LLM  │    │ Safety Guard │
          └──────┬───────┘    └──────────────┘
                 │
          Function Calling
                 │
        ┌────────┴─────────┐
        ▼                  ▼
┌───────────────┐  ┌────────────────┐
│ Doctor Tool   │  │ Hospital Tool  │
└───────┬───────┘  └───────┬────────┘
        │                  │
        └────────┬─────────┘
                 ▼
        ┌─────────────────┐
        │ PostgreSQL DB   │
        └─────────────────┘





Architecture Principles

The LLM does not access PostgreSQL directly.

Instead:

The user sends a message to the API.
The safety layer checks for predefined urgent-symptom indicators.
Normal requests are passed to the AI orchestrator.
The AI determines whether a tool is required.
The backend executes the requested tool.
The tool retrieves controlled data through backend services.
The result is returned to the AI model.
The AI generates the final response using the returned data.

This keeps database access under backend control and reduces the risk of hallucinated healthcare entities.



AI Agent Flow

A typical request follows this flow:
User Message
     │
     ▼
Safety Check
     │
     ├── Potentially Urgent
     │        │
     │        ▼
     │   Safety Response
     │
     └── Normal Request
              │
              ▼
        AI Orchestrator
              │
              ▼
           OpenAI
              │
              ▼
        Tool Selection
              │
        ┌─────┴─────┐
        ▼           ▼
 Search Doctors  Search Hospitals
        │           │
        └─────┬─────┘
              ▼
         PostgreSQL
              │
              ▼
        Tool Results
              │
              ▼
            OpenAI
              │
              ▼
       Final Response






AI Tools

The AI agent currently has two database-backed tools.

search_doctors

Searches doctors that exist in the HealTrip database.

Supported filters:
{
  "specialty": "Cardiology",
  "city": "Riyadh"
}


The tool calls the backend doctor service, which queries PostgreSQL through Entity Framework Core.

Example request:

I want to find a cardiologist in Riyadh.

The AI can call:

search_doctors

with:

{
  "specialty": "Cardiology",
  "city": "Riyadh"
}




The returned database records are then provided back to the AI model.

search_hospitals

Searches hospitals that exist in the HealTrip database.

Supported filters:

{
  "city": "Riyadh",
  "specialty": "Cardiology",
  "hasEmergency": true
}

The tool calls the backend hospital service, which queries PostgreSQL through Entity Framework Core.

Hallucination Prevention

A core requirement of the prototype is that the AI must not invent doctors or hospitals.

The system follows these rules:

The AI uses tools when searching for doctors or hospitals.
Database results are treated as the source of truth.
Only doctors and hospitals returned by the tools are presented to the user.
If no matching records are found, the AI reports that no matching records exist.
The AI must not claim doctor availability unless the database provides availability information.
The AI must not invent prices, phone numbers, addresses, or other unavailable information.

For example, if the user asks:

Find emergency hospitals in Dammam.

and the database contains no matching records, the assistant reports that no matching hospitals were found instead of generating fictional hospitals.

Safety Guardrail

The backend includes a SafetyTriageService that provides a basic deterministic guardrail for potentially urgent symptoms.

The prototype checks for predefined indicators such as:

English
Severe chest pain
Difficulty breathing
Shortness of breath
Fainting
Loss of consciousness
Heavy bleeding
Arabic
ألم شديد في الصدر
ألم شديد بالصدر
صعوبة في التنفس
ضيق في التنفس
ضيق نفس
إغماء
فقدان الوعي
نزيف شديد

When a matching indicator is detected, the backend returns an urgent-care response before sending the request to the AI model.

Important Limitation

This is a prototype safety guardrail, not a clinically validated triage system.

A production healthcare application would require validated clinical protocols, medical review, extensive testing, monitoring, and appropriate regulatory compliance.

Conversation Context

The backend maintains conversation history using a ConversationStore.

Each conversation is identified by a conversationId.

Example:

Conversation ID: context-test-001

User:
I want to find a cardiologist in Riyadh.

Assistant:
Dr. Ahmed Hassan — 12 years
Dr. Sara Mohammed — 9 years

User:
Which one has more experience?

Assistant:
Dr. Ahmed Hassan has 12 years of experience.

The same conversationId allows the AI to understand references such as:

Which one has more experience?

based on the previous conversation.

Prototype Limitation

The current implementation uses an in-memory conversation store.

This is suitable for the technical assessment prototype.

A production implementation would use persistent or distributed conversation storage such as a database or distributed cache/session store.


Arabic / English Support

The AI agent supports Arabic and English conversations.

The model is instructed to detect the language of the user's message and respond in the same language.

Examples:

Arabic:
عايز دكتور قلب في الرياض

English:
I want to find a cardiologist in Riyadh.

Mixed-language messages are also supported.

Example:

عايز cardiologist في Riyadh

The frontend additionally supports:

Arabic
English
RTL
LTR

Database entity names are kept based on the values returned from the database rather than being arbitrarily translated.

Database

The project uses PostgreSQL with Entity Framework Core.

Entities
Hospital
│
├── Id
├── Name
├── City
├── Address
├── HasEmergency
├── Specialties
└── Doctors

Doctor
│
├── Id
├── Name
├── Specialty
├── YearsOfExperience
├── Languages
├── City
└── HospitalId

Relationship:

Hospital 1 ─────────── * Doctor
Mock Data

The project contains mock healthcare data for demonstration purposes.

Hospitals
HealTrip Medical Center — Riyadh
Riyadh Care Hospital — Riyadh
Jeddah Health Hospital — Jeddah
Doctors
Dr. Ahmed Hassan — Cardiology — 12 years
Dr. Sara Mohammed — Cardiology — 9 years
Dr. Omar Ali — Internal Medicine — 15 years

This data is fictional mock data and is not intended to represent real healthcare providers.

API Endpoints
Chat
POST /api/chat

Request:

{
  "conversationId": "conversation-001",
  "message": "I want to find a cardiologist in Riyadh."
}

Response:

{
  "conversationId": "conversation-001",
  "message": "..."
}
Doctor Search
GET /api/doctors/search

Example:

GET /api/doctors/search?specialty=Cardiology&city=Riyadh
Example Scenarios
1. Emergency Scenario

Request:

{
  "conversationId": "test-emergency-001",
  "message": "I have severe chest pain and difficulty breathing."
}

Expected behavior:

Safety guard detects urgent indicators.
The request does not require a doctor-search tool.
The assistant recommends urgent medical evaluation.
No diagnosis is provided.
2. Doctor Search

Request:

{
  "conversationId": "test-doctor-001",
  "message": "I want to find a cardiologist in Riyadh."
}

Expected behavior:

AI calls search_doctors.
Backend searches PostgreSQL.
Matching doctors are returned.
AI responds using only returned database records.
3. No Matching Hospital

Request:

{
  "conversationId": "test-hospital-001",
  "message": "Find emergency hospitals in Dammam."
}

Expected behavior:

AI calls search_hospitals.
Database returns no matching records.
AI clearly reports that no matching hospitals were found.
No hospitals are invented.
4. Conversation Context

First request:

{
  "conversationId": "context-test-001",
  "message": "I want to find a cardiologist in Riyadh."
}

Follow-up request:

{
  "conversationId": "context-test-001",
  "message": "Which one has more experience?"
}

Expected behavior:

The assistant understands that "Which one" refers to the doctors discussed earlier in the same conversation.

Project Structure
HealTrip.Api/
│
├── Controllers/
│   ├── ChatController.cs
│   └── DoctorsController.cs
│
├── Data/
│   ├── HealTripDbContext.cs
│   └── SeedData.cs
│
├── DTOs/
│   ├── ChatRequest.cs
│   ├── DoctorSearchRequest.cs
│   ├── DoctorSearchResult.cs
│   ├── HospitalSearchRequest.cs
│   └── HospitalSearchResult.cs
│
├── Models/
│   ├── Doctor.cs
│   └── Hospital.cs
│
├── Options/
│   └── OpenAiOptions.cs
│
└── Services/
    │
    ├── Ai/
    │   ├── AiOrchestrator.cs
    │   ├── AiSystemPrompt.cs
    │   ├── ConversationStore.cs
    │   ├── SafetyTriageService.cs
    │   ├── ToolDefinitions.cs
    │   │
    │   └── Tools/
    │       ├── DoctorSearchTool.cs
    │       └── HospitalSearchTool.cs
    │
    ├── Doctors/
    │   ├── DoctorService.cs
    │   └── IDoctorService.cs
    │
    └── Hospitals/
        ├── HospitalService.cs
        └── IHospitalService.cs
Technology Stack
.NET 10
ASP.NET Core Web API
C#
Entity Framework Core
PostgreSQL
Npgsql
OpenAI API
Docker
REST API
Configuration

The application requires an OpenAI API key.

The API key should never be committed to Git.

For local development, ASP.NET Core User Secrets can be used:

dotnet user-secrets init
dotnet user-secrets set "OpenAI:ApiKey" "YOUR_API_KEY"

The application reads:

OpenAI:ApiKey
OpenAI:Model

from configuration.

PostgreSQL

PostgreSQL is provided through Docker Compose.

Start the database:

docker compose up -d

The development database uses:

Database: healtrip
Username: healtrip
Port: 5432

Database credentials should be managed through environment-specific configuration and should not be committed to source control.

Database Migrations

Create a migration:

dotnet ef migrations add InitialCreate

Apply the migration:

dotnet ef database update
Running the Backend
1. Start PostgreSQL
docker compose up -d
2. Configure the OpenAI API key
dotnet user-secrets set "OpenAI:ApiKey" "YOUR_API_KEY"
3. Apply database migrations
dotnet ef database update
4. Start the API
dotnet run

The API URL will be displayed in the terminal.

Security Considerations

The prototype includes several basic security considerations:

OpenAI API keys are kept outside source control.
The LLM does not have direct database access.
Database queries are executed by backend services.
Tool access is explicitly defined.
Tool results are controlled by backend code.
Frontend Markdown output is sanitized before rendering.
CORS is restricted to the Angular development origin.

For a production implementation, additional security controls would be required, including:

Authentication
Authorization
Rate limiting
Request size limits
Centralized exception handling
Structured audit logging
Secret management
PII protection
Security monitoring
HTTPS enforcement
Error Handling

The backend uses:

Request validation
Cancellation tokens
Controlled tool execution
Database query limits
Invalid tool parameter handling
HTTP-level error responses

A production implementation could further add:

Global exception handling middleware
Structured logging
Distributed tracing
Retry policies
Circuit breakers
Monitoring and alerting
Current Prototype Limitations

This project intentionally keeps the scope small for the technical assessment.

Current limitations include:

Mock healthcare data
In-memory conversation storage
Basic keyword-based safety guardrail
No authentication
No real doctor availability system
No appointment booking
No real patient records
No clinical diagnosis
No production-grade observability
Limited medical-language understanding

These limitations are intentional and would require additional design and validation in a production healthcare platform.