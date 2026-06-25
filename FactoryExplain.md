# Factory — Giải thích flow chi tiết (Search phones)

## Mục tiêu
Giải thích **đường đi thực thi** của pattern **Factory Method** trong module **Search** của hệ thống quản lý Phone.

- Factory Method: `PhoneSearchFactoryMethod.Create(searchBy)`
- Strategy: implement `IPhoneSearchStrategy.Apply(phones, keyword)`
- Dùng trong: `PhoneService.SearchAsync()`

---

## 1) Luồng request tổng thể (FE → BE)

### 1.1 FE: `FE/PhoneShopApp.FE.Web/Controllers/PhonesController.cs`
- Action `Index(string? keyword, string? searchBy, ...)`
- Lấy JWT từ Session:
  ```csharp
  private string? GetToken() => HttpContext.Session.GetString("jwt_token");
  ```
- Gọi backend:
  ```csharp
  var phones = await _backendApiClient.SearchPhonesAsync(keyword, searchBy, token, cancellationToken);
  ```

**Vai trò với Factory:** FE chỉ truyền tham số `keyword` và `searchBy` sang BE.

---

### 1.2 FE: `FE/PhoneShopApp.FE.Web/Services/BackendApiClient.cs`
- Method `SearchPhonesAsync(string? keyword, string? searchBy, string token, ...)`
- Set header:
  ```csharp
  client.DefaultRequestHeaders.Authorization =
      new AuthenticationHeaderValue("Bearer", token);
  ```
- Build query string:
  - `keyword=...` nếu `keyword` không rỗng
  - `searchBy=...` nếu `searchBy` không rỗng
- Gọi BE:
  ```csharp
  GET /api/phones?keyword=...&searchBy=...
  ```

---

### 1.3 BE API: `BE/PhoneShopApp.BE.Api/Controllers/PhonesController.cs`
- Endpoint:
  ```csharp
  [HttpGet]
  public async Task<ActionResult<List<PhoneDto>>> Search(
      [FromQuery] string? keyword,
      [FromQuery] string? searchBy,
      CancellationToken cancellationToken)
  ```
- Gọi facade:
  ```csharp
  var phones = await _phoneShopFacade.SearchPhonesAsync(keyword, searchBy, cancellationToken);
  return Ok(phones);
  ```

---

### 1.4 Facade: `BE/PhoneShopApp.BE.Services/Patterns/Facade/PhoneShopFacade.cs`
- Method `SearchPhonesAsync(...)` chỉ chuyển tiếp:
  ```csharp
  return _phoneService.SearchAsync(keyword, searchBy, cancellationToken);
  ```

---

## 2) Nơi Factory Method được kích hoạt (trọng tâm)

### Services: `BE/PhoneShopApp.BE.Services/Implementations/PhoneService.cs`
Method liên quan:
```csharp
public async Task<List<PhoneDto>> SearchAsync(string? keyword, string? searchBy, CancellationToken cancellationToken = default)
```

#### Bước 1 — Lấy danh sách phones
```csharp
var phones = await _phoneRepository.ListAsync(cancellationToken);
```

#### Bước 2 — Chỉ lọc khi keyword hợp lệ
```csharp
if (!string.IsNullOrWhiteSpace(keyword))
{
    var strategy = _searchFactory.Create(searchBy); // <<< Factory Method
    phones = strategy.Apply(phones, keyword).ToList(); // <<< Strategy thực thi
}
```

=> **Đây là điểm Factory Method quyết định sẽ dùng strategy nào.**

---

## 3) Factory là gì? (và Factory trong dự án này làm gì)

### 3.1 “Factory” trong OOP dùng để làm gì?
- Factory dùng để **tạo ra đúng kiểu đối tượng** dựa trên dữ liệu đầu vào (ở đây là `searchBy`).
- Thay vì code xử lý bằng `if/else` hoặc `switch` rải rác trong logic (vd trong `PhoneService`), ta dồn quyết định “tạo cái gì” vào 1 chỗ: **Factory**.

### 3.2 Factory trong dự án này làm gì?
- `PhoneSearchFactoryMethod` nhận tham số `searchBy`.
- Dựa vào `searchBy`, nó **tạo một Strategy** phù hợp:
  - `model` → tạo `ModelSearchStrategy`
  - `brand` → tạo `BrandSearchStrategy`
  - khác/null → tạo `AllFieldSearchStrategy`
- Sau đó `PhoneService` sẽ gọi `strategy.Apply(phones, keyword)` để thực hiện lọc.

---

## 4) Factory Method: `PhoneSearchFactoryMethod.cs`


### File: `BE/PhoneShopApp.BE.Services/Patterns/Factory/PhoneSearchFactoryMethod.cs`

```csharp
public class PhoneSearchFactoryMethod
{
    public virtual IPhoneSearchStrategy Create(string? searchBy)
    {
        return searchBy?.ToLowerInvariant() switch
        {
            "model" => new ModelSearchStrategy(),
            "brand" => new BrandSearchStrategy(),
            _ => new AllFieldSearchStrategy()
        };
    }
}
```

### Quy tắc chọn strategy
- `searchBy = "model"`  → tạo `ModelSearchStrategy`
- `searchBy = "brand"`  → tạo `BrandSearchStrategy`
- `searchBy = null/khác` → fallback tạo `AllFieldSearchStrategy`

### Ý nghĩa kiến trúc
- `PhoneService` **không cần if/switch** phức tạp.
- Việc thêm loại search mới chỉ cần:
  1) tạo strategy implement `IPhoneSearchStrategy`
  2) thêm case trong factory

---

## 4) Strategy Interface và từng strategy (lọc dữ liệu theo keyword)

### 4.1 Interface: `IPhoneSearchStrategy.cs`

```csharp
public interface IPhoneSearchStrategy
{
    IEnumerable<Phone> Apply(IEnumerable<Phone> phones, string keyword);
}
```
- Chuẩn hóa cách mọi strategy lọc:
  - Input: danh sách `phones` + `keyword`
  - Output: danh sách `Phone` sau lọc

---

### 4.2 `ModelSearchStrategy.cs`

```csharp
return phones.Where(x => x.Model.Contains(keyword, StringComparison.OrdinalIgnoreCase));
```
- Match theo **Phone.Model**

---

### 4.3 `BrandSearchStrategy.cs`

```csharp
return phones.Where(x => x.Brand.Contains(keyword, StringComparison.OrdinalIgnoreCase));
```
- Match theo **Phone.Brand**

---

### 4.4 `AllFieldSearchStrategy.cs`

```csharp
return phones.Where(x =>
    x.Model.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
    x.Brand.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
    x.Specifications.Contains(keyword, StringComparison.OrdinalIgnoreCase));
```
- Match theo **Model OR Brand OR Specifications**
- Là default khi `searchBy` không khớp `model` hoặc `brand`.

---

## 5) “Đường đi” ngắn gọn (đúng 5 bước Factory/Strategy trong BE)

1. FE gọi `GET /api/phones?keyword=...&searchBy=...`
2. BE `PhonesController.Search()` gọi `PhoneShopFacade.SearchPhonesAsync()`
3. `PhoneShopFacade` gọi `PhoneService.SearchAsync()`
4. `PhoneService.SearchAsync()`:
   - lấy list phone từ repository
   - nếu có keyword:
     - `strategy = PhoneSearchFactoryMethod.Create(searchBy)`
     - `phones = strategy.Apply(phones, keyword)`
5. `PhoneService` tiếp tục map sang `PhoneDto` và trả về JSON

---

## 6) Ví dụ cụ thể để nhìn rõ Factory decision

### Trường hợp A: `searchBy=model`
- Factory tạo: `new ModelSearchStrategy()`
- Lọc: `phone.Model chứa keyword`

### Trường hợp B: `searchBy=brand`
- Factory tạo: `new BrandSearchStrategy()`
- Lọc: `phone.Brand chứa keyword`

### Trường hợp C: `searchBy` rỗng/khác
- Factory tạo: `new AllFieldSearchStrategy()`
- Lọc: `Model hoặc Brand hoặc Specifications chứa keyword`

