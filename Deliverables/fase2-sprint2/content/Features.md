# Features

## User Management


| Method | Endpoint       | Description                                   | Access Level |
|--------|----------------|-----------------------------------------------|--------------|
| `POST` | `/auth/signin` | Validate credentials and return the JWT Token | Public       |
| `POST` | `/auth/signup` | Creates the user account                      | Public       |
| `GET`  | `/auth/me`     | Get user info by token                        | Public        |

### 1.1  `POST /auth/signup`

#### Payload Parameters

The parameters required to create a new user:

- **`id`** *(integer)* – Unique identifier for the user.
- **`name`** *(string)* – User’s full name.
- **`email`** *(string)* – User’s unique email address (must be valid).
- **`phone`** *(string)* – User’s unique mobile phone number (must be valid).
- **`role`** *(string)* – User’s role on the platform. Examples: `"client"`, `"admin"`.
- **`status`** *(string)* – User’s status. Possible values: `"enabled"`, `"disabled"`.
- **`password`** *(string)* – User’s password (The password must contain at least 16 characters, including uppercase and lowercase letters, numbers, and special characters.).


**Example Payload:**
```json
{
  "Name": "John",
  "Email": "john@x.com",
  "Password": "StrongP@ssw0rd2025!",
  "Phone": "1234567890",
  "RoleId": "client",
  "Status": "enabled"
}
```
**Response Payload:**

```json
{
  "code": 200,
  "status": "success",
  "data": [
    {
      "user": {
        "id": 1,
        "Name": "John",
        "Email": "john@x.com",
        "Phone": "1234567890",
        "RoleId": "client",
        "Status": "enabled"
      }
    }
  ]
}
```

### 1.2 `POST /auth/signin`

#### Payload Parameters

The request body must include the following parameters:

- **`user`** *(object)*
  - **`email`** *(string)* – Email used for login.
  - **`phone`** *(string)* – (Optional) User’s full mobile phone number.
  - **`password`** *(string)* – User’s password.


*Example Payload:**
```json
{
  "email": "john@x.com",
  "password": "StrongP@ssw0rd2025!"
}
```
**Response Payload:**

```json
{
  "code": 200,
  "status": "success",
  "data": [
    {
      "user": {
      "id": 1,
      "Name": "John",
      "Email": "john@x.com",
      "Phone": "1234567890",
      "RoleId": "client",
      "Status": "enabled"
    },
    "token": "apiblahblahblahblah"
    }
  ]
}
```
---

### 1.3 `GET /auth/me`

#### Description
This endpoint retrieves the authenticated user's information based on the provided JWT token. It returns the user's details such as ID, name, email, phone, role, and status.

#### Response Payload:

```json
{
  "code": 200,
  "status": "success",
  "data": [
    "user":{
      "id":1,
      "Name": "John",
      "Email": "john@x.com",
      "Phone": "1234567890",
      "RoleId": "client",
      "Status": "enabled"
    }
  ]
}  
```

---

## Store Management

| Method | Endpoint               | Description                               | Access Level                   |
|--------|------------------------|-------------------------------------------|--------------------------------|
| `GET`  | `/api/store/{id}`      | Retrieve a store by its unique identifier | Public                         |
| `POST` | `/api/store/create`    | Create a new store                        | System Administrator           |
| `POST` | `/api/store/colab/add` | Add a collaborator to an existing store   | SysAdmin or StoreAdmin         |
| `POST` | `/api/store/client/add`| Add a client to an existing store         | Client, SysAdmin, StoreAdmin, StoreColab |


### 2.1 `GET /api/store/{id}`

**Description**: Fetches the details of a store by its unique identifier.

**Path Parameters**

**`id`** *(UUID)* – The unique ID of the store to retrieve.

#### Response Payload:

Response (200 OK)
```json
{
  "code": 200,
  "status": "success",
  "data": [
      "id": "ecf28a43-3cce-4df8-8a70-fde15b0f00ff",
      "name": "Main Street Store",
      "address": "123 Main St, Springfield",
      "status": "Active"
}
```
**Errors:**

- **404 Not Found** – Store not found with given ID.

### 2.2 `POST /api/store/create`
**Description**: Creates a new store. Only accessible to users with the System Administrator role.

**Headers**:
**Authorization**: Bearer {JWT_TOKEN}

*Example Payload:**
```json
{
"name": "Main Street Store",
"address": "123 Main St, Springfield"
}
```
#### Response Payload:
```json
{
  "code": 200,
  "status": "success",
  "data": [
    {
      "id": "ecf28a43-3cce-4df8-8a70-fde15b0f00ff",
      "name": "Main Street Store",
      "address": "123 Main St, Springfield",
      "status": "Active"
    }
  ]
}
```
**Errors**:

- **401 Unauthorized** – JWT is missing, expired, or invalid.

- **403 Forbidden** – Authenticated user does not have system admin rights.

- **400 Bad Request** – Business rule violation or invalid input.

### 2.3 `POST /api/store/colab/add`
Description: Adds a user as a collaborator to a specific store. Requires the caller to be either a System Administrator or the Store Administrator of the target store.

**Headers**:
**Authorization**: Bearer {JWT_TOKEN}

*Example Payload:**

```json
{
  "storeId": "UUID of the store",
  "userEmail": "Email of the user to be added"
}
```
#### Response Payload:
```json
{
  "code": 200,
  "status": "success",
  "data": [
      "message": "Collaborator successfully added to the store."
    ]
}
```
**Errors**:
- **401 Unauthorized** – If JWT is invalid or email claim is missing.

- **403 Forbidden** – If the user is neither a SysAdmin nor StoreAdmin of the store.

- **400 Bad Request** – If the collaborator does not have the correct role or other business rule fails.


### 2.4 `POST /api/store/client/add`
Description: Assigns a client user to a specific store. This endpoint supports two modes off operation:
- Client self-assignment: A user with the Client role assigns themselves to a store using the token. 
- Administrative assignment: A System Administrator, Store Administrator, or Store Collaborator assigns a client to a store by providing their userId.

**Headers**:
**Authorization**: Bearer {JWT_TOKEN}

***Example Payload:***

**Client self-assignment**

```json
{
  "storeId": "a34b9c00-8dcf-4b89-97ef-f234b84a81e2"
}
```

**Admin assigning a client by userId**

```json
{
  "storeId": "a34b9c00-8dcf-4b89-97ef-f234b84a81e2",
  "userId": "45b3fc18-923e-4454-b75d-2c6482be1f0e"
}
```

#### Response Payload:
```json
{
  "code": 200,
  "status": "success",
  "data": [
      "message": "Client successfully added to the store."
    ]
}
```
**Errors**:
- **401 Unauthorized** – If JWT is invalid or email claim is missing.

- **403 Forbidden** – If the user is neither a SysAdmin nor StoreAdmin nor a StoreCollab of the store, or a user with the client role.

- **400 Bad Request** – If the client does not have the correct role or other business rule fails.

---

## Product Management
| Method  | Endpoint                         | Description                                 | Access Level                                                    |
|---------|----------------------------------|---------------------------------------------|-----------------------------------------------------------------|
| `GET`   | `/api/product/{id}`              | Retrieve a product by its unique identifier | Public                                                          |
| `GET`   | `/api/product`                   | List all products                           | Public                                                          |
| `GET`   | `/api/product/{id}/image`        | Retrieve and decrypt product image          | Authorized users with access to the store of the product        |
| `POST`  | `/api/product/create`            | Create a new product                        | System Administrator, Store Administrator or Store Collaborator |
| `POST`  | `/api/product/{id}/upload-image` | Upload image for a product                  | System Administrator, Store Administrator or Store Collaborator |
| `PATCH` | `/api/product/{id}`              | Update a product                            | System Administrator, Store Administrator or Store Collaborator |


### 3.1 `GET /api/product/{id}`
**Description**: Fetches the details of a product by its unique identifier.
**Path Parameters**
- **`id`** *(UUID)* – The unique ID of the product to retrieve.

**Authorization**: Bearer {JWT_TOKEN}

#### Response Payload:
```json
{
  "code": 200,
  "status": "success",
  "data": [
    {
      "id": "ecf28a43-3cce-4df8-8a70-fde15b0f00ff",
      "name": "Product Name",
      "description": "Product Description",
      "price": 19.99,
      "imageUrl": "https://example.com/ProductId/image",
      "storeId": "store-uuid"
    }
  ]
}
```

**Errors**
- **404 Not Found** – Product not found with given ID.
- **401 Unauthorized** – JWT is missing, expired, or invalid.
- **400 Bad Request** – Business rule violation or invalid input.

### 3.2 `GET /api/product`
**Description**: Lists all products available in the system.
**Authorization**: Bearer {JWT_TOKEN}

#### Response Payload:
```json
{
  "code": 200,
  "status": "success",
  "data": [
    {
      "id": "ecf28a43-3cce-4df8-8a70-fde15b0f00ff",
      "name": "Product Name",
      "description": "Product Description",
      "price": 19.99,
      "imageUrl": "https://example.com/ProductId/image",
      "storeId": "store-uuid"
    },
    ...
  ]
}
```

**Errors**
- **401 Unauthorized** – JWT is missing, expired, or invalid.
- **400 Bad Request** – Business rule violation or invalid input.

### 3.3 `GET /api/product/{id}/image`

**Description**: Retrieves the image associated with a product, identified by its unique ID.

**Path Parameters**
- `id` *(UUID)* – The unique identifier of the product.

**Headers**
- `Authorization`: Bearer `{JWT_TOKEN}`

#### Success Response (200 OK)

Returns the binary image data with the appropriate `Content-Type` header (e.g., `image/jpeg`, `image/png`, etc.).

> This endpoint returns raw image bytes, not a JSON object.

**Response Headers:**
Content-Type: image/jpeg

**Response Body:**  
Binary content of the product image.

**Errors**
- **401 Unauthorized** – JWT is missing, expired, or invalid.
- **404 Not Found** – Product or its image not found.
- **400 Bad Request** – Business rule violation or invalid input.

### 3.4 `POST /api/product/create`
**Description**: Creates a new product. Accessible to System Administrators, Store Administrators, or Store Collaborators.
**Authorization**: Bearer {JWT_TOKEN}

#### Payload Parameters
- **`name`** *(string)* – Name of the product.
- **`description`** *(string)* – Description of the product.
- **`price`** *(number)* – Price of the product.
- **`category`** *(string)* – Category of the product.
- **`status`** *(string)* – Status of the product.
- **`storeId`** *(UUID)* – ID of the store to which the product belongs.

**Example Payload:**
```json
{
  "name": "New Product",
  "description": "This is a new product.",
  "price": 29.99,
  "category": "Electronics",
  "status": "enabled",
  "storeId": "ecf28a43-3cce-4df8-8a70-fde15b0f00ff"
}
```

#### Response Payload:
```json
{
  "code": 200,
  "status": "success",
  "data": [
    {
      "id": "ecf28a43-3cce-4df8-8a70-fde15b0f00ff",
      "name": "New Product",
      "description": "This is a new product.",
      "price": 29.99,
      "category": "Electronics",
      "status": "enabled",
      "storeId": "ecf28a43-3cce-4df8-8a70-fde15b0f00ff"
    }
  ]
}
```

**Errors**
- **401 Unauthorized** – JWT is missing, expired, invalid or user does not have permissions.
- **400 Bad Request** – Business rule violation or invalid input.

### 3.5 `POST /api/product/{id}/upload-image`
**Description**: Uploads an image for a specific product. Accessible to System Administrators, Store Administrators, or Store Collaborators.
**Path Parameters**
- **`id`** *(UUID)* – The unique ID of the product for which the image is being uploaded.
**Authorization**: Bearer {JWT_TOKEN}

#### Request Body
The request body must be a multipart/form-data containing the image file.

#### Response Payload:
```json
{
  "code": 200,
  "status": "success",
  "data": "Image uploaded successfully",
}
```

**Errors**
- **401 Unauthorized** – JWT is missing, expired, invalid or user does not have permissions.
- **400 Bad Request** – Business rule violation, invalid input, image size or file format invalid.
- **404 Not Found** – Product not found with given ID.
- **500 Internal Server Error** – If the image storage fails.

### 3.6 `PATCH /api/product/{id}`
**Description**: Updates an existing product. Accessible to System Administrators, Store Administrators, or Store Collaborators.
**Path Parameters**
- **`id`** *(UUID)* – The unique ID of the product to update.
**Authorization**: Bearer {JWT_TOKEN}

#### Payload Parameters
- **`name`** *(string)* – (Optional) New name of the product.
- **`description`** *(string)* – (Optional) New description of the product.
- **`price`** *(number)* – (Optional) New price of the product.
- **`category`** *(string)* – (Optional) New category of the product.
- **`status`** *(string)* – (Optional) New status of the product. Possible values: `"enabled"`, `"disabled"`.

**Example Payload:**
```json
{
  "name": "Updated Product Name",
  "description": "Updated description of the product.",
  "price": 24.99,
  "category": "Updated Category",
  "status": "enabled"
}
```

#### Response Payload:
```json
{
  "code": 200,
  "status": "success",
  "data": [
    {
      "id": "ecf28a43-3cce-4df8-8a70-fde15b0f00ff",
      "name": "Updated Product Name",
      "description": "Updated description of the product.",
      "price": 24.99,
      "category": "Updated Category",
      "status": "enabled",
      "storeId": "ecf28a43-3cce-4df8-8a70-fde15b0f00ff"
    }
  ]
}
```

**Errors**
- **401 Unauthorized** – JWT is missing, expired, invalid or user does not have permissions.
- **404 Not Found** – Product not found with given ID.
- **400 Bad Request** – Business rule violation or invalid input.

---
## Orders Management

| Method   | Endpoint                          | Description                                           | Access Level                                                   |
|----------|-----------------------------------|-------------------------------------------------------|----------------------------------------------------------------|
| `GET`    | `/api/order/{id}`                 | Retrieve an order by its unique identifier           | System Admin, Store Admin, Store Collab, or Client (self)      |
| `GET`    | `/api/order?limit={x}&offset={y}` | List orders with pagination                          | System Admin sees all; others see store/self orders            |
| `POST`   | `/api/order`                      | Create a new order                                   | System Admin, Store Admin, Store Collab, or Client (self)      |
| `PATCH`  | `/api/order/{id}`                 | Partially update an order (status/products)          | System Admin, Store Admin, Store Collab, or Owner              |
| `DELETE` | `/api/order/{id}`                 | Delete an order                                      | System Admin, Store Admin, Store Collab, or Owner              |

### 4.1 `GET /api/order/{id}`

**Description**: Retrieves the details of a specific order by its unique ID.

**Path Parameters**
- `id` *(UUID)* – The unique identifier of the order.

**Headers**
- `Authorization`: Bearer `{JWT_TOKEN}`

#### Response Payload:
```json
{
  "code": 200,
  "status": "success",
  "data": [
    {
      "id": "uuid",
      "userId": "uuid",
      "status": "string",
      "createdAt": "timestamp",
      "updatedAt": "timestamp",
      "products": [
        {
          "productId": "uuid",
          "amount": int,
          "price": decimal
        }
      ]
    }
  ]
}
```

**Errors**
- **401 Unauthorized** – JWT is missing, expired, invalid or user does not have permissions.
- **403 Forbidden** – Authenticated user does not have sufficient permissions to access or modify the requested resource.
- **404 Not Found** – Product not found with given ID.
- **400 Bad Request** – Business rule violation or invalid input.

### 4.2 `GET /api/order?limit={x}&offset={y}`

**Description**: Retrieves a paginated list of orders accessible by the authenticated user.

**Query Parameters**
- `limit` *(int)* – Number of orders to retrieve. Default is 10.
- `offset` *(int)* – Number of orders to skip. Default is 0.

**Headers**
- `Authorization`: Bearer `{JWT_TOKEN}`

#### Response Payload:
```json
{
  "code": 200,
  "status": "success",
  "data": [
    {
      "id": "uuid",
      "userId": "uuid",
      "status": "string",
      "createdAt": "timestamp",
      "updatedAt": "timestamp",
      "products": [
        {
          "productId": "uuid",
          "amount": int,
          "price": decimal
        }
      ]
    },
    ...
  ]
}
```
**Errors**
- **401 Unauthorized** – JWT is missing, expired, invalid or user does not have permissions.
- **403 Forbidden** – Authenticated user does not have sufficient permissions to access or modify the requested resource.
- **404 Not Found** – Product not found with given ID.
- **400 Bad Request** – Business rule violation or invalid input.

### 4.3 `POST /api/order`

**Description**: Creates a new order with the specified products. If no userId is provided, the order is created for the authenticated user.

**Headers**
- `Authorization`: Bearer `{JWT_TOKEN}`

#### Example Payload:
```json
{
  "userId": "uuid (optional)",
  "status": "string",
  "products": [
    {
      "productId": "uuid",
      "amount": int,
      "price": decimal
    }
  ]
}
```

#### Response Payload:
```json
{
  "code": 200,
  "status": "success",
  "data": [
    {
      "id": "uuid",
      "userId": "uuid",
      "status": "string",
      "createdAt": "timestamp",
      "updatedAt": "timestamp",
      "products": [
        {
          "productId": "uuid",
          "amount": int,
          "price": decimal
        }
      ]
    }
  ]
}
```

**Errors**
- **401 Unauthorized** – JWT is missing, expired, invalid or user does not have permissions.
- **403 Forbidden** – Authenticated user does not have sufficient permissions to access or modify the requested resource.
- **404 Not Found** – Product not found with given ID.
- **400 Bad Request** – Business rule violation or invalid input.

### 4.4 `PATCH /api/order/{id}`

**Description**: Updates part of an existing order, including status and/or products.

**Path Parameters**
- `id` *(UUID)* – The ID of the order to update.

**Headers**
- `Authorization`: Bearer `{JWT_TOKEN}`


#### Example Payload:
```json
{
  "status": "string",
  "products": [
    {
      "productId": "uuid",
      "amount": int,
      "price": decimal
    }
  ]
}
```

#### Response Payload:
```json
{
  "code": 200,
  "status": "success",
  "data": [
    {
      "id": "uuid",
      "userId": "uuid",
      "status": "string",
      "createdAt": "timestamp",
      "updatedAt": "timestamp",
      "products": [
        {
          "productId": "uuid",
          "amount": int,
          "price": decimal
        }
      ]
    }
  ]
}
```

**Errors**
- **401 Unauthorized** – JWT is missing, expired, invalid or user does not have permissions.
- **403 Forbidden** – Authenticated user does not have sufficient permissions to access or modify the requested resource.
- **404 Not Found** – Product not found with given ID.
- **400 Bad Request** – Business rule violation or invalid input.

---

### 4.5 `DELETE /api/order/{id}`

**Description**: Deletes an order by ID if the user is the owner, a store operator, or a sysadmin.

**Path Parameters**
- `id` *(UUID)* – The ID of the order to delete.

**Headers**
- `Authorization`: Bearer `{JWT_TOKEN}`

#### Response Payload:
```json
{
  "code": 200,
  "status": "success",
  "data": [
    {
      "message": "Order {id} deleted successfully"
    }
  ]
}
```

**Errors**
- **401 Unauthorized** – JWT is missing, expired, invalid or user does not have permissions.
- **403 Forbidden** – Authenticated user does not have sufficient permissions to access or modify the requested resource.
- **404 Not Found** – Product not found with given ID.
- **400 Bad Request** – Business rule violation or invalid input.


---

## Database Management

Created users for the application and the application logs.
Each user has the minimum required permissions to do their job.

### ShopTex Database
It was created a user the username `shoptex` with the password stored in the .env file. It has the following permissions:
- **`SELECT`** – Read data from the ShopTex database.
- **`INSERT`** – Add new records to the ShopTex database.
- **`UPDATE`** – Modify existing records in the ShopTex database.
- **`DELETE`** – Remove records from the ShopTex database.

### Application Logs Database
It was created a user the username `shoptex_logs` with the password stored in the .env file. It has the following permissions:
- **`INSERT`** – Add new records to the application logs database.

---

## Security Features

### JWT Authentication
The application uses JWT (JSON Web Tokens) for secure user authentication. The token is generated upon successful login and must be included in the `Authorization` header of subsequent requests.
This token is signed with a secret key to ensure its integrity and authenticity. The token contains user information and expiration time, allowing the server to verify the user's identity without needing to access the database for every request.


### Password Hashing
All user passwords are securely hashed using the `bcrypt` algorithm before being stored in the database. This ensures that even if the database is compromised, user passwords remain protected.
It also uses salt to enhance security against rainbow table attacks.

### Environment Variables
The application uses environment variables to store sensitive information such as database credentials and JWT secret keys. This prevents hardcoding sensitive data in the source code, reducing the risk of exposure.

---

## HTTPS
The application is configured to run over HTTPS, ensuring that all data transmitted between the client and server is encrypted. This protects against eavesdropping

---

## Configuration Management

The application uses a `.env` file to manage configuration settings, including database connection strings, JWT secret keys, and other sensitive information. This file is not included in the version control system to prevent accidental exposure of sensitive data.
This repository has action secret management enabled, which allows us to securely store and manage sensitive information such as API keys, database credentials, and other secrets required for the application to function properly.
The appsettings.json file is used to store application port and other configuration settings. It is not used to store sensitive information, as this should be managed through environment variables or the `.env` file.

---

## Logging and Monitoring

The application implements logging to capture important events and errors. Logs are stored in a separate database (`shoptex_logs`) to ensure that application performance is not affected by logging operations.
It was implemented transactional logging, which ensures that all database operations are logged consistently. This allows for better tracking of changes and easier debugging of issues.

