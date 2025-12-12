using System;
using Microsoft.Data.Sqlite;
using System.IO;

namespace Project.Data
{
 public static class Database
 {
 private static string _dbPath = "app.db";
 public static string ConnectionString => $"Data Source={_dbPath}";

 public static void Initialize(string dbPath)
 {
 _dbPath = dbPath;
 if (!File.Exists(_dbPath))
 {
 // Microsoft.Data.Sqlite will create file when opening connection if not exists
 // ensure directory
 var dir = Path.GetDirectoryName(Path.GetFullPath(_dbPath));
 if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
 }

 using var conn = new SqliteConnection(ConnectionString);
 conn.Open();
 using var cmd = conn.CreateCommand();

 // Users
 cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Users (
 Id INTEGER PRIMARY KEY AUTOINCREMENT,
 Username TEXT NOT NULL UNIQUE,
 PasswordHash TEXT NOT NULL,
 Role TEXT NOT NULL
 );";
 cmd.ExecuteNonQuery();

 // Categories
 cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Categories (
 Id INTEGER PRIMARY KEY AUTOINCREMENT,
 Name TEXT NOT NULL,
 Description TEXT
 );";
 cmd.ExecuteNonQuery();

 // Suppliers
 cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Suppliers (
 Id INTEGER PRIMARY KEY AUTOINCREMENT,
 Name TEXT NOT NULL,
 ContactName TEXT,
 Phone TEXT,
 Email TEXT,
 Address TEXT
 );";
 cmd.ExecuteNonQuery();

 // Products
 cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Products (
 Id INTEGER PRIMARY KEY AUTOINCREMENT,
 Name TEXT NOT NULL,
 CategoryId INTEGER,
 SupplierId INTEGER,
 Quantity INTEGER NOT NULL DEFAULT 0,
 MinQuantity INTEGER NOT NULL DEFAULT 0,
 PurchasePrice REAL,
 SellPrice REAL,
 CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP,
 FOREIGN KEY(CategoryId) REFERENCES Categories(Id),
 FOREIGN KEY(SupplierId) REFERENCES Suppliers(Id)
 );";
 cmd.ExecuteNonQuery();

 // StockMovements
 cmd.CommandText = @"CREATE TABLE IF NOT EXISTS StockMovements (
 Id INTEGER PRIMARY KEY AUTOINCREMENT,
 ProductId INTEGER NOT NULL,
 MovementType TEXT NOT NULL,
 Quantity INTEGER NOT NULL,
 MovementDate TEXT NOT NULL,
 UserId INTEGER,
 Notes TEXT,
 FOREIGN KEY(ProductId) REFERENCES Products(Id),
 FOREIGN KEY(UserId) REFERENCES Users(Id)
 );";
 cmd.ExecuteNonQuery();

 // ActivityLogs
 cmd.CommandText = @"CREATE TABLE IF NOT EXISTS ActivityLogs (
 Id INTEGER PRIMARY KEY AUTOINCREMENT,
 UserId INTEGER,
 Action TEXT NOT NULL,
 Entity TEXT NOT NULL,
 EntityId INTEGER,
 Details TEXT,
 CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP,
 FOREIGN KEY(UserId) REFERENCES Users(Id)
 );";
 cmd.ExecuteNonQuery();
 }
 }
}
