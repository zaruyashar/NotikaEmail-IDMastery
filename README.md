# 🛡️ ASP.NET Core Identity & Security Masterclass (Course Project)

> **Project Status: Archived / Educational Artifact**
>
> This repository contains the source code from a comprehensive ASP.NET Core 10.0 course focused on backend security, identity management, and architecture. While I am aware that the user interface and overall frontend design require significant improvements, I have decided to archive this project "as-is" from the course curriculum. 
 
> My primary objective in this phase of my development journey was to deeply understand the underlying backend mechanics—specifically authentication, authorization, token management, and API integrations—rather than polishing the UI. Therefore, this repository serves as a backend learning milestone and will not receive further frontend updates.

## 💡 Key Learnings & Backend Features

Despite the unfinished frontend template, this project served as a heavy-lifting ground for implementing critical, real-world backend features:

* **Advanced Identity & Access Management:** Full implementation of ASP.NET Core Identity, including custom `AppUser` configurations, RoleManager, and UserManager.
* **Dual Authentication Systems:** Configured and managed both **JWT (JSON Web Tokens)** for secure API access and **Cookie Authentication** running in parallel within the same architecture.
* **Account Security & Recovery:** Real-time Email Verification with activation codes via SMTP.
  * Secure, token-based "Forgot Password" link generation and password reset flows.
* **AI-Powered Moderation:** Integrated the **Hugging Face API** to perform automated Toxic Comment Analysis for both English and Turkish inputs.
* **Authorization Policies:** Implemented deep Role-based, Claims-based, and Policy-based authorization rules.
* **Layered Architecture:** Built a monolithic application following the Model-View-Controller pattern, utilizing Entity Framework Core (Code First) for database operations.
* **Communication Modules:** Built an internal Inbox/Outbox messaging system for user-to-user communication.

## 🛠️ Tech Stack & Infrastructure

* **Framework:** ASP.NET Core 10.0
* **Architecture:** Monolithic MVC Pattern
* **Database:** MSSQL, Entity Framework Core (Code First)
* **Security:** ASP.NET Core Identity, JWT Bearer, Cookie Auth, OAuth concepts
* **Validation:** Fluent Validation with custom error handling (TR/EN)
* **Integrations:** SMTP (Gmail), Hugging Face API (NLP Text Classification), Summernote Editor

## 📸 Project Snapshots

Here is a look at the core functionalities implemented during the course:

### User Authentication & Role Management
<details>
<summary>Click to view Login & Role Management</summary>

![Login Screen]<img width="3071" height="1919" alt="login" src="https://github.com/user-attachments/assets/be02af83-88ef-4c93-ab07-4532a23e793c" />
*Custom Login and Registration interface.*

![Role List]<img width="3071" height="1919" alt="rbac" src="https://github.com/user-attachments/assets/2c239f7c-5684-49d7-8754-3541df46d5f7" />
*Dynamic Role creation and assignment management.*

</details>

### AI Content Moderation & Messaging
<details>
<summary>Click to view Toxic Comment Analysis & Inbox</summary>

![Comment Analysis]![4  hugging face](https://github.com/user-attachments/assets/5efdfb49-e6a7-4fcc-bbf5-dd3b089ebaa8)
*Real-time Turkish/English toxic comment detection using Hugging Face.*

![Messaging System]<img width="3071" height="1919" alt="inbox" src="https://github.com/user-attachments/assets/110278c9-3fb7-4d55-965e-ef3a40f07e93" />
*Internal user-to-user messaging dashboard.*

</details>

### Security & Account Recovery Flows
<details>
<summary>Click to view SMTP Implementations</summary>

![Email Activation]<img width="3070" height="1344" alt="activation" src="https://github.com/user-attachments/assets/13d28aa8-bf6e-43e3-849d-8a48604400eb" />
*Automated SMTP email for account activation.*

![Password Reset]<img width="3071" height="1347" alt="password reset" src="https://github.com/user-attachments/assets/46b686eb-7e38-4599-9375-7f5f3b1da78d" />
*Secure, tokenized password reset links sent via email.*

</details>
