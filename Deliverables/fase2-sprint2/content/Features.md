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