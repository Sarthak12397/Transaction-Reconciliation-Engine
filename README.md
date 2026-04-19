# Data Reconciliation / Consistency Monitor

A backend system that detects when two financial systems 
disagree — and resolves the gap automatically through 
retries, failure classification, and dead-letter handling.

> Built with .NET 10 · PostgreSQL · Hangfire · Docker · Serilog

## Problem

Financial systems maintain records across multiple sources.
When your internal database disagrees with an external
system (eSewa, bank, payment gateway) — that gap is real money:

- Your system says NPR 30 completed. eSewa says NPR 20.
- Your system says completed. The bank says never received.
- A timeout leaves the comparison in an unknown state.
- Nobody notices until the audit fails.

Without active reconciliation, these gaps silently accumulate.

## Solution

- **State machine** — every reconciliation attempt moves through
  strict, controlled states. No ambiguity. No illegal jumps.
- **Idempotent external client** — same TransactionId always
  returns the same external state. Requeue never triggers
  duplicate external actions.
- **Failure classification** — transient failures retried safely.
  Permanent failures dead-lettered immediately.
- **Audit trail** — every state change written to DB with
  CorrelationId for full traceability.
