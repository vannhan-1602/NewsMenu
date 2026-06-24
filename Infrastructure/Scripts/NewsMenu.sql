

-- 1. Bảng Countries (Quốc gia)
CREATE TABLE Countries (
    country_id  INT IDENTITY(1,1) PRIMARY KEY,
    name        NVARCHAR(255) NOT NULL,
    is_deleted  BIT          NOT NULL DEFAULT 0,
    created_at  DATETIME2    NOT NULL DEFAULT GETUTCDATE(),
    updated_at  DATETIME2    NOT NULL DEFAULT GETUTCDATE()
);

-- 2. Bảng Wards (Tỉnh/TP và Phường/Xã - tự đệ quy)
--    parent_id = 0    Tỉnh/TP (gốc, không có cha)
--    parent_id = N    Phường/Xã thuộc Tỉnh/TP có ward_id = N
CREATE TABLE Wards (
    ward_id     INT IDENTITY(1,1) PRIMARY KEY,
    name        NVARCHAR(255) NOT NULL,
    parent_id   INT          NOT NULL DEFAULT 0,
    country_id  INT          NOT NULL,
    is_deleted  BIT          NOT NULL DEFAULT 0,
    created_at  DATETIME2    NOT NULL DEFAULT GETUTCDATE(),
    updated_at  DATETIME2    NOT NULL DEFAULT GETUTCDATE()
);

-- 3. Bảng Menus
CREATE TABLE Menus (
    menu_id       INT IDENTITY(1,1) PRIMARY KEY,
    name          NVARCHAR(255) NOT NULL,
    slug          NVARCHAR(255) NOT NULL,
    display_order INT          NOT NULL DEFAULT 0,
    is_deleted    BIT          NOT NULL DEFAULT 0,
    created_at    DATETIME2    NOT NULL DEFAULT GETUTCDATE(),
    updated_at    DATETIME2    NOT NULL DEFAULT GETUTCDATE()
);

-- 4. Bảng News
CREATE TABLE News (
    news_id      INT IDENTITY(1,1) PRIMARY KEY,
    title        NVARCHAR(500) NOT NULL,
    content      NVARCHAR(MAX) NOT NULL,
    summary      NVARCHAR(1000) NULL,
    is_published BIT           NOT NULL DEFAULT 0,
    is_deleted   BIT           NOT NULL DEFAULT 0,
    created_at   DATETIME2     NOT NULL DEFAULT GETUTCDATE(),
    updated_at   DATETIME2     NOT NULL DEFAULT GETUTCDATE(),
    ward_id      INT           NULL,
    address      NVARCHAR(500) NULL
);

-- 5. Bảng trung gian MenuNews (quan hệ N-N giữa Menus và News)

CREATE TABLE MenuNews (
    menu_id     INT       NOT NULL,
    news_id     INT       NOT NULL,
    assigned_at DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT PK_MenuNews PRIMARY KEY (menu_id, news_id)
);



-- Quốc gia
INSERT INTO Countries (name, is_deleted, created_at, updated_at) VALUES
(N'Việt Nam', 0, GETUTCDATE(), GETUTCDATE());

-- Tỉnh/Thành phố (parent_id = 0)
INSERT INTO Wards (name, parent_id, country_id, is_deleted, created_at, updated_at) VALUES
(N'Thành phố Hà Nội',      0, 1, 0, GETUTCDATE(), GETUTCDATE()),
(N'Thành phố Hồ Chí Minh', 0, 1, 0, GETUTCDATE(), GETUTCDATE()),
(N'Thành phố Đà Nẵng',     0, 1, 0, GETUTCDATE(), GETUTCDATE()),
(N'Tỉnh Bình Dương',        0, 1, 0, GETUTCDATE(), GETUTCDATE()),
(N'Tỉnh Đồng Nai',          0, 1, 0, GETUTCDATE(), GETUTCDATE());

-- Phường/Xã Hà Nội (parent_id = 1)
INSERT INTO Wards (name, parent_id, country_id, is_deleted, created_at, updated_at) VALUES
(N'Phường Hoàn Kiếm',  1, 1, 0, GETUTCDATE(), GETUTCDATE()),
(N'Phường Ba Đình',     1, 1, 0, GETUTCDATE(), GETUTCDATE()),
(N'Phường Hoàng Mai',  1, 1, 0, GETUTCDATE(), GETUTCDATE()),
(N'Phường Cầu Giấy',   1, 1, 0, GETUTCDATE(), GETUTCDATE());

-- Phường/Xã TP HCM (parent_id = 2)
INSERT INTO Wards (name, parent_id, country_id, is_deleted, created_at, updated_at) VALUES
(N'Phường Bến Nghé',   2, 1, 0, GETUTCDATE(), GETUTCDATE()),
(N'Phường Phú Nhuận',  2, 1, 0, GETUTCDATE(), GETUTCDATE()),
(N'Phường Bình Thạnh', 2, 1, 0, GETUTCDATE(), GETUTCDATE()),
(N'Phường Thủ Đức',    2, 1, 0, GETUTCDATE(), GETUTCDATE());

-- Phường/Xã Đà Nẵng (parent_id = 3)
INSERT INTO Wards (name, parent_id, country_id, is_deleted, created_at, updated_at) VALUES
(N'Phường Hải Châu',   3, 1, 0, GETUTCDATE(), GETUTCDATE()),
(N'Phường Thanh Khê',  3, 1, 0, GETUTCDATE(), GETUTCDATE());

