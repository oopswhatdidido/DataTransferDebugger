# DataTransferDebugger

A Windows Presentation Foundation (WPF) application that allows you to **inspect and debug data objects** transferred via **drag & drop** or **clipboard** operations.  
It’s designed for developers who need to explore and understand how Windows data transfer formats work under the hood.

---

## ✨ Features

- 🖱️ **Drag & Drop Analysis** — Drop files, folders, images, or text onto the window to inspect all available formats and data content.
- 📋 **Clipboard Inspection** — Analyze the current contents of your clipboard (text, images, HTML, RTF, file lists, etc.).
- 📑 **Multi-format Detection** — Automatically lists and decodes all available data formats.
- 🖼️ **Image Metadata Extraction** — Displays resolution, DPI, and pixel format for dropped or copied images.
- 🗂️ **File & Directory Insights** — Shows size, last modified time, and counts of contained files/directories.
- 📜 **Stream and Byte Array Analysis** — Displays stream properties and previews raw bytes.
- 💾 **Save Results** — Export analysis results to a `.txt` file.
- 📎 **Copy Results** — Copy the full analysis report to clipboard.
- 🧹 **Clear Results** — Reset the display for new analysis.

---

## 🧰 Technology Stack

- **Language:** C#
- **Framework:** .NET 8.0 (WPF)
- **UI:** XAML
- **OS Compatibility:** Windows 10/11

---

## 🚀 Usage

1. **Run the application.**
2. You can:
   - **Drag and drop** any file, folder, or object into the main window.
   - Or click **“Analyze Clipboard”** to inspect the current clipboard data.
3. The results will appear in the main text area, showing:
   - Available data formats
   - Type information
   - Content preview (text, HTML, image, etc.)
4. Use:
   - **Copy Results** → Copies output to clipboard.
   - **Save Results** → Saves output to a `.txt` file.
   - **Clear Results** → Clears the output area.

---

## 🧪 Example Uses

- Inspect what formats Windows Explorer provides when you drag files.
- Debug clipboard issues between applications.
- Examine how HTML, RTF, or bitmap formats are represented.
- View structure of custom data formats or shell objects.

---

## 🖼️ UI Overview

- **Top Buttons:**
  - `Analyze Clipboard` — triggers analysis of current clipboard contents.
  - `Clear Results` — clears output window.
  - `Copy Results` — copies all analysis text.
  - `Save Results` — saves current analysis to a file.

- **Main Output Area:**
  - Scrollable text box showing detailed analysis results.

- **Status Bar:**
  - Displays status messages such as “Ready”, “Results copied”, or “Saved successfully”.

---

## ⚙️ Building the Project

1. Open the solution in **Visual Studio 2022** or later.
2. Ensure you have **.NET 8 SDK** installed.
3. Build and run the project:
   ```bash
   dotnet build
   dotnet run
