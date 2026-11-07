-- ============================================================
-- Employee Management System - PostgreSQL Database Schema
-- ============================================================
-- Database: shahdb  (create manually or use the command below)
-- Compatible with: PostgreSQL 13+
-- ============================================================

-- 1. Create the database (run as superuser / pgAdmin)
--    Uncomment the line below if you want to create the DB here:
-- CREATE DATABASE shahdb;

-- Connect to shahdb before running the rest:
-- \c shahdb

-- ============================================================
-- 2. account table  (stores employee / admin accounts)
-- ============================================================
CREATE TABLE IF NOT EXISTS account (
    id               SERIAL PRIMARY KEY,
    fname            VARCHAR(100)   NOT NULL,
    lname            VARCHAR(100)   NOT NULL,
    gender           VARCHAR(10),
    marrige_status   VARCHAR(20),
    dob              DATE,
    email            VARCHAR(150),
    phno             VARCHAR(20),
    password         VARCHAR(255)   NOT NULL,
    address          VARCHAR(300),
    username         VARCHAR(100)   UNIQUE NOT NULL,
    designation      VARCHAR(100)   DEFAULT 'employee',
    status           VARCHAR(50)    DEFAULT 'contract',
    salary           NUMERIC(12, 2) DEFAULT 50000.00
);

-- ============================================================
-- 3. attendance table  (daily attendance records)
-- ============================================================
CREATE TABLE IF NOT EXISTS attendance (
    attendance_id  SERIAL PRIMARY KEY,
    id             INTEGER        NOT NULL REFERENCES account(id) ON DELETE CASCADE,
    attendancedate DATE           NOT NULL,
    status         VARCHAR(20)    NOT NULL DEFAULT 'Absent',
    CONSTRAINT uq_attendance UNIQUE (id, attendancedate)
);

-- ============================================================
-- 4. Indexes for performance
-- ============================================================
CREATE INDEX IF NOT EXISTS idx_attendance_date ON attendance (attendancedate);
CREATE INDEX IF NOT EXISTS idx_attendance_emp  ON attendance (id);
CREATE INDEX IF NOT EXISTS idx_account_username ON account (username);

-- ============================================================
-- 5. Seed data – default Admin account
--    username: admin   password: admin123
-- ============================================================
INSERT INTO account (fname, lname, gender, marrige_status, dob, email, phno,
                     password, address, username, designation, status, salary)
VALUES ('System', 'Admin', 'Male', 'Single', '1990-01-01',
        'admin@company.com', '03000000000',
        'admin123', 'Head Office, Lahore, Punjab, Pakistan', 'admin',
        'CEO', 'permanent', 100000.00)
ON CONFLICT (username) DO NOTHING;

-- ============================================================
-- End of schema
-- ============================================================
