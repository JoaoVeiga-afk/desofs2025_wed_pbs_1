## Features

### User Management


| Method | Endpoint       | Description                                   | Access Level |
|--------|----------------|-----------------------------------------------|--------------|
| `POST` | `/auth/signin` | Validate credentials and return the JWT Token | Public       |
| `POST` | `/auth/signup` | Creates the user account                      | Public       |
| `GET`  | `/auth/me`     | Get user info by token                        | Public        |

#### `POST /auth/signup`

### Payload Parameters

The parameters required to create a new user:

- **`id`** *(integer)* – Unique identifier for the user.
- **`name`** *(string)* – User’s full name.
- **`email`** *(string)* – User’s unique email address (must be valid).
- **`phone`** *(string)* – User’s unique mobile phone number (must be valid).
- **`role`** *(string)* – User’s role on the platform. Examples: `"client"`, `"admin"`.
- **`status`** *(string)* – User’s status. Possible values: `"enabled"`, `"disabled"`.
- **`password`** *(string)* – User’s password (must be at least 8 characters long).
- 

**Example Payload:**
```json
{
  "Name": "John",
  "Email": "john@x.com",
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

#### 1.2 `POST /auth/signin'

### Payload Parameters

The request body must include the following parameters:

- **`user`** *(object)*
  - **`email`** *(string)* – Email used for login.
  - **`phone`** *(string)* – (Optional) User’s full mobile phone number.
  - **`password`** *(string)* – User’s password.


*Example Payload:**
```json
{
  "email": "john@x.com",
  "password": "password123"
}
```
**Response Payload:**

```json
{
  "code": 200,
  "status": "success",
  "data": [
    "user"{
      "id":1,
      "Name": "John",
      "Email": "john@x.com",
      "Phone": "1234567890",
      "RoleId": "client",
      "Status": "enabled"
    },
    "token":"apiblahblahblahblah"
  ]
}
```
---

#### 1.3 `GET /auth/me`

### Description
This endpoint retrieves the authenticated user's information based on the provided JWT token. It returns the user's details such as ID, name, email, phone, role, and status.

### Response Payload:

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

### Store Management
| Method | Endpoint               | Description                               | Access Level           |
| ------ | ---------------------- | ----------------------------------------- | ---------------------- |
| `GET`  | `/api/store/{id}`      | Retrieve a store by its unique identifier | Public                 |
| `POST` | `/api/store/create`    | Create a new store                        | System Administrator   |
| `POST` | `/api/store/colab/add` | Add a collaborator to an existing store   | SysAdmin or StoreAdmin |


#### 2.1 'GET /api/store/{id}'

**Description**: Fetches the details of a store by its unique identifier.

**Path Parameters**
id (UUID) – The unique ID of the store to retrieve.

### Response Payload:

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
**Errors**

**404 Not Found** – Store not found with given ID.

#### 2.2 'POST /api/store/create'
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
### Response Payload:
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
**Errors**:
**401 Unauthorized** – JWT is missing, expired, or invalid.

**403 Forbidden** – Authenticated user does not have system admin rights.

**400 Bad Request** – Business rule violation or invalid input.

#### 2.3 'POST /api/store/colab/add'
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
### Response Payload:
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
**401 Unauthorized** – If JWT is invalid or email claim is missing.

**403 Forbidden** – If the user is neither a SysAdmin nor StoreAdmin of the store.

**400 Bad Request** – If the collaborator does not have the correct role or other business rule fails.

