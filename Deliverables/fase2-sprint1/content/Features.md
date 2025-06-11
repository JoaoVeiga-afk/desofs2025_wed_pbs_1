## Features

### User Management


| Method | Endpoint       | Description                                   | Access Level |
|--------|----------------|-----------------------------------------------|--------------|
| `POST` | `/auth/signin` | Validate credentials and return the JWT Token | Public       |
| `POST` | `/auth/signup` | Creates the user account                      | Public       |

#### `POST /auth/signup`

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

### Database Management

Created users for the application and the application logs.
Each user has the minimum required permissions to do their job.

#### ShopTex Database
It was created a user the username `shoptex` with the password stored in the .env file. It has the following permissions:
- **`SELECT`** – Read data from the ShopTex database.
- **`INSERT`** – Add new records to the ShopTex database.
- **`UPDATE`** – Modify existing records in the ShopTex database.
- **`DELETE`** – Remove records from the ShopTex database.

#### Application Logs Database
It was created a user the username `shoptex_logs` with the password stored in the .env file. It has the following permissions:
- **`INSERT`** – Add new records to the application logs database.

### Security Features

#### JWT Authentication
The application uses JWT (JSON Web Tokens) for secure user authentication. The token is generated upon successful login and must be included in the `Authorization` header of subsequent requests.

#### Password Hashing
All user passwords are securely hashed using the `bcrypt` algorithm before being stored in the database. This ensures that even if the database is compromised, user passwords remain protected.
It also uses salt to enhance security against rainbow table attacks.

#### Environment Variables
The application uses environment variables to store sensitive information such as database credentials and JWT secret keys. This prevents hardcoding sensitive data in the source code, reducing the risk of exposure.

### HTTPS
The application is configured to run over HTTPS, ensuring that all data transmitted between the client and server is encrypted. This protects against eavesdropping
