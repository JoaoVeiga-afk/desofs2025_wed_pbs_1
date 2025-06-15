# API REST Test Checklist

This checklist covers the functional and security validation of the described e-commerce REST API.  
Each section addresses the critical flows, expected behavior, and validation points for both positive and negative scenarios.

---

## General API Validation

-  Ensure that all successful responses include `code`, `status`, and `data` fields.
-  Verify that all error responses include `code`, `status`, and `message` fields.
-  Confirm that all timestamps use the ISO-8601 UTC format (e.g., `2025-05-01T12:34:56Z`).
-  Validate that all monetary values are returned as integer cents (e.g., `3499` for 34.99 €).
-  Verify correct behavior for all user roles (`client`, `store_collaborator`, `store_admin`) in accessing relevant endpoints.
-  Confirm that unauthorized requests (missing or invalid token) are rejected with the correct HTTP status.

---

## Authentication Tests

### Register (`POST /auth/register`)

-  Register a new user with valid data. Expect a 200 OK response with user token.
-  Attempt to register with an existing email. Expect a 400 error with descriptive message.
-  Attempt to register without required fields. Expect validation error response.

### Login (`POST /auth/login`)

-  Login using valid username and password. Expect a valid token in the response.
-  Attempt login with a non-existing user. Expect an error response.
-  Attempt login with an incorrect password. Expect an error response.
-  Attempt login without optional `auth_code` and observe behavior.

---

## User Management Tests

### List Users (`GET /user`)

-  List all users as `store_admin`. Expect complete list.
-  Attempt to list users as `client` or `store_collaborator`. Expect forbidden or limited access.

### Get User Details (`GET /user/{username}`)

-  Retrieve a user's details as `store_admin`.
-  Access `/users/me` as `client` or `store_collaborator`. Ensure `status` and `role` fields are not present.

### Create User (`POST /user`)

-  Create a user as `store_admin`. Expect 201 Created.

### Update User (`PATCH /user/{username}`)

-  Update a user's information. Expect 200 OK with updated data.

### Delete User (`DELETE /user/{username}`)

-  Delete a user as `store_admin`. Expect confirmation message.

---

## Store Management Tests

### Create a new store (`POST /stores`) as `store_admin`.
- Create a new store with valid fields as store_admin. Expect 201 Created with store ID and details.
- Attempt to create a store with missing required fields (e.g., name or address). Expect 400 Bad Request.
- Attempt to create a store with invalid field types (e.g., name as a number). Expect validation error.

### Update store details (`PATCH /stores/{storeId}`) as `store_admin`.
- Update store name or description as store_admin. Expect 200 OK with updated store data.
- Attempt to update a non-existent store ID. Expect 404 Not Found.
- Attempt to update a store with invalid values (e.g., empty name). Expect validation error.

### Attempt these actions as lower roles and verify access is denied.

---

## Product Management Tests

### List products (`GET /product`).
- Retrieve all products with no filters. Expect 200 OK and full list.
- Apply filters (storeId, category, priceRange). Expect 200 OK and filtered results.
- Apply filters with invalid values (e.g., non-existent category). Expect empty list or validation error.
- List products as anonymous user (if allowed). Expect 200 OK.

### Retrieve product details (`GET /product/{productId}`).
- Retrieve existing product details by ID. Expect 200 OK with product info.
- Request a non-existent product ID. Expect 404 Not Found.
- Attempt with malformed ID (e.g., string instead of number). Expect 400 Bad Request.

### Create a product (`POST /product`).
- Create a product as store_admin or same level of authentication. Expect 201 Created with product ID and details.
- Attempt to create with missing required fields (e.g., name, price). Expect 400.
- Attempt to create with negative or invalid price. Expect validation error.
- Attempt to create as client. Expect 403 Forbidden.

### Update an existing product.
- Update product details (name, stock, price) as store_admin. Expect 200 OK with updated data.
- Attempt to update a non-existent product. Expect 404 Not Found.
- Attempt to update with invalid data (e.g., negative stock). Expect 422 Unprocessable Entity.

### Attempt administrative actions with lower authentication level and verify access is denied.

---

## Discount Management Tests

-  Create a discount (`POST /discounts`) as `Admin`.
-  Update a discount (`PATCH /discounts/{discountId}`).
-  Delete a discount (`DELETE /discounts/{discountId}`).
-  Attempt to perform these actions without `Admin` role and confirm rejection.

---

## Cart and Checkout Tests

-  Add a product to the cart (`POST /cart`).
-  Update quantity of an existing item in the cart.
-  List items in the cart (`GET /cart`).
-  Remove a product from the cart (`DELETE /cart/{productId}`).
-  Attempt these actions without authentication and expect access denied.

---

## Order Management Tests

-  Create an order from the current cart (`POST /order`).
-  List user orders (`GET /order`).
-  Get order details by ID (`GET /order/{orderId}`).
-  Update order status (`PATCH /order/{orderId}`) as `Admin`.
-  Attempt to update or delete orders as `client`. Expect access denied.
-  Delete an order (`DELETE /order/{orderId}`) as `Admin`.

---

## Payment Management Tests

-  List payment methods (`GET /payment-methods`).
-  Add a new payment method (`POST /payment-methods`).
-  Delete a payment method (`DELETE /payment-methods/{methodId}`).
-  Process a payment (`POST /payments`) for an order.
-  Attempt to process payment on an already paid order. Expect error.
-  Attempt to process payment with an invalid method. Expect error.

---

## Notification Tests\

-  Send a notification (`POST /notifications`) as `Admin`.
-  Attempt to send notification as `client` or unauthorized user.
-  Attempt to send notification using an invalid channel. Expect error.

---

## Security and Access Control Tests

-  Verify all protected endpoints reject requests without a valid token.
-  Verify access is denied when using expired or invalid tokens.
-  Verify that `client` users cannot access admin-only endpoints.
-  Confirm data isolation between different users (e.g., cannot access others' orders).
-  Test injection attacks (SQL Injection, XSS) in all input fields and confirm proper validation or sanitization.
