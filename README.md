# Automated Transcript System (ATS)
### 🏛 Specialized Academic Management for NUML University

[![Language](https://img.shields.io/badge/Language-C%23-blue.svg)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Framework](https://img.shields.io/badge/Framework-ASP.NET%20MVC-purple.svg)](https://dotnet.microsoft.com/en-us/apps/aspnet/mvc)
[![Database](https://img.shields.io/badge/Database-MS%20SQL%20Server-red.svg)](https://www.microsoft.com/en-us/sql-server/)
[![Design](https://img.shields.io/badge/Architecture-4%2B1%20View%20Model-orange.svg)](#system-architecture)

## 📌 Project Overview
The **Automated Transcript System (ATS)** is a professional-grade web application developed specifically for the National University of Modern Languages (NUML). It revolutionizes academic record management for BS programs by replacing manual evaluations with an automated, secure, and fault-tolerant digital framework.

## 🏗 System Architecture
This project follows the **(4+1) View Model of Software Architecture**, ensuring a clear separation of concerns:
* **Logical View:** Managed via Entity Framework (EF) Code-First approach.
* **Process View:** Implementation of complex GPA/CGPA calculation algorithms and transcript generation workflows.
* **Development View:** Built on the ASP.NET MVC pattern for scalable maintenance.
* **Physical View:** Multi-tier deployment involving a web server and a centralized MS SQL Server.



[Image of the MVC (Model-View-Controller) architecture pattern]


## 🚀 Key Features
* **Role-Based Access Control (RBAC):** Distinct modules for Admin, Coordinator, Exam Branch, and Students.
* **Algorithmic GPA Calculation:** Real-time processing of semester results with automated CGPA updates.
* **Document Generation:** Integrated **Rotativa** library for high-fidelity PDF generation of official transcripts.
* **Data Integrity:** Validated marks input and automated scheme-of-study assignment.

## 🛠 Tools & Technologies
* **Backend:** C# / ASP.NET MVC
* **Frontend:** HTML5, CSS3, jQuery, Bootstrap
* **Database:** MS SQL Server (Entity Framework)
* **Testing:** Selenium WebDriver (Automated Functional Testing)
* **Libraries:** Rotativa (PDF), Excel Data Reader (Bulk Uploads)

## ⚙️ Installation & Setup
To run this project locally, follow these steps:

### 1. Prerequisites
* Visual Studio 2022 (with ASP.NET and Web Development workload)
* SQL Server Management Studio (SSMS)
* .NET Framework 4.7.2 or higher

### 2. Database Configuration
1.  Open **SQL Server Management Studio**.
2.  Create a new database named `ATS_NUML`.
3.  Locate the `web.config` file in the project root and update the connection string:
    ```xml
    <add name="DefaultConnection" connectionString="Data Source=YOUR_SERVER_NAME;Initial Catalog=ATS_NUML;Integrated Security=True" providerName="System.Data.SqlClient" />
    ```

### 3. Running the Application
1.  Open the `.sln` file in **Visual Studio**.
2.  Open the **Package Manager Console** and run:
    ```powershell
    Update-Database
    ```
    *(This will automatically create all tables via Entity Framework Migrations).*
3.  Press `F5` or click **Start** to launch the application in your browser.

## 🧬 Engineering Methodology
This system was developed using a **Waterfall Iterative Approach**. Each phase—from requirement engineering to deployment—was documented following standard SDLC practices.

### Quality Assurance (QA)
* **Black Box Testing:** Validating system outputs against functional requirements.
* **Selenium Testing:** Automated scripts were used to verify the "Search Student" and "Transcript Generation" modules.

## 📄 Documentation
* **[Full Technical Report](https://github.com/abdulrafay45/Automated-Transcript-System/blob/main/Final%20Project%20Report%20Automated%20Transcript%20System%20Final.pdf)** - Includes UML Class Diagrams, Sequence Diagrams, and ERD.
