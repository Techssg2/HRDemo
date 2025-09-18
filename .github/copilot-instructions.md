# HRDemo - Enterprise HR Management System

HRDemo is a comprehensive enterprise HR management system built with .NET Framework, ASP.NET Web API, and AngularJS. The system handles HR workflows, employee management, academy training, business trip applications, and various automated job processes.

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Working Effectively

### Bootstrap and Build Process
- **CRITICAL BUILD LIMITATION**: The full .NET Framework solution (Aeon.HR.sln) requires Visual Studio on Windows due to:
  - .NET Framework 4.6.1-4.8 targeting packs not available on Linux
  - SharePoint development tools dependencies
  - Web application deployment targets missing on non-Windows platforms
- **Frontend builds work perfectly**: All Node.js/AngularJS applications build and run successfully on Linux
- Build the frontend applications:
  - `cd Aeon.HR.API/ClientApp && npm install` -- takes 45-60 seconds. NEVER CANCEL. Set timeout to 180+ seconds.
  - `cd Aeon.Navigation/ClientApp && npm install` -- takes 45-60 seconds. NEVER CANCEL. Set timeout to 180+ seconds.
  - `cd "Aeon.HR.UI/Layouts/AeonHR/ClientApp" && npm install` -- takes 45-60 seconds. NEVER CANCEL. Set timeout to 180+ seconds.
- **dotnet restore works partially** for .NET Standard projects but fails on .NET Framework projects
- **Use mono/xbuild for experimental builds** but expect dependency resolution failures

### Run the Applications
- **Frontend Development Servers**:
  - `cd Aeon.HR.API/ClientApp && npm start` -- starts lite-server on http://localhost:3000
  - `cd Aeon.Navigation/ClientApp && npm start` -- starts lite-server on http://localhost:3000
  - `cd "Aeon.HR.UI/Layouts/AeonHR/ClientApp" && npm start` -- starts lite-server on http://localhost:3000
- **Backend APIs**: Cannot run without Windows/.NET Framework runtime and SQL Server database
- **NEVER CANCEL frontend servers** - they run indefinitely for development. Use Ctrl+C or stop_bash tool to terminate.

### Testing and Validation
- **Frontend validation**: Always test at least one AngularJS application by:
  1. Running `npm install` in a ClientApp directory
  2. Running `npm start` to launch lite-server
  3. Verifying HTTP 200 response from http://localhost:3000
  4. Checking that HTML content loads with AngularJS app structure
- **Dependency issues are expected**: Many npm packages show deprecation warnings and security vulnerabilities - this is normal for legacy AngularJS applications
- **No automated tests found**: The repository does not contain unit tests or e2e test configurations
- **No linting tools configured**: No ESLint, JSHint, or similar tools found in the project

### Timeout Requirements
- **npm install**: 180+ seconds timeout (can take 45-90 seconds)
- **npm start**: No timeout needed (runs continuously)
- **dotnet restore**: 300+ seconds timeout (complex dependency resolution)
- **dotnet build**: 600+ seconds timeout (would take 5-10+ minutes if successful)
- **xbuild attempts**: 180+ seconds timeout (fails due to missing dependencies)

## Common Tasks

### Repository Structure
```
Aeon.HR.sln                    # Main .NET solution (30+ projects)
├── Aeon.HR.API/               # Main HR Web API
│   └── ClientApp/             # AngularJS frontend (primary UI)
├── Aeon.Navigation/           # Navigation service
│   └── ClientApp/             # AngularJS navigation UI
├── Aeon.HR.UI/               # SharePoint HR UI components
│   └── Layouts/AeonHR/ClientApp/ # AngularJS HR forms
├── Aeon.Academy.API/         # Training management API
├── Job*/                     # Various background job processors
├── Aeon.HR.BusinessObjects/  # Business logic layer
├── Aeon.HR.Infrastructure/   # Infrastructure services
├── Aeon.HR.ViewModels/       # Data transfer objects
└── Multiple console apps     # HR workflow automation
```

### Technology Stack
- **Backend**: .NET Framework 4.6.1-4.8, ASP.NET Web API, Entity Framework 6.x
- **Frontend**: AngularJS 1.7.9, Kendo UI, Bootstrap 4.4.1, lite-server for development
- **Database**: SQL Server (connection strings in Web.config)
- **Background Jobs**: Hangfire for job scheduling
- **SharePoint**: Integration with SharePoint 2016 CSOM
- **Build Tools**: MSBuild (Windows only), npm for frontend assets

### Key Project Purposes
- **Aeon.HR.API**: Main HR REST API and primary AngularJS application
- **Aeon.Navigation**: Shared navigation services across HR modules
- **Aeon.Academy.API**: Training course management and academy workflows
- **Job* projects**: Automated background processes for HR workflows (resignation, promotion, notifications)
- **Aeon.HR.SP.API**: SharePoint-specific API endpoints
- **Console applications**: Batch processing for SAP integration and data sync

### Development Workflow Commands
```bash
# Setup frontend development environment
cd Aeon.HR.API/ClientApp
npm install                    # Install dependencies (45-60 seconds)
npm start                      # Start development server

# Test other frontend applications
cd ../../../Aeon.Navigation/ClientApp
npm install && npm start

# View application structure
curl http://localhost:3000     # Verify AngularJS app loads
```

### Known Issues and Workarounds
- **npm audit shows 90+ vulnerabilities**: Expected for legacy AngularJS projects. Upgrading would require major refactoring
- **Deprecated Angular packages**: AngularJS 1.x reached end-of-life but is still functional
- **Missing .NET Framework targeting packs**: Linux cannot build .NET Framework projects without Windows SDK
- **SharePoint dependencies**: Require Visual Studio SharePoint development tools
- **SQL Server integration**: Database connections will fail without proper SQL Server setup

### Validation Scenarios
When making changes to the codebase:
1. **Always test frontend builds**: Run `npm install` and `npm start` in affected ClientApp directories
2. **Verify application loads**: Check that http://localhost:3000 returns HTML with ng-app="ssg" 
3. **Check console for errors**: Monitor browser-sync output for file watching and serving
4. **Validate AngularJS structure**: Ensure scripts load properly and app.module.js is accessible

### Common File Locations
- **Main AngularJS app**: `/Aeon.HR.API/ClientApp/app/app.module.js`
- **Package definitions**: `*/ClientApp/package.json` (3 locations)
- **Frontend assets**: `*/ClientApp/assets/` and `*/ClientApp/newassets/`
- **Build outputs**: `bin/Debug/`, `bin/Release/` (git-ignored)
- **NuGet packages**: `packages/` (git-ignored)
- **Node modules**: `*/ClientApp/node_modules/` (git-ignored)

### Important Notes for Agents
- **DO NOT** attempt to build the full solution on Linux - it will fail due to platform requirements
- **DO** test frontend applications which work perfectly and represent the user-facing functionality
- **DO** use appropriate timeouts for npm operations (180+ seconds minimum)
- **DO NOT** try to fix npm audit vulnerabilities - they require major framework upgrades
- **DO** verify that lite-server starts successfully and serves the AngularJS applications
- **DO NOT** expect unit tests - none exist in this legacy enterprise codebase