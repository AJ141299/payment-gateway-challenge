# Payment Gateway

## How to Run

### Prerequisites
- .NET 10 SDK
- Docker (for the bank simulator)

### Start the bank simulator
```bash
docker-compose up
```

### Run the API
```bash
cd src/PaymentGateway.Api
dotnet run
```

### Run the tests
```bash
dotnet test
```

The API will be available at `https://localhost:7092` and `http://localhost:5067`

---

## Endpoints

### Process a Payment
**POST** `/api/v1/payments/process`

Request body:
```json
{
  "cardNumber": "2222405343248877",
  "expiryMonth": 6,
  "expiryYear": 2027,
  "currency": "USD",
  "amount": 1050,
  "cvv": "123"
}
```

Response:
```json
{
  "id": "a1b2c3d4-...",
  "status": "Authorized",
  "cardNumberLastFour": "8877",
  "expiryMonth": 6,
  "expiryYear": 2027,
  "currency": "USD",
  "amount": 1050
}
```

Possible status values: `Authorized`, `Declined`.

Supported currencies: `USD`, `GBP`, `PKR`.

---

### Retrieve a Payment
**GET** `/api/v1/payments/{id}`

Response:
```json
{
  "id": "a1b2c3d4-...",
  "status": "Authorized",
  "cardNumberLastFour": "8877",
  "expiryMonth": 6,
  "expiryYear": 2027,
  "currency": "USD",
  "amount": 1050
}
```

Returns `404 Not Found` if the payment does not exist.

---

## Design Choices

The following design choices were made with the intent to keep this service production-grade, with some improvements deferred in the interest of time (see [Future Improvements](#future-improvements)):

**Clean Architecture** - separation of concerns across API, Core, and Infrastructure layers. This makes the codebase maintainable and extensible without changes rippling across unrelated layers. Read more [here](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures#clean-architecture)

**FluentValidation** - validation logic lives outside models and controllers, making it independently testable. It also scales better as requirements grow (e.g. cross field validationss) and produces well-structured error responses out of the box.

**Testing strategy** - unit tests cover each component in isolation, while integration tests verify that components work together correctly according to business expectations. The validator is unit tested separately since it is pure logic with no dependencies.

**No docstrings** - this service is assumed to be maintained by an internal team where knowledge is shared. Code is written to be self-explanatory, with inline comments reserved for non-obvious behaviour. If the team has a standard requiring docstrings, that can be adopted easily.

**Extensibility over brevity** - the solution may appear verbose, but the layered structure means new features, payment methods, or providers can be added with minimal friction.

## Assumptions

- Amount will never be negative, so `ulong` was chosen as the type. An amount of `0` is considered valid.
- The bank is assumed to have no rate limits, so requests are forwarded directly. See point 6 in [Future Improvements](#future-improvements) for how this could be addressed.
- The assessment outline mentions a `Rejected` status, but the expected response schemas only define `Authorized` and `Declined`. This has been treated as a leftover from an older version of the requirements, hence has not been implemented.

## Future Improvements

1. **Mapper library** - replace manual mappings with a library like [Mapperly](https://github.com/riok/mapperly) to reduce the risk of mapping errors.
2. **Configurable currencies** - `AllowedCurrencies` is currently hardcoded in the validator. Ideally this would be driven by configuration (e.g. `appsettings.json` or a remote config store) so it can be updated without a deployment.
3. **Health endpoints** - expose readiness and liveness probes to support deployment to orchestrated environments like Kubernetes.
4. **Ingress protection** - if the service is exposed to the internet:
   - Rate limiting to prevent abuse
   - DDoS protection, ideally via a cloud provider's WAF or gateway
5. **Metrics** - track authorization vs decline rates, ideally at the merchant level. This can surface:
   - Merchants with abnormally high decline rates
   - Bank-side issues if declines spike across all merchants
   - Validation rejection spikes after a release, which could indicate a bug
6. **Queue-based bank requests** - if the bank enforces rate limits, a queue system would allow the gateway to absorb traffic spikes without breaching those limits, or expose equivalent rate limits to merchants.
7. **Alerting** - integrate with a tool like PagerDuty triggered by health endpoint failures to provide on-call visibility if the gateway goes down.
8. **Dockerize the API** - containerize the service for simplified deployment.
9. **Authentication and authorization** - secure the API so only registered merchants can process and retrieve payments.

## AI Usage Disclosure
- AI was used to generate the majority of tests in this solution. All tests were written under my direction (specifying the libraries, patterns, and scenarios to cover), and reviewed by me for correctness.
- Everything else has been designed and implemented by me.