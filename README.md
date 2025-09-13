# 🔐 IdentityRefreshToken — Authorization system with ASP.NET Core and JWT

**IdentityRefreshToken** is a simple but complete project that shows how to build an authentication system with:

- Registration and login
- Access and refresh tokens
- Protected API access
- Frontend with login form and protected page

The solution includes **3 small projects**:

- `AuthAPI` — handles login, register, and token generation
- `ProtectedAPI` — allows access only with a valid token
- `Frontend` — web interface (Razor) that communicates with both APIs

---

## 📦 Project structure

```
IdentityRefreshToken/
├── AuthAPI/         # Registration and login logic
├── ProtectedAPI/    # Protected data with JWT check
└── Frontend/        # Web client with login, token handling and views
```

---

## 🔐 AuthAPI

This is the authentication service. It allows users to register, log in, and refresh their token.

### Features:

- Register new user
- Login with email and password
- Create access token (JWT)
- Create and store refresh token in database
- Update tokens with `/refresh`
- Return clear JSON responses
- Use FluentValidation
- Logs actions for debugging

### Main endpoints:

```http
POST /api/user/register
POST /api/user/login
POST /api/user/refresh
```

### Technologies:

- ASP.NET Core 8
- ASP.NET Identity
- Entity Framework Core
- JWT tokens
- FluentValidation
- SQL Server (LocalDb)
- Swagger for testing

---

## 🔒 ProtectedAPI

This API allows access only with a valid JWT token.

### Features:

- Validates token from header
- Returns protected data only if token is valid
- Shows 401 / 403 messages if not authorized
- Global error handler
- Swagger with JWT token support

### Example:

```http
GET /api/protected
Authorization: Bearer {access_token}
```

---

## 🌐 Frontend (Razor client)

This is the user interface. It is a website with login/register forms and a page to show protected data.

### Features:

- Sends login/register data to AuthAPI
- Saves tokens to cookies using `TokenManager`
- Sends access token to ProtectedAPI
- Automatically refreshes token if expired
- Shows messages for success or error
- Uses cookies only for Razor login display (`ClaimsPrincipal`)

❗ **Note**: The website does not use cookies for access to the API. It uses `Authorization: Bearer` in every API request.

---

## 🚀 Quick start

### 1. Clone the project

```bash
git clone https://github.com/yourusername/IRefreshToken.git
```

### 2. Setup the database

Open `appsettings.json` in `AuthAPI` and set your connection string:

```json
"ConnectionStrings": {
  "Identity": "Server=(localdb)\MSSQLLocalDB;Database=IdRefresh;Trusted_Connection=True;"
}
```

### 3. Create the database

In terminal, run this inside the `AuthAPI` folder:

```bash
dotnet ef database update
```

### 4. Run all 3 projects

Make sure these ports are correct (or update them):

- AuthAPI: `https://localhost:7236`
- ProtectedAPI: `https://localhost:7136`
- Frontend: `https://localhost:7185`

In `Frontend/Program.cs`, check the base URLs:

```csharp
builder.Services.AddHttpClient<AuthHttpClient>(client => client.BaseAddress = new Uri("https://localhost:7236"));
builder.Services.AddHttpClient<ProtectedHttpClient>(client => client.BaseAddress = new Uri("https://localhost:7136"));
```

---

## ✅ How to test

1. Open the Frontend in browser  
2. Register or log in  
3. Click "Protected" — it will load data from ProtectedAPI  
4. If token is expired — it will be refreshed automatically  
5. Click "Logout" to remove tokens  

---

## 🧪 Test cases

- Wrong password → shows error  
- No token → redirects to Login  
- Expired refresh token → logs out  

---

## 🛠 Technologies used

- ASP.NET Core 8
- Identity + EF Core
- JWT (access + refresh)
- Razor Views (MVC)
- FluentValidation
- Swagger
- Middleware (for errors)
- Cookies for Claims only

---

## 📄 License

MIT — free to use and modify.