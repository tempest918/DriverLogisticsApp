# Driver Logistics App

A comprehensive, offline-first mobile application for Android, built with .NET MAUI, designed to help truck drivers and owner-operators manage their loads, track expenses, and generate professional invoices and settlement reports.

---

## ✨ Features

This application provides a full suite of tools to digitize and streamline the administrative tasks of a professional driver:

* **Load Management:**
    * Full CRUD (Create, Read, Update, Delete) functionality for freight loads.
    * Detailed address management for shippers and consignees.
    * Intuitive status workflow: `Planned` -> `In Progress` -> `Completed` -> `Invoiced`.

* **Expense Tracking:**
    * Log expenses (Fuel, Tolls, Maintenance, etc.) for each load.
    * Attach receipt photos using the device camera.
    * View and manage expenses on a per-load basis.

* **Reporting & Invoicing:**
    * Generate professional, multi-item **PDF invoices** for individual loads.
    * Create detailed **Driver Settlement Reports** for any date range, with grouped earnings and deductions.
    * Export and share all reports as PDF files.

* **Dashboard & KPIs:**
    * A main dashboard that displays key performance indicators (KPIs) for the current month, including **Actual Revenue**, **Potential Revenue**, and **Net Profit**.

* **Data Management:**
    * **Import and Export** all application data (loads and expenses) using JSON files for easy backup and restore.

* **Security:**
    * Optional **PIN lock screen** on app startup to secure sensitive financial data.
    * PIN is stored securely using the device's native keychain/keystore.

---

## 🛠️ Technology Stack

This project is built using a modern, cross-platform technology stack and follows industry-standard architectural patterns.

* **Framework:** .NET MAUI
* **Language:** C#
* **Architecture:** Model-View-ViewModel (MVVM) with the CommunityToolkit.Mvvm library.
* **Database:** Local SQLite database for full offline functionality.
* **PDF Generation:** iText 7 library.
* **Testing:** MSTest and Moq for comprehensive unit testing of ViewModel logic.

---

## 🚀 Getting Started

To get a local copy up and running, follow these simple steps.

### Prerequisites

* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or newer.
* Visual Studio 2022 with the ".NET Multi-platform App UI development" workload installed.
* An Android emulator (or a physical Android device) configured for debugging.

### Installation

1.  Clone the repository:
    ```sh
    git clone https://github.com/tempest918/DriverLogisticsApp.git
    ```
2.  Open `DriverLogisticsApp.sln` in Visual Studio.
3.  Restore the NuGet packages by right-clicking the solution and selecting "Restore NuGet Packages".
4.  Build the solution (**Build > Build Solution**).
5.  Select your target Android emulator from the debug dropdown and press the "Run" button.
