# REST API Entry-Points

**This document describes the entrypoints of the application's backend REST API, 
aimed at supporting e-commerce operations and managing users, stores, products and orders, in accordance with the defined use cases.**


## General Description for API Operation

### Base Information
* **Base URL:** `https://shoptex/api`
* **Data/Hora:** All timestamps follow the ISO-8601 UTC format (`2025-05-01T12:34:56Z`).
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
    "expiry_date": 3600
  }
}
```

**Description of fields:**
- code (integer): HTTP status code (example: 200 for success, 201 for resource creation).

- status (string): General status of the operation. Can be “success” or “error”.

- data (object): Data resulting from the operation, with detailed information (example: user details or generated token).

  - username (string): Authenticated username.

  - token (string): JWT token for authentication in subsequent requests.

  - expiry_date (integer): Token expiry time, in seconds.

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
3. [Lojas](#3-stores)
4. [Produtos](#4-products)
5. [Descontos](#5-discounts)
6. [Carrinho](#6-checkout)
7. [Encomendas](#7-orders)
8. [Pagamentos](#8-payments)
9. [Notificações](#9-notifications)
---
### 1. Authentication

| Method | Endpoint         | Description                      | Access Level |
|--------|------------------|----------------------------------|--------------|
| `POST` | `/auth/login`    | Valida credenciais e devolve JWT | Public       |
| `POST` | `/auth/register` | Cria conta de usuário            | Public       |

#### 1.1 `POST /auth/register`

**Payload de Parâmetros:**

Os parâmetros necessários para criar um novo usuário:

- `id` (inteiro): Identificador único do usuário.
- `name` (string): Nome completo do usuário.
- `email` (string): Endereço de e-mail único do usuário. Deve ser válido.
- `role` (string): Papel ou função do usuário na plataforma. Exemplo: `"client"`, `"admin"`.
- `created_at` (string - ISO 8601): Data e hora em que o usuário foi criado, no formato `"YYYY-MM-DDTHH:mm:ssZ"`.

**Exemplo de Payload de Parâmetros:**
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
    "expiry_date" => "3600"
  ]
}
```

### 1.2 `POST /auth/login`

**Payload de Parâmetros:**

O corpo da requisição deve conter os seguintes parâmetros:

- `token` (string): Token JWT enviado na requisição, se aplicável (caso esteja usando autenticação adicional).
- `user` (objeto):
  - `username` (string): Nome de usuário utilizado para login.
  - `name` (string): Nome completo do usuário.
  - `password` (string): Senha do usuário.

**Exemplo de Payload de Parâmetros:**
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
    "username" => "john42"
    "token" => "apiblahblahblahblah"
    "expiry_date" => "3600"
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
| `PUT` / `PATCH` | `/user/{username}` | Atualiza utilizador | **store_admin** |
| `DELETE` | `/user/{username}` | Apaga utilizador | **store_admin** |


### 2.1 `GET /users`
Response Payload:

```json
[
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
```

### 2.2 `GET /users/{username}`
Objeto único, igual ao anterior.

> **Nota:** `client` e `store_collaborator` apenas acedem a `/users/me`, recebendo o mesmo objeto **sem** os campos `status` e `role`.

---

### 3. Stores

| Método | Endpoint | Descrição | Acesso |
|--------|----------|-----------|--------|
| `POST` | `/stores` | Cria nova loja | **store_admin** |
| `PUT` / `PATCH` | `/stores/{storeId}` | Atualiza loja | **store_admin** |
| `DELETE` | `/stores/{storeId}` | Remove loja | **store_admin** |

---

### 4. Products

| Método | Endpoint | Descrição | Acesso |
|--------|----------|-----------|--------|
| `GET`  | `/products` | Lista produtos (query params: `storeId`, `category`, `priceMin`, `priceMax`, `search`) | Autenticado |
| `GET`  | `/products/{productId}` | Detalhes do produto | Autenticado |
| `POST` | `/products` | Cria produto | **Admin** |
| `PUT` / `PATCH` | `/products/{productId}` | Atualiza produto | **Admin** |
| `DELETE` | `/products/{productId}` | Apaga produto | **Admin** |


---

### 5. Discounts

| Método | Endpoint | Descrição | Acesso |
|--------|----------|-----------|--------|
| `POST` | `/discounts` | Cria desconto | **Admin** |
| `PUT` / `PATCH` | `/discounts/{discountId}` | Atualiza desconto | **Admin** |
| `DELETE` | `/discounts/{discountId}` | Remove desconto | **Admin** |

---

## 6. Checkout

| Método | Endpoint | Descrição | Acesso |
|--------|----------|-----------|--------|
| `GET`  | `/cart` | Obtém itens do carrinho | Autenticado |
| `POST` | `/cart` | Adiciona/atualiza item (`productId`, `quantity`) | Autenticado |
| `DELETE` | `/cart/{productId}` | Remove item | Autenticado |

---

## 7. Orders

| Método | Endpoint | Descrição | Acesso |
|--------|----------|-----------|--------|
| `POST` | `/orders` | Cria encomenda a partir do carrinho | Autenticado |
| `GET`  | `/orders` | Lista encomendas do utilizador | Autenticado |
| `GET`  | `/orders/{orderId}` | Detalhe da encomenda | Autenticado |
| `PUT` / `PATCH` | `/orders/{orderId}` | Atualiza estado da encomenda | **Admin** |

---

## 8. Payments

| Método | Endpoint | Descrição | Acesso |
|--------|----------|-----------|--------|
| `POST` | `/payments` | Processa pagamento (liga a gateway) | Autenticado |
| `GET`  | `/payments/{paymentId}` | Detalhes do pagamento | Autenticado |
| `GET`  | `/payment-methods` | Lista métodos de pagamento do utilizador | Autenticado |
| `POST` | `/payment-methods` | Adiciona método de pagamento | Autenticado |
| `DELETE` | `/payment-methods/{methodId}` | Remove método | Autenticado |

---

## 9. Notifications

| Método | Endpoint | Descrição | Acesso |
|--------|----------|-----------|--------|
| `POST` | `/notifications` | Envia notificação (`userId`, `message`, `channel`) | **Admin** |

---