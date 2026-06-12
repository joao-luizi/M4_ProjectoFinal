# RideReady 🐴

Aplicação web de gestão para escolas de equitação: marcação de aulas online, gestão de alunos, professores e cavalos, controlo de presenças e pagamentos — tudo num só sítio.

Projeto final do Módulo 4 (curso de programação C#/.NET), desenvolvido a partir de levantamento de requisitos junto de uma escola de equitação real.

---

## O problema

Numa escola de equitação tradicional, as marcações fazem-se por telefone e a gestão de recursos (cavalos, professores, horários) é manual — o que sobrecarrega a administração e gera erros de agendamento. O RideReady automatiza este processo de ponta a ponta.

## Funcionalidades

**Alunos**
- Registo de conta com confirmação por email (wizard de 3 passos, RGPD)
- Consulta do calendário de aulas e marcação/desmarcação online
- Dashboard pessoal: próxima aula, créditos por produto, estado de pagamentos, histórico
- Bloqueio automático de marcações quando há pagamentos em atraso

**Professores**
- Dashboard com a próxima aula e respetivos alunos
- Criação e edição de aulas dentro do horário disponível
- Registo de presenças por aula

**Administradores**
- Dashboard com indicadores da escola (alunos, aulas, cavalos, pagamentos em falta)
- Gestão de utilizadores, escolas e cavalos
- Exportação da informação de alunos para Excel
- Notificação por email aos alunos em caso de cancelamento/alteração de aula

**Regras de negócio dos cavalos**
- Máximo de 2 aulas/dia por cavalo, com 2 dias de descanso semanais
- Regras específicas para passeios (até 4/dia, com restrições de combinação com aulas)

## Stack tecnológica

| Camada | Tecnologia |
|---|---|
| Frontend | Blazor Server (.NET 8) + Radzen Components 6.6.1 |
| Backend | ASP.NET Core 8 |
| API | ASP.NET Core Web API + JWT (Swagger incluído) |
| Dados | Entity Framework Core 8 + SQL Server (LocalDB em desenvolvimento) |
| Autenticação | ASP.NET Core Identity |
| Exportação | ClosedXML (Excel) |
| Logging | Serilog (consola + ficheiro, em `RideReady/Logs/`) |
| Email | SMTP (smtp4dev/Papercut em desenvolvimento) |

## Arquitetura

Solução com três projetos e **duas bases de dados separadas** — uma para identidade/utilizadores e outra para o domínio da escola:

```
M4_ProjectoFinal/
├── RepositoryLibrary/            # Domínio e acesso a dados (camada partilhada)
│   ├── Data/
│   │   ├── Context/              # AppIdentityDbContext + RideReadyDbContext
│   │   ├── Migrations/           # Migrações separadas por contexto
│   │   └── Seeds/                # Dados iniciais
│   └── Features/                 # Organização por funcionalidade (vertical slices):
│                                 # Account, Bookings, DashBoard, Entitlements,
│                                 # Horses, Lessons, Products, Purchases, Schools, Users
│                                 # (cada uma com DTOs / Entities / Interfaces /
│                                 #  Repositories / Services)
├── RideReady/                    # Aplicação Blazor Server (frontend)
│   ├── Components/
│   │   ├── Features/             # Páginas e modais por funcionalidade
│   │   ├── Layout/               # MainLayout, NavMenu
│   │   ├── Shared/               # Componentes reutilizáveis (StatCard, EmptyState, UserAvatar)
│   │   └── Pages/                # Landing page, páginas de erro temáticas (404/401/403)
│   ├── Services/                 # Serviços de UI (ex.: ToastService)
│   └── wwwroot/
│       ├── css/rideready-theme.css    # Fundação visual (tokens, paleta, movimento)
│       └── js/rideready-motion.js     # Camada de animação
└── RideReadyAPI/                 # Web API com autenticação JWT
    └── Features/Auth/
```

Cada feature agrupa DTOs, entidades, interfaces, repositórios e serviços (*vertical slices*), em vez de camadas horizontais por tipo.

## Como executar

### Pré-requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server LocalDB (incluído no Visual Studio)
- Ferramentas EF Core: `dotnet tool install --global dotnet-ef`
- Um servidor SMTP de desenvolvimento para os emails — [smtp4dev](https://github.com/rnwood/smtp4dev) (`dotnet tool install -g Rnwood.Smtp4dev`) ou [Papercut SMTP](https://github.com/ChangemakerStudios/Papercut-SMTP), à escuta no porto 25

### Configuração
1. Clonar o repositório e abrir `RideReady.sln`
2. Criar `RideReady/appsettings.json` a partir do `appsettings.example.json` (connection strings `IdentityDb` e `RideReadyDB`, e secção `Smtp`)
3. Aplicar as migrações às **duas** bases de dados:

```bash
dotnet ef database update --context AppIdentityDbContext --project RepositoryLibrary --startup-project RideReady
dotnet ef database update --context RideReadyDbContext --project RepositoryLibrary --startup-project RideReady
```

(Guia completo de migrações em [`docs/dev/database-migrations.md`](docs/dev/database-migrations.md).)

4. Executar:

```bash
dotnet run --project RideReady
```

A aplicação fica disponível em `https://localhost:56116`.

> ⚠️ **Nota:** o servidor SMTP de desenvolvimento deve estar ligado antes de testar o registo de contas ou a recuperação de password — o envio de email de confirmação faz parte desses fluxos.

## Documentação

> 📚 **Nota para futuros desenvolvedores deste projeto:** este README é apenas o ponto de partida. A documentação mais elaborada vive na pasta [`Docs/`](Docs/) — planeamento, notas de desenvolvimento, decisões de arquitetura e material de apresentação — e em [`docs/dev/`](docs/dev/) encontra-se a documentação técnica de desenvolvimento (ex.: o guia de migrações da base de dados). Antes de mexer no código, vale a pena passar por lá: muitas das decisões que vais questionar já estão explicadas.

## Equipa

Este projeto teve **duas fases de desenvolvimento**: foi iniciado por uma primeira equipa e posteriormente retomado pela equipa atual — a segunda a trabalhar no código —, que fez o levantamento do estado do trabalho, consolidou a base existente e o levou até à versão atual.

**Equipa atual:**

| | Área |
|---|---|
| **João Luizi** | Backend, Web API, base de dados |
| **Susana Ribeiro** | Frontend, UI/UX, documentação, exportação Excel, email e logging |

Desenvolvido com metodologia ágil (sprints de 3 dias) e recurso assistido a IA, de forma declarada.

## Próximos passos

- Transferência de cavalos entre escolas
- Marcação de "aula livre" para cavaleiros com cavalo próprio
- Fotografias dos cavalos carregadas pelos utilizadores
- Repetição de aulas na criação (recorrência semanal)
- Associação de pistas da escola às aulas

---

*Projeto académico — Módulo 4, 2026.*
