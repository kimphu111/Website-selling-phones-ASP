# PhoneShop Management System (FE/BE)

## 1) Setup
- .NET SDK **9.0+**
- MySQL Server **8.0+**

## 2) Create Database (MySQL)
Run `data.sql`:

```sql
source data.sql;
```

> Database name: `phone_shop_app`

## 3) Run Backend (BE - Web API)
From project root (`d:/nam4/SA/ProjectTIen`):

```bash
dotnet run --project BE/PhoneShopApp.BE.Api
```

- BE URL: **http://localhost:5201**

## 4) Run Frontend (FE - MVC)
From project root (`d:/nam4/SA/ProjectTIen`):

```bash
dotnet run --project FE/PhoneShopApp.FE.Web
```

- FE URL: **http://localhost:5298**

## 5) Test Login
- Username: `admin`
- Password: `admin123`

## 6) Auth / Authorization
- FE stores JWT in **Session** after login.
- BE uses `[Authorize]` and `[Authorize(Roles="Admin")]` for protected endpoints.

## 7) API Endpoints (BE)
| Method | URL | Auth | Description |
|---|---|---|---|
| GET | `/api/phones?keyword=&searchBy=` | JWT | Search phones |
| POST | `/api/phones` | Admin | Create phone |
| PUT | `/api/phones/{id}` | Admin | Update phone |
| PATCH | `/api/phones/{id}/status` | Admin | Approve/Reject |
| DELETE | `/api/phones/{id}` | Admin | Delete phone |
| GET | `/api/users` | Admin | List users |
| POST | `/api/users` | Admin | Create user |

## 8) Notes
- Do run commands from the **solution root** so the `--project FE/...` paths resolve correctly.
- Do not run from inside `BE/` or `FE/` unless you adjust the `--project` path.

