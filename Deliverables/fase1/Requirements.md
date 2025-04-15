# Requirements

This section lists the main functional and non-functional requirements for the system/application.

## Functional Requirements

### FR01 - User Management
- The system must allow users to create accounts with details such as name, email, address, and a secure password.
- The system must validate user credentials during login.
- The system must allow users to update their personal information, including contact details and preferences.

### FR02 - Store Creation and Management
- The system must allow administrators to manage stores, including editing store details, adding products, and configuring preferences.

### FR03 - Order Management
- The system must allow users to browse and select products from the available stores.
- The system must allow users to complete the checkout process and place orders.
- The system must provide order tracking and history for users.
- The system must notify administrators of new orders.
- The system must allow administrators to manage orders, including updating status and canceling orders.

### FR04 - Product Management
- The system must allow administrators to add, edit, and remove products in the stores.
- The system must maintain product details such as name, description, price, and availability.

### FR05 - Discount Management
- The system must allow administrators to create and apply discounts, including percentage-based discounts and time-limited offers.
- The system must calculate the final price for products considering any applicable discounts.

### FR06 - Payment Management
- The system must provide multiple payment options for users, such as credit/debit cards, digital wallets, and direct bank transfers.
- The system must securely process and handle all payment transactions.

### FR07 - Notifications and Alerts
- The system must send notifications to administrators about new orders placed in the store.

## Non-Functional Requirements

### NFR01 – Performance
- The system should handle simultaneous orders and data retrieval operations efficiently for hundreds of users.
- The system must process and complete a checkout process in less than 3 seconds under normal load conditions.

### NFR02 - Scalability
- The system must scale to support additional users, stores, admins, and products as the business grows.
- The system should be able to handle a large number of products (up to a hundred items for a hundred stores) and orders without performance issues.

### NFR03 - Compliance
- The system must comply with GDPR and other relevant privacy regulations, ensuring that user data is stored and processed in a secure and legal manner.

### NFR04 - Maintainability
- The system must be built with modular architecture to allow easy updates and maintenance.
- The system should have logging and monitoring features to help administrators detect and fix issues quickly.

### NFR05 - Availability
- The system must have 99.9% uptime, ensuring that users and administrators can access the platform reliably.
- In case of system failure, the platform must have a backup and disaster recovery mechanism to restore service as quickly as possible.
- The system must implement regular backups of critical data (e.g., user information, store data, orders) to prevent data loss in the event of a system failure.

## Security Requirements
- Passwords must be hashed and salted before storage.
- Sensitive data like payment info must be encrypted at rest and in transit.
- Input validation and sanitization to prevent SQL Injection, XSS, etc.
- Strong session management (e.g., session expiration, cookie security).
- Logging and monitoring of critical actions.
- Least privilege principle for all roles.
- Secure error handling (no stack traces to users).

