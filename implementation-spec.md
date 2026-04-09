# VC Hub SignalR Demo Implementation Specification

## 1. Scope

This specification defines how to implement the new .NET 10 Blazor demo based on `requirements.md`.

Included:
- OAuth client-credentials authentication against VC Hub OpenID endpoints.
- SignalR client connection to the primary hub route `ws/v/realtimedata`.
- SignalR stream subscriptions for `TagValues` and `Alarms`.
- A Blazor sample UI for configuring connection settings, connecting, subscribing, and visualizing live messages.
- Logging and error reporting in both UI and backend logs.

Excluded:
- Persistent storage.
- Production-grade UX.
- Full HTTP API coverage.
- Full migration of `OldOpenApiDemo`.

## 2. Baseline Decisions

- VC Hub baseline version: `5.1`
- Runtime: `.NET 10`
- UI framework: `Blazor`
- Primary SignalR route: `ws/v/realtimedata`
- Primary stream scenarios: `TagValues`, `Alarms`

## 3. Solution Architecture

## 3.1 High-Level Flow

1. User opens Blazor UI and reviews or edits connection settings.
2. App validates configuration.
3. App requests access token via client credentials.
4. App creates SignalR connection with WebSockets transport and access token provider.
5. User clicks subscribe for `TagValues` and/or `Alarms`.
6. App consumes stream messages and updates UI in near real-time.
7. App logs all key events and surfaces errors in the UI.

## 3.2 Logical Layers

- Presentation layer: Blazor components/pages.
- Application layer: connection orchestration and stream subscription workflow.
- Integration layer: token acquisition and SignalR transport.
- Configuration layer: strongly typed options bound from `appsettings.json`.

## 4. Proposed Project Structure

Use this structure in the new demo project:

- `src/VcHubSignalRDemo/`
- `src/VcHubSignalRDemo/Program.cs`
- `src/VcHubSignalRDemo/appsettings.json`
- `src/VcHubSignalRDemo/Configuration/OpenApiOptions.cs`
- `src/VcHubSignalRDemo/Auth/ITokenService.cs`
- `src/VcHubSignalRDemo/Auth/TokenService.cs`
- `src/VcHubSignalRDemo/SignalR/IVcHubConnectionFactory.cs`
- `src/VcHubSignalRDemo/SignalR/VcHubConnectionFactory.cs`
- `src/VcHubSignalRDemo/SignalR/ISignalRStreamService.cs`
- `src/VcHubSignalRDemo/SignalR/SignalRStreamService.cs`
- `src/VcHubSignalRDemo/Models/TagValueMessage.cs`
- `src/VcHubSignalRDemo/Models/AlarmMessage.cs`
- `src/VcHubSignalRDemo/Models/UiEventLogItem.cs`
- `src/VcHubSignalRDemo/State/ConnectionStateStore.cs`
- `src/VcHubSignalRDemo/Components/Pages/Home.razor`
- `src/VcHubSignalRDemo/Components/ConnectionPanel.razor`
- `src/VcHubSignalRDemo/Components/SubscriptionPanel.razor`
- `src/VcHubSignalRDemo/Components/MessagePanel.razor`
- `src/VcHubSignalRDemo/Components/ErrorBanner.razor`

## 5. Configuration Specification

## 5.1 appsettings.json

```json
{
  "OpenApi": {
    "Url": "https://your-vc-hub-host:port",
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret"
  },
  "SignalR": {
    "PrimaryRoute": "ws/v/realtimedata",
    "EnableCertificateValidationBypassForDemo": true
  },
  "Demo": {
    "MaxMessagesPerStream": 200,
    "DefaultTagPaths": ["Default:m1"]
  }
}
```

## 5.2 Validation Rules

- `OpenApi:Url` must be a valid absolute URI.
- `OpenApi:ClientId` and `OpenApi:ClientSecret` must be non-empty.
- `SignalR:PrimaryRoute` must be non-empty.
- If certificate validation bypass is enabled, emit a warning at startup.

## 6. Authentication Specification

Reuse the old demo approach functionally, but implement in clean new classes.

Token service behavior:
- GET OpenID discovery document from `/.well-known/openid-configuration`.
- Resolve token endpoint from discovery metadata.
- Request token using `client_credentials`.
- Return access token string.

Caching:
- Cache token in memory.
- Refresh before expiry with a safe buffer (for example 60 seconds).

Error handling:
- Parse and log token endpoint failures.
- Surface token failure to UI as a connection-precondition error.

## 7. SignalR Connection Specification

Factory behavior:
- Build `HubConnection` from base URL + `SignalR:PrimaryRoute`.
- Force WebSockets transport.
- Set `AccessTokenProvider` to call token service.
- Optional demo TLS certificate bypass controlled by configuration.

Connection lifecycle:
- `ConnectAsync()` creates and starts connection.
- `DisconnectAsync()` stops and disposes connection.
- Prevent duplicate active connections.
- Publish connection state transitions to UI state store.

## 8. Stream Subscription Specification

## 8.1 TagValues

- Hub method: `TagValues`
- Input payload: list of tag paths from UI or default config.
- Output handling: append each received message to in-memory stream feed.
- UI mapping: display timestamp, path, value, quality.

## 8.2 Alarms

- Hub method: `Alarms`
- Input payload: none unless environment requires filter parameters.
- Output handling: append each alarm event to in-memory stream feed.
- UI mapping: display alarm path, priority, state, event time, message summary.

## 8.3 Subscription Controls

- User can start/stop each stream independently.
- If not connected, subscription command is blocked and error is shown.
- Stream cancellation token is managed per stream.

## 9. Blazor UI Specification

## 9.1 Home Page Layout

- Top: environment banner and app title.
- Left panel: connection settings and connect/disconnect controls.
- Middle panel: stream controls (`TagValues`, `Alarms`) and status chips.
- Right panel: message feed tabs (`TagValues`, `Alarms`) with newest-first items.
- Bottom area: diagnostic events log.

## 9.2 Required UI Elements

Connection panel:
- Inputs: URL, Client ID, Client Secret (secret masked).
- Button: Test Auth.
- Button: Connect / Disconnect.
- Label: Connection State (`Disconnected`, `Connecting`, `Connected`, `Error`).

Subscription panel:
- Checkbox or toggle for `TagValues`.
- Checkbox or toggle for `Alarms`.
- Input for tag path list used by `TagValues`.
- Action buttons: Start Selected, Stop Selected.

Message panel:
- Separate tabs or sections for `TagValues` and `Alarms`.
- Clear button per stream.
- Message count indicator.

Error visibility:
- Persistent banner for latest blocking error.
- Non-blocking warnings in event log.

## 9.3 UI Behavior Rules

- Disable subscribe controls while disconnected.
- Disable connect button while connection is in progress.
- On disconnect, stop all active streams and update UI state.
- Keep only latest N messages per stream to prevent memory growth.

## 10. State Management

Use a scoped state store (`ConnectionStateStore`) to track:
- Connection state.
- Active streams.
- Latest errors.
- Message collections for `TagValues` and `Alarms`.
- Diagnostic event timeline.

State updates must trigger UI refresh through standard Blazor state notifications.

## 11. Logging and Diagnostics

Backend logging categories:
- `Auth`
- `SignalR.Connection`
- `SignalR.Stream.TagValues`
- `SignalR.Stream.Alarms`
- `UI.Actions`

Minimum logged events:
- App startup.
- Config validation result.
- Token request start/success/failure.
- Connection start/success/failure/stop.
- Stream subscribe/unsubscribe and first-message receipt.
- Exception details with correlation id.

## 12. Error Handling Matrix

- Invalid configuration: block connect, show actionable message.
- Token acquisition failure: show auth error, keep disconnected state.
- Hub connection failure: show connection error and retry guidance.
- Stream subscription failure: keep connection alive, show stream-level error.
- Message parse failure: log warning, continue stream.

## 13. Security Controls (Demo-Level)

- No secrets hardcoded in source.
- Secrets sourced from configuration or environment variables.
- Certificate validation bypass must be feature-flagged and defaulted to safe value for non-demo environments.
- Do not echo client secret in UI or logs.

## 14. Delivery Plan

Milestone 1: Foundation
- Create .NET 10 Blazor project skeleton.
- Add options binding and config validation.
- Implement token service.

Milestone 2: Connection
- Implement SignalR connection factory and lifecycle service.
- Implement connect/disconnect UI and connection state.

Milestone 3: Streams
- Implement `TagValues` and `Alarms` subscriptions.
- Wire message feeds to UI panels.

Milestone 4: Hardening
- Add error banner and diagnostic log panel.
- Add message limits and stream cancellation handling.
- Final acceptance check against requirements.

## 15. Test Scenarios

Happy path:
1. Valid config.
2. Token acquired.
3. Connect to `ws/v/realtimedata`.
4. Subscribe to `TagValues` and `Alarms`.
5. UI shows incoming data.

Negative path:
1. Invalid client secret.
2. Verify auth error appears in UI and logs.
3. Verify no stream action allowed while disconnected.

Resilience path:
1. Disconnect while streams active.
2. Verify streams stop and state resets cleanly.

## 16. Definition of Done

- All functional requirements in `requirements.md` FR-1 through FR-8 are implemented or intentionally deferred with notes.
- UI demonstrates `TagValues` and `Alarms` end-to-end.
- Connection and stream errors are visible in UI and logs.
- Demo runs on .NET 10 in target environment.
