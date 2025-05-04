# Use Cases
Based on the requirements of the defining system, we defined the following use cases for our system

## Index

### 1. User-Facing Use Cases

- [UC-01 – Register Account](#uc-01--register-account)
- [UC-02 – Log In](#uc-02--log-in)
- [UC-03 – Update Profile](#uc-03--update-profile)
- [UC-04 – Browse Products](#uc-04--browse-products)
- [UC-05 – Manage Cart](#uc-05--manage-cart)
- [UC-06 – Place Order](#uc-06--place-order)
- [UC-07 – Track Orders & History](#uc-07--track-orders--history)
- [UC-08 – Password Recovery](#uc-08--password-recovery)
- [UC-09 – Delete Account / GDPR Erasure](#uc-09--delete-account--gdpr-erasure)

### 2. Administrator Use Cases

- [UC-10 – Manage Stores](#uc-09--manage-stores)
- [UC-11 – Manage Products](#uc-10--manage-products)
- [UC-12 – Manage Discounts](#uc-11--manage-discounts)
- [UC-13 – Manage Orders](#uc-12--manage-orders)
- [UC-14 – Manage User Roles](#uc-13--manage-user-roles)

### 3. System Use Cases

- [UC-15 – Process Payment](#uc-14--process-payment)
- [UC-16 – Send Notifications](#uc-15--send-notifications)
- [UC-17 – Ensure Performance](#uc-16--ensure-performance)
- [UC-18 – Auto-Scale Resources](#uc-17--auto-scale-resources)
- [UC-19 – GDPR Compliance](#uc-18--gdpr-compliance)
- [UC-20 – High Availability & DR](#uc-19--high-availability--dr)
- [UC-21 – CI/CD Quality Gate](#uc-20--cicd-quality-gate)


---

## UC-01 – Register Account

**Actor:** User
**Description:** Allows the creation of a new user account via the API.

**Preconditions:**
- The user is not authenticated.
- Registration functionality is available.

**Basic Flow:**
1. The user sends a request to register a new account with the required data (name, email, address, password).
2. The system validates the data provided.
3. If the data is valid, the system creates a new user record.
4. The system confirms successful registration to the customer.

**Alternate Flows:**
- **A1: Invalid Data** - The system rejects the request and provides error messages for the invalid fields.
- **A2: Existing Email** - The system informs you that the email is already registered.
- **A3: Username Already Existing** - The system informs you that the username is already registered.

**Post conditions:**
- The new user account is successfully created in the system.

---

## UC-02 – Log In

**Actor:** User
**Description:** Allows the user to obtain an authentication token (e.g. JWT) to be used in subsequent calls to the API.

**Preconditions:**
- The user has a previously registered account.
- The authentication functionality is operational.
- The client knows the login details.

**Basic Flow:**
1. The user sends an authentication request with their credentials.
2. The system checks the validity of the credentials.
3. If valid, the system generates and returns an authentication token.
4. The user uses this token to access all the API's protected endpoints.

**Alternate Flows:**
- **A1: Invalid Credentials** - The system rejects the request with an authentication error.
- **A2: Account Not Confirmed** - The system informs you that the account needs confirmation.

**Post conditions:**
- The customer has a valid authentication token.
- The client can now access any protected API resource using this token.

---

## UC-03 – Update Profile

**Actor:** User
**Description:** Allows the user to update their personal data via the API.

**Preconditions:**
- The user is authenticated.
- The user has a valid authentication token.

**Basic Flow:**
1. The user sends a request with the data to be updated (e.g. name, address, preferences).
2. The system validates the new data.
3. The system updates the user's information in the database.
4. The system confirms that the operation was successful.

**Alternate Flows:**
- **A1: Invalid Data** - The system rejects the request and indicates which fields are in error.
- **A2: Invalid or Expired Token** - The system responds with an authentication error.

**Post conditions:**
- User information is updated in the system.

---

## UC-04 – Browse Products

**Actor:** User  
**Description:** Allows the user to consult the products available in a store through the API.

**Preconditions:**
- The user is authenticated.
- The user has a valid authentication token.
- The store is active and contains visible products.

**Basic Flow:**
1. The user sends a request to obtain a list of products from a store.
2. The system validates the authentication and parameters of the request.
3. The system returns the available products, with support for filters such as category, price, availability, etc.

**Alternate Flows:**
- **A1: Invalid Token** - The system rejects the request with an authentication error.
- **A2: Inexistent or Inactive Store** - The system returns an error indicating that the store is not accessible.
- **A3: No Products Available** - The system returns an empty list or an informative message.

**Post conditions:**
- The user receives a list of products.

---

## UC-05 – Manage Cart

**Actor:** User
**Description:** Allows the user to add, update or remove products from the shopping cart.

**Preconditions:**
- The user is authenticated.
- The user has a valid authentication token.
- The store and products are available.

**Basic Flow:**
1. The user adds products to the cart.
2. The user can update quantities or remove products.
3. The system saves the status of the cart associated with the user.
4. The cart can be persisted between sessions.

**Alternate Flows:**
- **A1: Non-existent product** - The system returns an error when trying to add an invalid product.
- **A2: Invalid Quantity** - The system rejects the order and reports the error.

**Post conditions:**
- The current status of the cart is stored in the system.

---

## UC-06 – Place Order

**Actor:** User
**Description:** Allows the user to checkout and finalize the order.

**Preconditions:**
- The user is authenticated.
- The user has a valid authentication token.
- The payment functionality is operational.

**Basic Flow:**
1. The system calculates totals, applies discounts and validates the order.
2. The system creates the order and stores the data.
3. The system sends confirmation to the user.

**Alternate Flows:**
- **A1: Payment Failure** - The system reports failure and does not create the order.
- **A2: Incomplete Data** - The system rejects the order and requests corrections.
- **A3: Invalid/Expired Discount** – The system ignores the discount and informs the user.


**Post conditions:**
- The order is created or registered.

---

## UC-07 – Track Orders & History

**Actor:** User  
**Description:** Allows the user to consult the history of their orders.

**Preconditions:**
- The user is authenticated.
- The user has a valid authentication token.
- The user has placed previous orders.

**Basic Flow:**
1. The user requests the order list.
3. The user can consult the details of each order.

**Alternate Flows:**
- **A1: No Order Found** - The system returns an empty list.

**Postconditions:**
- The user has access to their order history.

---

## UC-08 – Password Recovery

**Actor:** User
**Description:** Allows the user to regain access to the account through a password reset process.

**Preconditions:**
- The user has a registered email address.
- The email sending system is functional.
- The user is authenticated.
- The user has a valid authentication token.

**Basic Flow:**
1. The user requests password recovery.
2. The system sends a secure link with a reset token.
3. The user accesses the link and sets a new password.
4. The system updates the password and confirms.

**Alternate Flows:**
- **A1: Unregistered Email** - The system may return a generic response to avoid account enumeration.

**Post conditions:**
- The new password replaces the previous one and can be used for login.

---

## UC-09 – Delete Account / GDPR Erasure

**Actor:** User  
**Description:** Allows the user to permanently delete their account and personal data.

**Preconditions:**
- The user is authenticated **or** presents a valid deletion token (email-based).

**Basic Flow:**
1. The user requests account deletion.
2. The system confirms the request (2-step verification).
3. The system anonymises or deletes personal data, respecting retention obligations.
4. A confirmation email is sent; associated tokens are revoked.

**Alternate Flows:**
- **A1: Pending Financial Transactions** – Deletion is blocked until all payments are settled.

**Post conditions:**
- Personal data is erased/anonymised; the account can no longer be used.

---

## UC-10 – Manage Stores

**Actor:** Administrator  
**Description:** Allows the administrator to create, edit and remove stores.

**Preconditions:**
- The Administrator is authenticated.
- The Administrator has a valid authentication token.
- The system is operational.

**Basic Flow:**
1. The administrator sends requests to manage stores (creation, editing, removal).
2. The system validates the data and applies the changes.
3. The system confirms the operation to the administrator.

**Alternate Flows:**
- **A1: Invalid Data** - The system rejects the request with error messages.

**Post conditions:**
- The store database is updated as requested.

---

## UC-11 – Manage Products

**Actor:** Administrator  
**Description:** Allows the administrator to manage the products available in the stores.

**Preconditions:**
- The Administrator is authenticated.
- The Administrator has a valid authentication token.
- The store exists and is active.

**Basic Flow:**
1. The administrator adds, edits or removes products.
2. The system validates the data and updates the records.
3. The products become available or updated for users.

**Alternate Flows:**
- **A1: Product Not Associated with Valid Store** - The system rejects the operation.

**Post conditions:**
- The product database reflects the changes made.

---

## UC-12 – Manage Discounts

**Actor:** Administrator  
**Description:** Allows the administrator to define and apply discounts in stores.

**Preconditions:**
- The Administrator is authenticated.
- The Administrator has a valid authentication token.

**Basic Flow:**
1. The administrator defines a new discount (percentage, validity, target products).
2. The system applies and saves the data.
3. The discount becomes available on the affected products.

**Alternate Flows:**
- **A1: Invalid dates or non-existent product** - The system rejects and reports the error.

**Post conditions:**
- The products reflect the active discounts.

---

## UC-13 – Manage Orders

**Actor:** Administrator  
**Description:** Allows the administrator to manage the status of orders.

**Preconditions:**
- The Administrator is authenticated.
- The Administrator has a valid authentication token.
- There are orders in the system.

**Basic Flow:**
1. The administrator consults orders and updates the status (e.g. sent, canceled).
2. The system validates the operation and updates the data.
3. The system can notify the customer of changes.

**Alternate Flows:**
- **A1: Invalid Status** - The system rejects with an error.

**Post conditions:**
- The order status is updated in the system.

---

## UC-14 – Manage User Roles

**Actor:** Administrator  
**Description:** Allows the administrator to assign or revoke permissions to users.

**Preconditions:**
- The Administrator is authenticated.
- The Administrator has a valid authentication token.

**Basic Flow:**
1. The administrator sets permissions for a user (e.g. make another user administrator).
2. The system applies the change.
3. The affected user gains or loses access to specific functionalities.

**Alternate Flows:**
- **A1: Invalid Permission** - The system rejects the request.

**Post conditions:**
- The user's permissions are updated.

---

## UC-15 – Process Payment

**Actor:** System  
**Description:** Processes order payments with the data provided by the user.

**Preconditions:**
- The user has started the checkout process.
- The payment method is valid.

**Basic Flow:**
1. The system sends the payment data to the external gateway.
2. It receives the response and records the result.
3. Associates the payment with the order.

**Alternate Flows:**
- **A1: Payment Rejected** - The system notifies the user and does not create the order.

**Post conditions:**
- The payment is processed and registered.

---

## UC-16 – Send Notifications

**Actor:** System  
**Description:** Sends notifications by email, SMS or other channels when relevant events occur.

**Preconditions:**
- The notification subsystem is operational.

**Basic Flow:**
1. An event occurs (e.g. new order, shipped order).
2. The system identifies the recipients and channels.
3. Sends the notification.

**Post conditions:**
- The recipients receive the appropriate notification.

---

## UC-17 – Ensure Performance

**Actor:** System  
**Description:** Ensures that the system responds efficiently, especially during checkout.

**Preconditions:**
- System in normal operation.

**Basic Flow:**
1. The system is tested under load to keep the checkout time below 3 seconds (p95).
2. Metrics are monitored.

**Post conditions:**
- Platform performance is maintained within the defined parameters.

---

## UC-18 – Auto-Scale Resources

**Actor:** System  
**Description:** Automatically adjusts the infrastructure according to the system load.

**Preconditions:**
- Active monitoring of resource consumption.

**Basic Flow:**
1. The system detects high CPU/memory usage.
2. Automatically launches new instances/resources.
3. Reduces resources when load drops.

**Post conditions:**
- The system automatically adapts to demand.

---

## UC-19 – GDPR Compliance

**Actor:** System  
**Description:** Guarantees compliance with the GDPR when processing personal data.

**Preconditions:**
- Personal data is collected and stored.

**Basic Flow:**
1. The system records the user's explicit consent.
2. Data is encrypted and stored securely.
3. The user can request the deletion of their data.

**Post conditions:**
- Compliance with the GDPR is ensured.

---

## UC-20 – High Availability & DR

**Actor:** System  
**Description:** Maintains system availability and recovery in case of failure.

**Preconditions:**
- Active system with redundancy policy.

**Basic Flow:**
1. The system performs periodic backups.
2. In the event of a failure, it fails over to a secondary region.
3. Restores the service with minimal interruption.

**Post conditions:**
- Availability is maintained (99.9%) and data is preserved.

---

## UC-21 – CI/CD Quality Gate

**Actor:** System  
**Description:** Prevents the integration of faulty code into the CI/CD pipeline.

**Preconditions:**
- New code is submitted to the repository.

**Basic Flow:**
1. The pipeline runs tests and code analysis (lint, security).
2. If any test fails, the merge is blocked.
3. Only validated code is integrated into main.

**Post conditions:**
- Code in production maintains the defined quality standards.

---

## UC-22 – Logging & Monitoring

**Actor:** System  
**Description:** Provides observability for troubleshooting and compliance.

**Preconditions:**
- Logging and monitoring stack (e.g. ELK, Grafana) is operational.

**Basic Flow:**
1. The system collects structured logs, metrics and traces for all services.
2. Logs include user ID (pseudonymised), timestamp, operation and result.
3. Dashboards and alerts notify the operations team of anomalies.
4. Logs are retained for at least 90 days (or as required by policy).

**Post conditions:**
- Administrators can detect and resolve issues quickly; audit evidence is available.
---