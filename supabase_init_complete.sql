-- ================================================================
-- 寵物健康管理系統 - Supabase 完整初始化腳本
-- ⚠️  此版本欄位名稱完全符合 EF Core Migration
-- ================================================================

-- 清除現有表
DROP TABLE IF EXISTS "Alerts" CASCADE;
DROP TABLE IF EXISTS "Appointments" CASCADE;
DROP TABLE IF EXISTS "DailyLogs" CASCADE;
DROP TABLE IF EXISTS "HealthData" CASCADE;
DROP TABLE IF EXISTS "Pets" CASCADE;
DROP TABLE IF EXISTS "Users" CASCADE;

-- 1. Users
CREATE TABLE "Users" (
    "ID" SERIAL PRIMARY KEY,
    "Username" VARCHAR(100) NOT NULL UNIQUE,
    "Email" VARCHAR(255) NOT NULL UNIQUE,
    "PasswordHash" VARCHAR(255) NOT NULL,
    "FullName" VARCHAR(100),
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

-- 2. Pets
CREATE TABLE "Pets" (
    "ID" SERIAL PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL,
    "Species" VARCHAR(50) NOT NULL,
    "Birthday" TIMESTAMP WITH TIME ZONE,
    "WeightGoal" NUMERIC(5,2),
    "UserID" INTEGER NOT NULL REFERENCES "Users"("ID") ON DELETE CASCADE
);

CREATE INDEX "IX_Pets_UserID" ON "Pets"("UserID");

-- 3. Alerts
CREATE TABLE "Alerts" (
    "ID" SERIAL PRIMARY KEY,
    "PetID" INTEGER NOT NULL REFERENCES "Pets"("ID") ON DELETE CASCADE,
    "Type" VARCHAR(50) NOT NULL,
    "Severity" VARCHAR(20) NOT NULL,
    "Message" VARCHAR(500) NOT NULL,
    "IsResolved" BOOLEAN NOT NULL DEFAULT FALSE,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX "IX_Alerts_PetID" ON "Alerts"("PetID");

-- 4. Appointments
CREATE TABLE "Appointments" (
    "ID" SERIAL PRIMARY KEY,
    "PetID" INTEGER NOT NULL REFERENCES "Pets"("ID") ON DELETE CASCADE,
    "ClinicName" VARCHAR(200) NOT NULL,
    "VetName" VARCHAR(100),
    "AppointmentDate" TIMESTAMP WITH TIME ZONE NOT NULL,
    "Reason" VARCHAR(500),
    "Status" VARCHAR(50) NOT NULL
);

CREATE INDEX "IX_Appointments_PetID" ON "Appointments"("PetID");

-- 5. DailyLogs
CREATE TABLE "DailyLogs" (
    "ID" SERIAL PRIMARY KEY,
    "PetID" INTEGER NOT NULL REFERENCES "Pets"("ID") ON DELETE CASCADE,
    "FoodAmount" NUMERIC(5,2) NOT NULL,
    "WaterIntake" NUMERIC(5,2) NOT NULL,
    "StoolStatus" VARCHAR(50),
    "UrineStatus" VARCHAR(50),
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX "IX_DailyLogs_PetID" ON "DailyLogs"("PetID");

-- 6. HealthData（注意欄位是 "Timestamp"，不是 RecordedAt）
CREATE TABLE "HealthData" (
    "ID" SERIAL PRIMARY KEY,
    "PetID" INTEGER NOT NULL REFERENCES "Pets"("ID") ON DELETE CASCADE,
    "Temperature" NUMERIC(4,1) NOT NULL,
    "HeartRate" INTEGER NOT NULL,
    "ActivityLevel" INTEGER NOT NULL,
    "SleepQuality" INTEGER NOT NULL,
    "Timestamp" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX "IX_HealthData_PetID" ON "HealthData"("PetID");

-- ================================================================
-- 初始化測試數據
-- ================================================================

-- admin 用戶（密碼: admin123）
INSERT INTO "Users" ("Username", "Email", "PasswordHash", "FullName", "CreatedAt", "UpdatedAt")
VALUES (
    'admin',
    'admin@pets.local',
    '$2a$11$PB9Yqt/R8lgq0N2G44n0fOCXh94NkQj5CfraOkTPHCSxs.Hntlluu',
    'System Administrator',
    NOW(), NOW()
);

-- 插入寵物資料（屬於 admin）
WITH admin_user AS (
    SELECT "ID" FROM "Users" WHERE "Username" = 'admin'
)
INSERT INTO "Pets" ("Name", "Species", "Birthday", "WeightGoal", "UserID")
VALUES
    ('小黑', '狗', '2021-06-15'::TIMESTAMP WITH TIME ZONE, 8.5, (SELECT "ID" FROM admin_user)),
    ('咪咪', '貓', '2020-03-20'::TIMESTAMP WITH TIME ZONE, 4.2, (SELECT "ID" FROM admin_user));

-- 插入初始健康數據（每隻寵物 5 筆）
INSERT INTO "HealthData" ("PetID", "Temperature", "HeartRate", "ActivityLevel", "SleepQuality", "Timestamp")
SELECT
    p."ID",
    (38.0 + random() * 1.5)::numeric(4,1),
    (70 + (random() * 40))::int,
    (50 + (random() * 50))::int,
    (40 + (random() * 40))::int,
    NOW() - ((gs.n * 15) || ' minutes')::interval
FROM "Pets" p
CROSS JOIN generate_series(1, 5) AS gs(n)
WHERE p."Name" = '小黑';

INSERT INTO "HealthData" ("PetID", "Temperature", "HeartRate", "ActivityLevel", "SleepQuality", "Timestamp")
SELECT
    p."ID",
    (37.8 + random() * 1.2)::numeric(4,1),
    (100 + (random() * 40))::int,
    (10 + (random() * 50))::int,
    (60 + (random() * 30))::int,
    NOW() - ((gs.n * 15) || ' minutes')::interval
FROM "Pets" p
CROSS JOIN generate_series(1, 5) AS gs(n)
WHERE p."Name" = '咪咪';

-- ================================================================
-- 驗證
-- ================================================================
SELECT '✅ 初始化完成' AS STATUS;
SELECT COUNT(*) AS "Users Count" FROM "Users";
SELECT COUNT(*) AS "Pets Count" FROM "Pets";
SELECT COUNT(*) AS "HealthData Count" FROM "HealthData";
SELECT "ID", "Name", "Species", "UserID" FROM "Pets";
