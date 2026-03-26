# MakeSvg

A lightweight toolset for generating and manipulating **SVG graphics** using C# and Docker. This project provides a foundation for building scalable, scriptable SVG workflows that can be integrated into desktop or server-side applications.
Primarily used as possible value to src attributes of img tags in html.


# Running API
API can run via Visual Studio and rider by opening the solution or sln file. Dockerfile is in the <solution root>/SvgMaker directory.

## Links when running from Rider or Visual Studio
- Swagger [http://localhost:5001/swagger/index.html](http://localhost:5001/swagger/index.html)
- random [http://localhost:5001/random/](http://localhost:5001/random/)
- Text [http://localhost:5001/write/Hello%20World](http://localhost:5001/write/Hello%20World)

Replace with localhost:5001 with "127.0.0.1:8080" if running from Docker.

---

## 🚀 Features
- **SVG generation in C#** – create vector graphics programmatically.
- **Dockerized environment** – reproducible builds and easy deployment.
- **Extensible design** – add custom shapes, transformations, and export options.
- **Cross-platform** – works on Windows, Linux, and macOS via Docker.

---

## 📦 Installation

Clone the repository:

```bash
git clone https://github.com/nickleus-j/MakeSvg.git
cd MakeSvg
```

Build with Docker:

```bash
docker build -t makesvg .
```

Or open the solution directly in **Visual Studio**:

```bash
SvgMaker.sln
```

---

## 🎨 Example Outputs

Here are some simple SVGs generated with *MakeSvg*:

### Circle
```svg
<svg width="200" height="200">
  <circle cx="100" cy="100" r="50" fill="skyblue" stroke="black" />
</svg>
```

### Rectangle
```svg
<svg width="200" height="200">
  <rect x="20" y="20" width="160" height="100" fill="lightgreen" stroke="black" />
</svg>
```

### Logo-style Composition
```svg
<svg width="200" height="200">
  <circle cx="100" cy="60" r="40" fill="orange" />
  <rect x="60" y="100" width="80" height="60" fill="purple" />
  <text x="100" y="190" font-size="20" text-anchor="middle" fill="black">MakeSvg</text>
</svg>
```

---

## 🌐 Demo Showcase

For a live demonstration of endpoints and rendering, open **`index.html`** in your browser with the Web project running.
Prefered method to see the examples is running the web project (SvgMaker) and browse http://localhost:5001/.
This page showcases how the API responds and displays SVG outputs interactively.

---


## 📜 License
This project is licensed under the MIT License – see the LICENSE file for details.

---
