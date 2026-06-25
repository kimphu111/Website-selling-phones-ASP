CREATE DATABASE IF NOT EXISTS phone_shop_app;

USE phone_shop_app;

-- Bảng users (AppUser)
CREATE TABLE IF NOT EXISTS users (
  id INT NOT NULL AUTO_INCREMENT,
  username VARCHAR(100) NOT NULL,
  full_name VARCHAR(200) NOT NULL,
  password_hash VARCHAR(255) NOT NULL,
  role VARCHAR(50) NOT NULL,
  PRIMARY KEY (id),
  UNIQUE KEY uq_users_username (username)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Bảng phones (Phone)
CREATE TABLE IF NOT EXISTS phones (
  id INT NOT NULL AUTO_INCREMENT,
  model VARCHAR(200) NOT NULL,
  brand VARCHAR(200) NOT NULL,
  specifications VARCHAR(1000) NULL,
  status VARCHAR(50) NOT NULL,
  PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Admin user
INSERT INTO users (username, full_name, password_hash, role)
SELECT 'admin', 'System Admin',
       '240BE518FABD2724DDB6F04EEB1DA5967448D7E831C08C8FA822809F74C720A9',
       'Admin';

-- Phones sample
INSERT INTO phones (model, brand, specifications, status) VALUES
('iPhone 15 Pro Max', 'Apple', 'A17 Pro chip, 8GB RAM, 256GB, Titanium', 'Pending'),
('Galaxy S24 Ultra', 'Samsung', 'Snapdragon 8 Gen 3, 12GB RAM, 512GB, Titanium Gray', 'Pending'),
('Pixel 8 Pro', 'Google', 'Google Tensor G3, 12GB RAM, 256GB, AI features', 'Pending'),
('Xperia 1 V', 'Sony', 'Snapdragon 8 Gen 2, 12GB RAM, 256GB, 4K display', 'Pending'),
('OnePlus 12', 'OnePlus', 'Snapdragon 8 Gen 3, 16GB RAM, 512GB, Hasselblad', 'Pending'),
('Xiaomi 14 Ultra', 'Xiaomi', 'Snapdragon 8 Gen 3, 16GB RAM, 512GB, Leica optics', 'Pending'),
('Nothing Phone 2', 'Nothing', 'Snapdragon 8+ Gen 1, 12GB RAM, 256GB, Glyph interface', 'Pending'),
('ROG Phone 8', 'Asus', 'Snapdragon 8 Gen 3, 16GB RAM, 512GB, Gaming phone', 'Pending'),
('Galaxy Z Fold 5', 'Samsung', 'Snapdragon 8 Gen 2, 12GB RAM, 512GB, Foldable', 'Pending'),
('iPhone 15', 'Apple', 'A16 Bionic chip, 6GB RAM, 128GB', 'Pending'),
('Redmi Note 13 Pro+', 'Xiaomi', 'Dimensity 7200 Ultra, 12GB RAM, 256GB', 'Pending'),
('Oppo Find X7 Ultra', 'Oppo', 'Snapdragon 8 Gen 3, 16GB RAM, 512GB', 'Pending'),
('Vivo X100 Pro', 'Vivo', 'Dimensity 9300, 16GB RAM, 512GB', 'Pending'),
('Mate 60 Pro', 'Huawei', 'Kirin 9000S, 12GB RAM, 512GB', 'Pending'),
('Galaxy A55', 'Samsung', 'Exynos 1480, 8GB RAM, 256GB', 'Pending'),
('iPhone SE 2024', 'Apple', 'A16 Bionic, 6GB RAM, 128GB', 'Pending'),
('Pixel 8a', 'Google', 'Tensor G3, 8GB RAM, 256GB', 'Pending'),
('Motorola Edge 50 Pro', 'Motorola', 'Snapdragon 7 Gen 3, 12GB RAM, 256GB', 'Pending'),
('Honor Magic 6 Pro', 'Honor', 'Snapdragon 8 Gen 3, 12GB RAM, 512GB', 'Pending'),
('Zenfone 11 Ultra', 'Asus', 'Snapdragon 8 Gen 3, 16GB RAM, 512GB', 'Pending');
