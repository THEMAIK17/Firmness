#  Firmness - Sales and Rental Management System

![.NET Version](https://img.shields.io/badge/.NET-8.0-purple) ![React](https://img.shields.io/badge/React-18-blue) ![Docker](https://img.shields.io/badge/Docker-Enabled-2496ED) ![Architecture](https://img.shields.io/badge/Architecture-Clean%20DDD-orange)

**Firmness** is a robust Fullstack solution designed for the comprehensive management of inventory, clients, and sales processes in the construction and machinery rental sector. The system implements a **distributed architecture** that separates business logic (Backend API), administration (Razor Pages), and client experience (Frontend React), all orchestrated via Docker containers.

---

## Table of Contents
1. [Architecture and Technologies](#-architecture-and-technologies)
2. [Key Features](#-key-features)
3. [ Security Note & Configuration (.env)](#-security-note--configuration-env)
4. [Installation and Deployment (Docker)](#-installation-and-deployment-docker)
5. [Access Credentials](#-access-credentials)
6. [Usage Guide](#-usage-guide)
7. [Testing & Quality](#-testing--quality)
8. [Author](#-author)

---

##  Architecture and Technologies

The project has been built following **Clean Architecture** and **Domain-Driven Design (DDD)** principles to ensure scalability, maintainability, and testability.

### Backend (.NET 8)
* **Firmeza.API:** Secure RESTful API with JWT authentication.
* **Firmeza.Web:** Server-side rendered administrative panel (Razor Pages).
* **Firmeza.Infraestructure:** Persistence implementation with Entity Framework Core and PostgreSQL.
* **Firmeza.Application:** Pure business logic, DTOs, Interfaces, and Services (Email, PDF, Excel).
* **Firmeza.Domain:** Core entities and business rules.

### Frontend (React 18)
* **Framework:** React with Vite.
* **State Management:** Context API for global session and shopping cart management.
* **Networking:** Axios with interceptors for automatic Token injection.
* **UI:** Bootstrap 5 for responsive design.

### Infrastructure (DevOps)
* **Docker Compose:** Orchestration of 4 services (Database, API, Web Admin, Client) + Test Container.

---

##  Key Features

* **Inventory Management (Admin):** Complete product CRUD.
* **Smart Mass Import (Admin):** Import of denormalized Excel files. The system validates, normalizes data, creates non-existent clients/products, and registers sales automatically.
* **Virtual Store (Client):** Public catalog, real-time shopping cart, and integrated checkout.
* **Document Automation:** Automatic generation of PDF receipts using **QuestPDF** after each purchase.
* **Notifications:** Transactional email sending (SMTP) with the receipt attached as a PDF.
* **Robust Security:** Identity system with differentiated roles (Administrator vs. Client) and JWT protection.

---

##  Security Note & Configuration (.env)

For the purpose of this **Technical Test**, I have made the deliberate decision to **include the `.env` file in the repository** (excluding it from `.gitignore`).

**Justification:**
I am fully aware that in a real production environment, versioning credentials and secrets is a **critical security vulnerability (CWE-798)**. However, to facilitate the review and deployment process for the evaluation team, I have pre-configured the environment.

**Benefit for the Reviewer:**
This allows executing the `docker compose up` command immediately after cloning the repository, **without the need to manually create configuration files** or set up database/email credentials. The system is "Plug & Play".

---

##  Installation and Deployment (Docker)

Thanks to Docker orchestration, there is no need to install .NET SDK, Node.js, or PostgreSQL on your local machine.

### Steps to run:

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/THEMAIK17/Firmness.git
    cd Firmness
    ```

2.  **Run the system:**
    Open a terminal in the solution root and run:
    ```bash
    docker compose up --build
    ```

3.  **Automatic Initialization Process:**
    * The system will build the images.
    * It will first execute the **Unit Tests**. If they pass, it proceeds.
    * It will spin up the Database and automatically execute **Migrations** and **Seeders** (Test Data).
    * *Note:* The first run may take a few minutes while downloading dependencies.

---

##  Access Credentials

The system starts with pre-loaded data to facilitate immediate testing.

### 1. Administrative Panel (Backend)
* **URL:** [http://localhost:8080](http://localhost:8080)
* **Admin User:** `admin@firmeza.com`
* **Password:** `AdminPassword123*`

### 2. Virtual Store (React Client)
* **URL:** [http://localhost:5173](http://localhost:5173)
* **Demo User:** `cliente@test.com`
* **Password:** `ClientPassword123!`
* *(You can also register a new user from the interface).*

### 3. API & Documentation
* **Swagger UI:** [http://localhost:5143/swagger](http://localhost:5143/swagger)
* **Database (External):** Port `5435`

---

##  Usage Guide

### Test Mass Import (Excel)
1.  Log in to the **Admin Panel**.
2.  Go to **"Import Excel"**.
3.  Upload an `.xlsx` file with sales data. The system will process and normalize the information.

### Test Purchase and PDF
1.  Log in to the **Virtual Store**.
2.  Add products and finalize the purchase.
3.  The system will send an email (if valid SMTP credentials are used).
4.  Go to **Admin Panel** -> **Ventas** (Sales) and click on **"Ver PDF"** (View PDF). The file will be generated and downloaded on demand.

---

##  Testing & Quality

The project includes a test execution pipeline integrated into the deployment.

* **Unit Testing:** `Firmness.Tests` project using **xUnit** and **Moq**.
* **Coverage:** Validates critical business services, such as the Excel import logic.
* **Deployment Policy:** The test container (`firmness_tests`) acts as a "Quality Gate". If tests fail, the application deployment stops automatically.

---
##  Note for Linux / Mac Users

If deploying the project in a Linux environment and you wish for PDF files to physically persist in the shared folder, you must grant write permissions to the volume:

```bash

sudo chmod -R 777 Firmness.Web/wwwroot

```
---

##  Author

**ARMANDO MAIKOL DURAN SALAS**
* **Email:** maikold774@gmail.com
* **Document ID:** 1002184305