# VC Hub Open API Demo Requirements Template

## 1. Document Information

- Project name: VC Hub Open API Demo
- Document owner: [Owner Name]
- Version: 0.1
- Last updated: 2026-04-09
- Status: Draft

## 1.1 Confirmed Decisions

- Target VC Hub baseline version: `5.1`
- Primary SignalR hub route for the demo: `ws/v/realtimedata`
- Primary SignalR stream scenarios: `TagValues` and `Alarms`
- Preferred sample UI technology: `Blazor`

## 2. Background

This project is a new demo application that shows how to integrate with the VC Hub Open API SignalR interface from a .NET application. The existing project under OldOpenApiDemo is an archived reference implementation and shall only be used as a reference for authentication design and SignalR connection implementation details. The new demo shall define its own application structure, scenarios, and feature scope independently from the old sample.

The test environment also exposes a Swagger document at `http://vpc-sz-scadap52:8066/swagger/v1/swagger.json`. That Swagger definition may be used as supplementary environment reference information, but it is not the primary contract source for this demo because the new project is focused on demonstrating the SignalR API rather than HTTP endpoint coverage.

## 3. Purpose

The purpose of this demo is to provide a clean reference implementation for:

- Authenticating to VC Hub Open API with client credentials.
- Establishing an authenticated SignalR connection to VC Hub.
- Demonstrating one or more real-time subscriptions exposed by the VC Hub SignalR API.
- Providing a simple sample UI for configuring the demo, starting the connection, and viewing live messages.
- Keeping the new implementation decoupled from the old demo except where authentication and SignalR connection behavior are intentionally reused.

## 4. Goals

- Provide a simple, runnable .NET sample for VC Hub Open API integration.
- Demonstrate how to configure the base URL, client ID, and client secret.
- Demonstrate how to retrieve an access token before establishing SignalR connections.
- Reuse the proven authentication flow from the old demo without carrying over unrelated legacy code.
- Demonstrate how to connect to the VC Hub SignalR endpoints over WebSockets.
- Demonstrate at least one SignalR stream subscription.
- Provide a simple sample UI that makes the demo easier to run and observe.
- Keep the sample small enough for onboarding and troubleshooting.

## 5. Non-Goals

- Production-grade UI or dashboard.
- Full SDK abstraction for all VC Hub endpoints.
- HTTP REST API demonstration.
- Complete error recovery and retry strategy for enterprise use.
- Long-term token/session persistence.
- Advanced deployment automation.
- Reproducing the full REST and SignalR examples from OldOpenApiDemo unless explicitly required later.

## 6. Intended Audience

- Developers integrating custom applications with VC Hub.
- Solution architects evaluating VC Hub Open API capabilities.
- QA or support engineers validating connectivity and credentials.

## 7. Scope

### In Scope

- .NET 10 sample application.
- Blazor-based sample UI.
- Configuration through appsettings.json.
- OAuth/OpenID Connect client credentials token retrieval.
- Reuse of the authentication and SignalR connection implementation approach from OldOpenApiDemo.
- Sample UI for demo configuration and live stream observation.
- SignalR hub connection setup for VC Hub real-time endpoints.
- WebSocket transport configuration for the SignalR client.
- Access token injection for SignalR connections.
- Demonstration of one or more SignalR subscriptions.
- UI and log output showing authentication, connection, subscription, and message receipt status.

### Out of Scope

- Database persistence.
- Multi-tenant authentication.
- Role and permission administration.
- API management or gateway setup.
- HTTP request and response demo scenarios.
- Migration of the full OldOpenApiDemo application structure.
- Carrying forward old REST or SignalR sample scenarios by default.

## 8. Assumptions

- VC Hub is already installed and reachable from the demo environment.
- Open API is enabled in VC Hub.
- A valid client ID and client secret are available.
- The demo environment is allowed to access the VC Hub HTTPS and WebSocket endpoints.
- The old demo's authentication and SignalR implementation are valid reference baselines.
- Suitable test tags or events exist so the selected SignalR stream can produce observable data.
- The target VC Hub environment for this demo is version `5.1`.

## 9. Stakeholders

- Requester: [Business or technical requester]
- Development owner: [Team or engineer]
- VC Hub administrator: [Name or team]
- Reviewers: [Names]

## 10. High-Level User Stories

- As a developer, I want to configure VC Hub connection settings so that I can run the sample against my environment.
- As a developer, I want the sample to acquire an access token automatically so that I do not need to call the token endpoint manually.
- As a developer, I want the new demo to reuse only the old authentication and SignalR connection implementation so that legacy demo logic does not constrain the new project.
- As a developer, I want to subscribe to a real-time VC Hub stream so that I can verify SignalR integration.
- As a user, I want a simple UI to connect, start a subscription, and inspect incoming real-time messages.
- As a support engineer, I want useful logs so that I can diagnose authentication, connection, and subscription issues.

## 11. Functional Requirements

### FR-1 Configuration

- The application shall read the following settings from configuration:
	- VC Hub base URL
	- Client ID
	- Client secret
- The application shall support environment-specific configuration overrides.

### FR-2 Authentication

- The application shall retrieve OpenID configuration from the VC Hub discovery endpoint.
- The application shall request an access token using the client credentials grant.
- The application shall provide the access token to SignalR connections using the access token provider pattern.
- The authentication implementation may reference the old demo project's token service and related configuration pattern.
- The new project shall not directly depend on unrelated service logic from OldOpenApiDemo.

### FR-3 SignalR Connection Foundation

- The application shall build SignalR hub connections using the configured VC Hub base URL.
- The application shall use WebSockets transport for the SignalR client.
- The application shall use `ws/v/realtimedata` as the primary SignalR hub route for the demo scenario.
- The application may support additional hub routes in later phases.
- The application shall be capable of starting and stopping hub connections cleanly.

### FR-4 Sample UI

- The demo shall include a sample UI.
- The sample UI shall be implemented with Blazor.
- The UI shall allow the user to provide or review the VC Hub base URL, client ID, and client secret or the effective configured values used by the application.
- The UI shall provide controls to connect and disconnect from the selected SignalR hub route.
- The UI shall allow the user to choose or trigger at least one supported SignalR stream scenario.
- The UI shall display connection state, selected stream, and the most recent received messages or events.
- The UI should make authentication, connection, and subscription failures visible without requiring the user to inspect only backend logs.

### FR-5 SignalR Subscription Demo

- The demo shall demonstrate the `TagValues` SignalR stream.
- The demo shall demonstrate the `Alarms` SignalR stream.
- `TagValues` and `Alarms` are the primary demo scenarios for the first implementation.
- The application shall log received messages from the selected stream.

### FR-6 Logging and Diagnostics

- The application shall log authentication success and failure events.
- The application shall log SignalR connection start, stop, and subscription events.
- The application shall log configuration validation errors.
- The application should emit enough detail to troubleshoot connectivity, token acquisition, and streaming issues.

### FR-7 Error Handling

- The application shall catch and log authentication, connection, and subscription errors.
- The application shall fail with readable log output when configuration is missing or invalid.

### FR-8 Security

- The application shall not hardcode production credentials in source code.
- The sample shall document that certificate validation bypass is for demo or test environments only.
- The sample should allow future replacement with proper server certificate validation.
- The sample shall avoid coupling security-sensitive code to unrelated demo behavior inherited from the old project.

## 12. Non-Functional Requirements

### NFR-1 Simplicity

- The code should be understandable to a developer new to VC Hub Open API.
- The sample UI should be simple and focused on demonstrating the SignalR workflow rather than polished product behavior.

### NFR-2 Maintainability

- The sample should separate configuration, token acquisition, and application logic into distinct classes.
- The reused authentication and SignalR connection approach should be refactored into the new project structure rather than copied wholesale with unused dependencies.

### NFR-3 Observability

- The sample should emit logs for startup, token acquisition, SignalR connection setup, subscription start, and failures.
- The sample UI should clearly reflect connection status and incoming message activity.

### NFR-4 Contract Traceability

- The selected demo streams and hub routes should be traceable back to validated VC Hub SignalR behaviors used in the test environment.

### NFR-5 Compatibility

- The sample shall run on .NET 10.

### NFR-6 Technology Choice

- The sample UI technology for the demo shall be Blazor.

## 13. Integration Requirements

### External System

- VC Hub Open API

### Test Environment Reference

- Supplementary Swagger JSON: `http://vpc-sz-scadap52:8066/swagger/v1/swagger.json`
- The Swagger document is environment reference material only and is not the primary integration surface for this demo.
- Target VC Hub version: `5.1`

### Required Endpoints

- Discovery endpoint: `/.well-known/openid-configuration`
- Token endpoint: resolved from discovery document

### Required SignalR Routes

- Primary route: `ws/v/realtimedata`

### Target SignalR Streams

- `TagValues`
- `Alarms`

### Legacy Reference Constraint

- OldOpenApiDemo may be referenced for authentication-related classes, configuration shape, token acquisition flow, and SignalR connection setup only.
- OldOpenApiDemo shall not be treated as the functional scope baseline for the new demo.

## 14. Configuration Template

```json
{
	"OpenApi": {
		"Url": "https://your-vc-hub-host:port",
		"ClientId": "your-client-id",
		"ClientSecret": "your-client-secret"
	}
}
```

## 15. Demo Scenarios

### Scenario A: Validate Authentication Configuration

- Start the demo application.
- Verify the application can retrieve an access token.
- Verify the sample UI shows whether authentication succeeded or failed.

### Scenario B: Establish SignalR Connection

- Initialize an authenticated SignalR connection using the acquired token.
- Verify the client can connect to `ws/v/realtimedata`.
- Verify the sample UI shows the current connection state.

### Scenario C: Receive Real-Time Messages

- Subscribe to the `TagValues` and `Alarms` streams.
- Confirm the application receives at least one message or event from one or both selected streams.
- Confirm the sample UI displays incoming messages or events.

### Scenario D: Validate Failure Paths

- Run the application with missing or invalid credentials.
- Confirm the application reports configuration, authentication, or SignalR connection failures clearly.
- Confirm the sample UI surfaces failure information to the user.

## 16. Acceptance Criteria

- A developer can clone the project, set configuration values, and run the demo without code changes.
- The demo successfully acquires an access token from VC Hub.
- The demo provides a sample UI for running and observing the SignalR scenario.
- The demo successfully establishes a SignalR connection to VC Hub through `ws/v/realtimedata`.
- The demo successfully receives data from the `TagValues` and `Alarms` SignalR scenarios.
- The new demo references the old project only for authentication and SignalR connection implementation guidance.
- The UI and logs clearly show success and failure states.

## 17. Risks and Constraints

- Demo success depends on valid VC Hub connectivity and credentials.
- Demo success depends on WebSocket access to the required SignalR routes.
- The `TagValues` and `Alarms` streams may not emit data if the test environment lacks live events or configured tags.
- Self-signed or invalid certificates may require temporary test-only handling.
- API behavior may differ by VC Hub version or environment configuration.
- Reusing old code too broadly may introduce unnecessary coupling and legacy behavior into the new demo.

## 18. Resolved Decisions

- VC Hub target baseline version: `5.1`
- Primary SignalR hub route: `ws/v/realtimedata`
- Primary SignalR streams: `TagValues` and `Alarms`
- Preferred sample UI technology: `Blazor`

## 19. Future Enhancements

- Add support for additional SignalR streams after the first scenario is accepted.
- Add structured output samples for the selected SignalR messages.
- Add reconnect and retry behavior for SignalR connections.
- Add richer UI filtering, message formatting, and stream selection.
- Add integration tests against a controlled VC Hub test environment.
- Add packaging or containerization guidance.

## 20. Approval

- Prepared by: [Name]
- Reviewed by: [Name]
- Approved by: [Name]
- Approval date: [Date]

## 21. Implementation Reference

- Detailed implementation spec: `implementation-spec.md`
