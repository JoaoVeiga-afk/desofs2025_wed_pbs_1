# REST API Entry-Points

**This document describes the entrypoints of the application's backend REST API, 
aimed at supporting e-commerce operations and managing users, stores, products and orders, in accordance with the defined use cases.**


## General Description for API Operation

### Base Information
* **Base URL:** `https://shoptex/api`
* **Date:** All timestamps follow the ISO-8601 UTC format (`2025-05-01T12:34:56Z`).
* **Money:** All monetary values are represented as integers in cents (e.g. `3499` → **34.99 €**).
* **Roles:**
  * `client` - Default user with limited permissions.
  * `store_collaborator` - Store collaborator with management permissions.
  * `store_admin` - Store administrator with full permissions.
  
### Role-Based Field Access
Fields marked with the following labels indicate the permissions required for access:

* **`admin-only`** - Only available to users with the **store_admin** role.
* **`collab+admin`** - Available to **store_collaborator** and **store_admin**.
* **`public`** - Available for **store_collaborator**, **store_admin**, and **client**.

---

### API Format and Standards

The choice of formats used by the API follows the standards established in international documents, guaranteeing interoperability and consistency in the data:

- [RFC 2616 - Hypertext Transfer Protocol (HTTP/1.1)](https://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html) - HTTP response status definitions.
- [RFC 9110 - HTTP Semantics](https://www.rfc-editor.org/rfc/rfc9110.html) - Latest standards on semantics and handling of HTTP responses.

---

### General API Success Response

Successful API responses follow a consistent and clear format, with a status code, operation status, and structured data in the body of the response.

**Example of a Successful Response:**

```json
{
  "code": 200,
  "status": "success",
  "data": {
    "username": "john42",
    "token": "apiblahblahblah",
    "expiration_date": 3600
  }
}
```

**Description of fields:**
- code (integer): HTTP status code (example: 200 for success, 201 for resource creation).

- status (string): General status of the operation. Can be “success” or “error”.

- data (object): Data resulting from the operation, with detailed information (example: user details or generated token).

  - username (string): Authenticated username.

  - token (string): JWT token for authentication in subsequent requests.

  - expiration_date (integer): Token expiry time, in seconds.

### General API Insuccess Response

The API implements a robust error handling system, returning appropriate HTTP status codes along with an explanatory message in the body of the response.

**Example of Insuccess Response:**

```json
{
  "code": 400,
  "status": "error",
  "message": "Email already registered. Please provide a unique email address."
}
```

**Description of fields:**
- code (integer): HTTP status code indicating the type of error (e.g. 400 for validation error, 401 for authentication failure).
- status (string): Indicates whether the operation was successful or not, “error” for errors.
- message (string): Explanatory message describing the error in a way that is clear and accessible to the user or developer.
--- 

## Endpoints Description Index
Index of List of Endpoints: 
1. [Authentication](#1-authentication)
2. [Users](#2-users)
3. [Stores](#3-stores)
4. [Products](#4-products)
5. [Discounts](#5-discounts)
6. [Checkout](#6-checkout)
7. [Orders](#7-orders)
8. [Payments](#8-payments)
9. [Notifications](#9-notifications)
---
### 1. Authentication

| Method | Endpoint         | Description                                   | Access Level |
|--------|------------------|-----------------------------------------------|--------------|
| `POST` | `/auth/login`    | Validate credentials and return the JWT Token | Public       |
| `POST` | `/auth/register` | Creates the user account                      | Public       |

#### 1.1 `POST /auth/register`

### Payload Parameters

The parameters required to create a new user:

- **`id`** *(integer)* – Unique identifier for the user.
- **`name`** *(string)* – User’s full name.
- **`email`** *(string)* – User’s unique email address (must be valid).
- **`phone`** *(string)* – User’s unique mobile phone number (must be valid).
- **`role`** *(string)* – User’s role on the platform. Examples: `"client"`, `"admin"`.
- **`created_at`** *(string – ISO 8601)* – Date and time the user was created, in the format `"YYYY-MM-DDTHH:mm:ssZ"`.

**Example Payload:**
```json
{
  "id": 42,
  "name": "John",
  "email": "john@x.com",
  "role": "client",
  "created_at": "2025-05-01T12:34:56Z"
}
```
**Response Payload:**
```json
{
  "code": 200,
  "status": "success",
  "data": [
    "username" => "john42"
    "token" => "apiblahblahblahblah"
    "expiration_date" => "3600"
  ]
}
```

#### 1.2 `POST /auth/login`

### Payload Parameters

The request body must include the following parameters:

- **`auth_code`** *(string)* – Token sent with the request, if applicable (when using additional authentication).
- **`user`** *(object)*
  - **`username`** *(string)* – Username used for login.
  - **`name`** *(string)* – User’s full name.
  - **`phone`** *(string)* – User’s full mobile phone number.
  - **`password`** *(string)* – User’s password.

*Example Payload:**
```json
{
  "token": "jwt…",
  "user": {
    "username": "john42",
    "name": "John",
    "password": "john42client"
  }
}
```
**Response Payload:**

```json
{
  "code": 200,
  "status": "success",
  "data": [
    "username" => "john42",
    "token" => "apiblahblahblahblah",
    "expiration_date" => "3600"
  ]
}
```
---


### 2. Users

| Método | Endpoint | Descrição | Acesso |
|--------|---------|-----------|--------|
| `GET`  | `/user` | Lista todos os utilizadores | **store_admin** |
| `GET`  | `/user/{username}` | Detalhe de um utilizador | **store_admin** |
| `POST` | `/user` | Cria utilizador | **store_admin** |
| `PATCH` | `/user/{username}` | Atualiza utilizador | **store_admin** |
| `DELETE` | `/user/{username}` | Apaga utilizador | **store_admin** |


#### 2.1 `GET /user`
Response Payload:

```json
{
  "code": 200,
  "status": "success",
  "data": [
    {
      "id": 7,
      "name": "Alice Silva",
      "email": "alice@shop.com",
      "phone": "931234567",
      "address": "Rua X, Porto",
      "status": "active",
      "role": { "id": 2, "name": "store_collaborator" },
      "created_at": "2025‑05‑01T12:34:56Z",
      "updated_at": "2025‑05‑01T12:40:00Z"
    },
    {
      "id": 8,
      "name": "Alice Silva",
      "email": "alice@shop.com",
      "phone": "931234567",
      "address": "Rua X, Porto",
      "status": "active",
      "role": { "id": 2, "name": "store_collaborator" },
      "created_at": "2025‑05‑01T12:34:56Z",
      "updated_at": "2025‑05‑01T12:40:00Z"
    },
    {
      "id": 9,
      "name": "Alice Silva",
      "email": "alice@shop.com",
      "phone": "931234567",
      "address": "Rua X, Porto",
      "status": "active",
      "role": { "id": 2, "name": "store_collaborator" },
      "created_at": "2025‑05‑01T12:34:56Z",
      "updated_at": "2025‑05‑01T12:40:00Z"
    }
  ]
}
```

#### 2.2 `GET /user/{username}`
Response Payload:
```json
{
  "code": 200,
  "status": "success",
  "data": [
    {
      "id": 7,
      "name": "Alice Silva",
      "email": "alice@shop.com",
      "phone": "931234567",
      "address": "Rua X, Porto",
      "status": "active",
      "role": { "id": 2, "name": "store_collaborator" },
      "created_at": "2025‑05‑01T12:34:56Z",
      "updated_at": "2025‑05‑01T12:40:00Z"
    }
  ]
}
```
> **Note:** `client` and `store_collaborator` only access `/users/me`, receiving the same object **without** the `status` and `role` fields.


#### 2.3 `POST /user`

**Request Payload:**

The request body must contain the following parameters to create a new user:

| Field        | Type         | Description                                     |
|--------------|--------------|-------------------------------------------------|
| `id`         | integer      | Unique identifier for the user                 |
| `name`       | string       | Full name of the user                          |
| `email`      | string       | Unique email address                           |
| `phone`      | string       | Mobile phone number (9 digits)                 |
| `address`    | string       | Optional physical address                      |
| `password`   | string       | User password (hashed or raw, depending on API)|
| `status`     | string       | Account status (e.g., `active`)                |
| `role`       | object       | Role object containing `id` and `name`         |
| `created_at` | string       | ISO 8601 timestamp of user creation            |
| `updated_at` | string       | ISO 8601 timestamp of last update              |

**Example Payload:**

```json
{
  "id": 7,
  "name": "Alice Silva",
  "email": "alice@shop.com",
  "phone": "931234567",
  "address": "Rua X, Porto",
  "password": "abc123",
  "status": "active",
  "role": { "id": 2, "name": "store_collaborator" },
  "created_at": "2025-05-01T12:34:56Z",
  "updated_at": "2025-05-01T12:40:00Z"
}
```
**Response Payload:**

```json
{
  "code": 201,
  "status": "success",
  "data": {
    "username": "alice",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI...",
    "expiration_date": 3600
  }
}
```


#### 2.4 `PATCH /user/{username}`

Updates information of an existing user. Only fields included in the payload will be updated.

**Request Payload:**

| Field        | Type     | Description                              |
|--------------|----------|------------------------------------------|
| `name`       | string   | (Optional) New full name                 |
| `email`      | string   | (Optional) New email address             |
| `phone`      | string   | (Optional) New phone number              |
| `address`    | string   | (Optional) New address                   |
| `password`   | string   | (Optional) New password (hashed/raw)     |
| `status`     | string   | (Optional) New status (`active`, `blocked`, etc.) |
| `role`       | object   | (Optional) New role object `{ id, name }`|

**Example Payload:**

```json
{
  "name": "Alice M. Silva",
  "email": "alice.silva@shop.com",
  "status": "active"
}
```
**Response Payload:**
```json
{
  "code": 200,
  "status": "success",
  "data": {
    "id": 7,
    "name": "Alice M. Silva",
    "email": "alice.silva@shop.com",
    "phone": "931234567",
    "address": "Rua X, Porto",
    "status": "active",
    "role": { "id": 2, "name": "store_collaborator" },
    "updated_at": "2025-05-01T15:30:00Z"
  }
}
```


#### 2.5 `DELETE /user/{username}`
Permanently deletes a user from the system.
No request body required.

**Response Payload:**

```json
{
  "code": 200,
  "status": "success",
  "message": "User 'alice.silva' was successfully deleted."
}
```
---

### 3. Stores

| Método | Endpoint | Descrição | Acesso |
|--------|----------|-----------|--------|
| `POST` | `/stores` | Cria nova loja | **store_admin** |
| `PATCH` | `/stores/{storeId}` | Atualiza loja | **store_admin** |
| `DELETE` | `/stores/{storeId}` | Remove loja | **store_admin** |

#### 3.1 `POST /stores`

Creates a new store managed by a store administrator.

**Request Payload:**

| Field     | Type     | Description                             |
|-----------|----------|-----------------------------------------|
| `name`    | string   | Name of the store                       |
| `address` | string   | Physical address of the store           |
| `phone`   | string   | Store contact number (9 digits)         |
| `status`  | string   | Store status (`active`, `inactive`)     |
| `AdminId` | integer  | ID of the user assigned as Store Admin  |

**Example Payload:**

```json
{
  "name": "Loja Porto",
  "address": "Rua Central, 123",
  "phone": "912345678",
  "status": "active",
  "AdminId": 5
}
```

**Response Payload:**
```json
{
  "code": 201,
  "status": "success",
  "data": {
    "id": 15,
    "name": "Loja Porto",
    "created_at": "2025-05-01T14:00:00Z"
  }
}
```


#### 3.2 `PATCH /stores/{storeId}`

Updates an existing store. Only fields present in the payload will be updated.


**Request Payload:**

| Field     | Type     | Description                                 |
|-----------|----------|---------------------------------------------|
| `name`    | string   | (Optional) New name of the store            |
| `address` | string   | (Optional) New physical address             |
| `phone`   | string   | (Optional) New store contact number         |
| `status`  | string   | (Optional) New status (`active`, `inactive`)|
| `AdminId` | integer  | (Optional) New Store Admin user ID          |

**Example Payload:**

```json
{
  "name": "Loja Porto Renovada",
  "status": "inactive"
}
```

**Response Payload:**
```json
{
  "code": 200,
  "status": "success",
  "data": {
    "id": 15,
    "name": "Loja Porto Renovada",
    "address": "Rua Central, 123",
    "phone": "912345678",
    "status": "inactive",
    "updated_at": "2025-05-01T15:45:00Z"
  }
}
```


#### 3.3 `DELETE /stores/{storeId}`

Permanently deletes a store from the system.
No request body required.

**Response Payload:**

```json
{
  "code": 200,
  "status": "success",
  "message": "Store with ID '15' was successfully deleted."
}
```
---

### 4. Products

| Método | Endpoint | Descrição | Acesso |
|--------|----------|-----------|--------|
| `GET`  | `/product` | Lista produtos (query params: `storeId`, `category`, `priceMin`, `priceMax`, `search`) | Autenticado |
| `GET`  | `/product/{productId}` | Detalhes do produto | Autenticado |
| `POST` | `/product` | Cria produto | **Admin** |
| `PATCH` | `/product/{productId}` | Atualiza produto | **Admin** |
| `DELETE` | `/product/{productId}` | Apaga produto | **Admin** |

#### 4.1 `GET /product`
Lists all products.

**Response Payload:**
```json
{
  "code": 200,
  "status": "success",
  "data": [
    {
      "id": 5,
      "name": "T-Shirt",
      "description": "100% cotton T-shirt",
      "price": 1299,
      "category": "clothing",
      "status": "available",
      "storeId": 2
    },
    {
      "id": 6,
      "name": "T-Shirt",
      "description": "100% cotton T-shirt",
      "price": 1299,
      "category": "clothing",
      "status": "available",
      "storeId": 2
    },
    {
      "id": 8,
      "name": "T-Shirt",
      "description": "100% cotton T-shirt",
      "price": 1299,
      "category": "clothing",
      "status": "available",
      "storeId": 2
    }
  ]
}

```

#### 4.2 `GET /product/{productId}`
Retrieves the details of a specific product by its ID.

**Response Payload:**
```json
{
  "code": 200,
  "status": "success",
  "data": 
    {
      "id": 5,
      "name": "T-Shirt",
      "description": "100% cotton T-shirt",
      "price": 1299,
      "category": "clothing",
      "status": "available",
      "storeId": 2
    }
}
```

#### 4.3 `POST /product`

Creates a new product in the specified store.

**Request Payload:**

| Field         | Type     | Description                                 |
|---------------|----------|---------------------------------------------|
| `name`        | string   | Name of the product                         |
| `description` | string   | Description of the product                  |
| `price`       | integer  | Price in cents (e.g. 1299 = 12.99€)         |
| `category`    | string   | Product category                            |
| `status`      | string   | Product status (`available`, `out_of_stock`)|
| `storeId`     | integer  | Store ID where the product belongs          |

**Example Payload:**

```json
{
  "name": "T-Shirt",
  "description": "100% cotton T-shirt",
  "price": 1299,
  "category": "clothing",
  "status": "available",
  "storeId": 2
}
```

**Response Payload:**
```json
{
  "code": 201,
  "status": "success",
  "data": {
    "id": 5,
    "name": "T-Shirt",
    "created_at": "2025-05-01T14:00:00Z"
  }
}
```


#### 4.4 `PATCH /product/{productId}`

Updates a field from a product in the specified product.

**Request Payload:**

| Field         | Type     | Description                                 |
|---------------|----------|---------------------------------------------|
| `name`        | string   | Name of the product                         |
| `description` | string   | Description of the product                  |
| `price`       | integer  | Price in cents (e.g. 1299 = 12.99€)         |
| `category`    | string   | Product category                            |
| `status`      | string   | Product status (`available`, `out_of_stock`)|
| `storeId`     | integer  | Store ID where the product belongs          |

**Example Payload:**

```json
{
  "name": "T-Shirt Premium",
  "price": 1499,
  "status": "available"
}
```

**Response Payload:**
```json
{
  "code": 200,
  "status": "success",
  "data": {
    "id": 5,
    "name": "T-Shirt Premium",
    "description": "100% cotton T-shirt",
    "price": 1499,
    "category": "clothing",
    "status": "available",
    "storeId": 2,
    "updated_at": "2025-05-01T15:45:00Z"
  }
}
```

#### 4.5 `DELETE /product/{productId}`

Deletes a product from the system.

**Response Payload:**
```json
{
  "code": 200,
  "status": "success",
  "message": "Product with ID '5' was successfully deleted."
}
```
---

### 5. Discounts

| Método | Endpoint | Descrição | Acesso |
|--------|----------|-----------|--------|
| `POST` | `/discounts` | Cria desconto | **Admin** |
| `PUT` / `PATCH` | `/discounts/{discountId}` | Atualiza desconto | **Admin** |
| `DELETE` | `/discounts/{discountId}` | Remove desconto | **Admin** |



#### 5.1 `POST /discounts`

Creates a new discount associated with a specific product.

**Request Payload:**

| Field        | Type     | Description                                   |
|--------------|----------|-----------------------------------------------|
| `productId`  | integer  | ID of the product where the discount applies   |
| `amount`     | integer  | Discount amount in cents (e.g., 300 = 3.00€)  |
| `status`     | string   | Discount status (`active`, `inactive`)        |
| `start_date` | string   | Start date of the discount (`YYYY-MM-DD`)     |
| `end_date`   | string   | End date of the discount (`YYYY-MM-DD`)       |

**Example Payload:**

```json
{
  "productId": 3,
  "amount": 300,
  "status": "active",
  "start_date": "2025-05-10",
  "end_date": "2025-05-15"
}
```
**Response Payload:**

```json
{
  "code": 201,
  "status": "success",
  "data": {
    "id": 12,
    "productId": 3,
    "amount": 300,
    "status": "active",
    "start_date": "2025-05-10",
    "end_date": "2025-05-15",
    "created_at": "2025-05-01T16:00:00Z"
  }
}
```

#### 5.2 `PATCH /discounts/{discountId}`

Updates an existing discount. Only the provided fields will be updated.

**Request Payload:**

| Field        | Type     | Description                                   |
|--------------|----------|-----------------------------------------------|
| `productId`  | integer  | ID of the product where the discount applies   |
| `amount`     | integer  | Discount amount in cents (e.g., 300 = 3.00€)  |
| `status`     | string   | Discount status (`active`, `inactive`)        |
| `start_date` | string   | Start date of the discount (`YYYY-MM-DD`)     |
| `end_date`   | string   | End date of the discount (`YYYY-MM-DD`)       |

**Example Payload:**

```json
{
  "amount": 500,
  "status": "inactive"
}
```
**Response Payload:**

```json
{
  "code": 200,
  "status": "success",
  "data": {
    "id": 12,
    "productId": 3,
    "amount": 500,
    "status": "inactive",
    "start_date": "2025-05-10",
    "end_date": "2025-05-15",
    "updated_at": "2025-05-01T16:30:00Z"
  }
}
```

#### 5.3 `DELETE /discounts/{discountId}`

Deletes a discount from the system.
No request body required.

**Response Payload:**

```json
{
  "code": 200,
  "status": "success",
  "message": "Discount with ID '12' was successfully deleted."
}
```
---

## 6. Checkout

| Method  | Endpoint              | Description                                    | Access        |
|---------|-----------------------|------------------------------------------------|---------------|
| GET     | `/cart`               | Get the list of items currently in the cart    | Authenticated |
| POST    | `/cart`               | Add or update an item in the cart              | Authenticated |
| DELETE  | `/cart/{productId}`   | Remove an item from the cart                   | Authenticated |


#### 6.1 `GET /cart`

Retrieves the list of items currently in the authenticated user's cart.

**Response Payload:**

```json
{
  "code": 200,
  "status": "success",
  "data":[
    {
      "productId": 5,
      "name": "T-Shirt",
      "quantity": 2,
      "price": 1299,
      "subtotal": 2598
    },
    {
      "productId": 8,
      "name": "Sneakers",
      "quantity": 1,
      "price": 4999,
      "subtotal": 4999
    }
  ]
}
```

#### 6.2 `POST /cart`

Adds or updates an item in the authenticated user's cart. If the product already exists in the cart, its quantity will be updated.

**Example Payload:**

```json
{
  "productId": 5,
  "quantity": 3
}
```

**Response Payload:**

```json
{
  "code": 200,
  "status": "success",
  "message": "Product with ID '5' added/updated in cart."
}
```

#### 6.3 `DELETE /cart/{productId}`

Removes a product from the authenticated user's cart.
No request body required.

**Response Payload:**

```json
{
  "code": 200,
  "status": "success",
  "message": "Product with ID '5' removed from cart."
}
```
---

## 7. Orders

| Method  | Endpoint            | Description                                | Access        |
|---------|---------------------|--------------------------------------------|---------------|
| GET     | `/order`            | List the user's orders                     | Authenticated |
| GET     | `/order/{orderId}`  | Get order details by ID                    | Authenticated |
| POST    | `/order`            | Create an order from the current cart      | Authenticated |
| PATCH   | `/order/{orderId}`  | Update the status of an order              | Admin only    |
| DELETE  | `/order/{orderId}`  | Cancel or remove an order                  | Admin only    |

#### 7.1 `GET /order`

Get all the orders from the authenticated user.

**Response Payload:**

```json
{
  "code": 200,
  "status": "success",
  "data":[
    {
      "orderId": 1001,
      "status": "processing",
      "total": 7597,
      "created_at": "2025-05-01T16:00:00Z"
    },
    {
      "orderId": 1002,
      "status": "shipped",
      "total": 4999,
      "created_at": "2025-05-02T11:30:00Z"
    }
  ]
}
```

#### 7.2 `GET /order/{orderId}`

Retrieves the details of a specific order by ID.

**Response Payload:**

```json
{
  "code": 200,
  "status": "success",
  "data":{
      "orderId": 1001,
      "userId": 10,
      "status": "processing",
      "products": [
      { "productId": 5, "amount": 2, "price": 1299 },
      { "productId": 8, "amount": 1, "price": 4999 }
      ],
      "total": 7597,
      "created_at": "2025-05-01T16:00:00Z"
  }
}
```

#### 7.3 `POST /order`

Creates an order using the items currently in the user's cart.

**Request Payload:**

| Field      | Type    | Description                             |
|------------|---------|-----------------------------------------|
| `userId`   | integer | ID of the user placing the order        |
| `products` | array   | List of products and their quantities   |

Each item in `products` array:

| Subfield    | Type     | Description                      |
|-------------|----------|----------------------------------|
| `productId` | integer  | ID of the product                |
| `amount`    | integer  | Quantity of the product          |
| `price`     | integer  | Price per unit in cents          |

**Example Payload:**

```json
{
  "userId": 10,
  "products": [
    { "productId": 5, "amount": 2, "price": 1299 },
    { "productId": 8, "amount": 1, "price": 4999 }
  ]
}
```

**Response Payload:**

```json
{
  "code": 201,
  "status": "success",
  "data": {
    "orderId": 1001,
    "total": 7597,
    "created_at": "2025-05-01T16:00:00Z"
  }
}
```


#### 7.4 `PATCH /order/{orderId}`

Updates the status of an order.

**Example Payload:**

```json
{
  "status": "shipped"
}
```

**Response Payload:**

```json
{
  "code": 200,
  "status": "success",
  "data": {
    "orderId": 1001,
    "status": "shipped",
    "updated_at": "2025-05-01T17:00:00Z"
  }
}
```

#### 7.5 `DELETE /order/{orderId}`

Deletes or cancels an order.
No request body required.


**Response Payload:**

```json
{
  "code": 200,
  "status": "success",
  "message": "Order with ID '1001' was successfully canceled."
}
```
---

## 8. Payments

| Method  | Endpoint                      | Description                                  | Access        |
|---------|-------------------------------|----------------------------------------------|---------------|
| GET     | `/payments-methods`           | List user's payment methods                  | Authenticated |
| GET     | `/payment/{paymentId}`        | Get payment details by ID                    | Authenticated |
| POST    | `/payments`                   | Process payment (connect to payment gateway) | Authenticated |
| POST    | `/payment-methods`            | Add a new payment method                     | Authenticated |
| DELETE  | `/payment-methods/{methodId}` | Remove a payment method                      | Authenticated |


#### 8.1 `GET /payment-methods`
Lists all payment methods registered by the authenticated user.


**Response Payload:**

```json
{
  "code": 200,
  "status": "success",
  "data":[
    {
      "methodId": 2,
      "type": "credit_card",
      "info": "**** **** **** 4242",
      "status": "active",
      "added_at": "2025-05-01T14:00:00Z"
    },
    {
      "methodId": 3,
      "type": "paypal",
      "info": "user@paypal.com",
      "status": "active",
      "added_at": "2025-05-02T10:00:00Z"
    }
  ]
}
```

#### 8.2 `GET /payments/{paymentId}`

Retrieves the details of a specific payment.

**Response Payload:**

```json
{
  "code": 200,
  "status": "success",
  "data":{
    "paymentId": 2001,
    "orderId": 1001,
    "amount": 7597,
    "status": "paid",
    "method": "credit_card",
    "processed_at": "2025-05-01T16:10:00Z"
  }
}
```


#### 8.3 `POST /payments`

Processes a payment for an order.

**Example Payload:**

```json
{
  "orderId": 1001,
  "amount": 7597,
  "paymentMethodId": 2,
  "status": "paid"
}
```

**Response Payload:**

```json
{
  "code": 201,
  "status": "success",
  "data": {
    "paymentId": 2001,
    "orderId": 1001,
    "amount": 7597,
    "status": "paid",
    "processed_at": "2025-05-01T16:10:00Z"
  }
}
```

#### 8.4 `POST /payment-methods`

Creates a new payment method for the user.

**Example Payload:**

```json
{
  "type": "credit_card",
  "info": "**** **** **** 4242",
  "status": "active"
}
```

**Response Payload:**

```json
{
  "code": 201,
  "status": "success",
  "data": {
    "methodId": 4,
    "type": "credit_card",
    "info": "**** **** **** 4242",
    "status": "active",
    "added_at": "2025-05-01T16:30:00Z"
  }
}
```

#### 8.5 `DELETE /payment-methods/{methodId}`

Removes a payment method from the user's account.

No request body required.

**Example Payload:**

```json
{
  "code": 200,
  "status": "success",
  "message": "Payment method with ID '4' was successfully removed."
}
```

**Response Payload:**

```json
{
  "code": 404,
  "status": "error",
  "message": "Payment method with ID '999' not found."
}
```

---

## 9. Notifications

| Method  | Endpoint            | Description                                       | Access     |
|---------|---------------------|---------------------------------------------------|------------|
| POST    | `/notifications`    | Send a notification to a user (`userId`, `message`, `channel`) | Admin only |


#### 9.1 `POST /notifications`

Sends a notification to a user via the specified channel.

**Request Payload:**

| Field     | Type     | Description                                        |
|-----------|----------|----------------------------------------------------|
| `userId`  | integer  | ID of the user to receive the notification         |
| `message` | string   | Content of the notification                        |
| `channel` | string   | Delivery channel (`email`, `sms`, `push`)         |

**Example Payload:**

```json
{
  "userId": 10,
  "message": "Your order #1001 has been shipped!",
  "channel": "email"
}
```

**Response Payload:**

```json
{
  "code": 200,
  "status": "success",
  "message": "Notification sent to user ID '10' via 'email'."
}
```
---