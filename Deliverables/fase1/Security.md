# Security

## Security Requirements
- Passwords must be hashed, salted and peppered before storage.
- Sensitive data like payment info must be encrypted at rest and in transit.
- Input validation and sanitization to prevent SQL Injection, XSS, etc.
- Strong session management (e.g., session expiration, cookie security).
- Logging and monitoring of critical actions.
- Least privilege principle for all roles.
- Secure error handling (no stack traces to users).

## Abuse Cases
- Brute-force login attacks.
- Denial of Service (DoS) attacks.
- Malicious users attempting privilege escalation (e.g., a client becoming an admin).
- Replay or manipulation of payment requests.
- Injection attacks via product/order fields.
- Abuse of discounts for free or nearly-free purchases.
- Insider threats: store collaborators misusing admin privileges.


## Secure Design
- Use HTTPS for all communications to protect data in transit.
- Implement rate limiting to prevent brute-force attacks and DOS attacks.
- Use JWT for secure user authentication and authorization.
- Use parameterized queries to prevent SQL injection.
- Implement input validation at every user input point.
- Hashed passwords.
- Use external providers for payment processing.
- Use a web application firewall (WAF) to filter and monitor HTTP traffic.
- Regularly update and patch all software dependencies to mitigate vulnerabilities.

## Secure Architecture
- Reverse proxy with rate limiting and WAF (Web Application Firewall).
- Enforce HTTPS everywhere (HSTS).
- Have logs in a separate, secure location.
- Use a secure database connection string.

## Secure Testing Planning
- **Static Application Security Testing (SAST)**: Check for code vulnerabilities (e.g., with SonarQube).
- **Dynamic Application Security Testing (DAST)**: Use OWASP ZAP or Burp Suite to find runtime vulnerabilities.
- **Penetration Testing**: Simulate attacks on login, payment, and discount systems.
- **Fuzz Testing**: Randomized input testing on product and payment APIs.
- **Role-based Access Tests**: Ensure users can only perform actions assigned to their role.
