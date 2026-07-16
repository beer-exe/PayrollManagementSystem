
const fs = require("fs");
const path = require("path");

const directoryPath = path.join(__dirname, "src");

const replacements = [
  // Backgrounds
  { regex: /background-color:\s*(#ffffff|#fff)\b/gi, replace: "background-color: var(--bg-surface)" },
  { regex: /background:\s*(#ffffff|#fff)\b/gi, replace: "background: var(--bg-surface)" },
  { regex: /background-color:\s*#f3f4f6\b/gi, replace: "background-color: var(--bg-main)" },
  { regex: /background:\s*#f3f4f6\b/gi, replace: "background: var(--bg-main)" },
  { regex: /background-color:\s*#f9fafb\b/gi, replace: "background-color: var(--bg-hover)" },
  { regex: /background:\s*#f9fafb\b/gi, replace: "background: var(--bg-hover)" },
  { regex: /background-color:\s*#e5e7eb\b/gi, replace: "background-color: var(--border-color)" },
  
  // Texts
  { regex: /color:\s*#1f2937\b/gi, replace: "color: var(--text-primary)" },
  { regex: /color:\s*#333333\b/gi, replace: "color: var(--text-primary)" },
  { regex: /color:\s*#333\b/gi, replace: "color: var(--text-primary)" },
  { regex: /color:\s*#111827\b/gi, replace: "color: var(--text-primary)" },
  { regex: /color:\s*#374151\b/gi, replace: "color: var(--text-primary)" },
  { regex: /color:\s*#4b5563\b/gi, replace: "color: var(--text-secondary)" },
  { regex: /color:\s*#6b7280\b/gi, replace: "color: var(--text-secondary)" },
  { regex: /color:\s*#9ca3af\b/gi, replace: "color: var(--text-muted)" },
  
  // Borders
  { regex: /border-color:\s*#e5e7eb\b/gi, replace: "border-color: var(--border-color)" },
  { regex: /border:\s*1px\s+solid\s+#e5e7eb\b/gi, replace: "border: 1px solid var(--border-color)" },
  { regex: /border-bottom:\s*1px\s+solid\s+#e5e7eb\b/gi, replace: "border-bottom: 1px solid var(--border-color)" },
  { regex: /border-top:\s*1px\s+solid\s+#e5e7eb\b/gi, replace: "border-top: 1px solid var(--border-color)" },
  { regex: /border:\s*1px\s+solid\s+#d1d5db\b/gi, replace: "border: 1px solid var(--border-hover)" },
  { regex: /border-color:\s*#d1d5db\b/gi, replace: "border-color: var(--border-hover)" },

  // Primary colors
  { regex: /color:\s*#7c3aed\b/gi, replace: "color: var(--primary)" },
  { regex: /background-color:\s*#7c3aed\b/gi, replace: "background-color: var(--primary)" },
  { regex: /border-color:\s*#7c3aed\b/gi, replace: "border-color: var(--primary)" },
  { regex: /border:\s*1px\s+solid\s+#7c3aed\b/gi, replace: "border: 1px solid var(--primary)" },
  { regex: /border:\s*2px\s+solid\s+#7c3aed\b/gi, replace: "border: 2px solid var(--primary)" },
  { regex: /background:\s*rgba\(124,\s*58,\s*237,\s*0\.1\)\b/gi, replace: "background: var(--primary-light)" },
  { regex: /background-color:\s*#ede9fe\b/gi, replace: "background-color: var(--primary-light)" },
  { regex: /background:\s*#ede9fe\b/gi, replace: "background: var(--primary-light)" },
  
  // States (Success, Danger, Warning)
  { regex: /color:\s*#10b981\b/gi, replace: "color: var(--success)" },
  { regex: /background-color:\s*#d1fae5\b/gi, replace: "background-color: var(--success-bg)" },
  { regex: /color:\s*#065f46\b/gi, replace: "color: var(--success-text)" },
  
  { regex: /color:\s*#ef4444\b/gi, replace: "color: var(--danger)" },
  { regex: /background-color:\s*#fee2e2\b/gi, replace: "background-color: var(--danger-bg)" },
  { regex: /color:\s*#991b1b\b/gi, replace: "color: var(--danger-text)" },
  
  { regex: /color:\s*#f59e0b\b/gi, replace: "color: var(--warning)" },
  { regex: /background-color:\s*#fef3c7\b/gi, replace: "background-color: var(--warning-bg)" },
  { regex: /color:\s*#92400e\b/gi, replace: "color: var(--warning-text)" },
  
  // Shadows (simple replacement for common ones)
  { regex: /box-shadow:\s*0\s+4px\s+6px\s+-1px\s+rgba\(0,\s*0,\s*0,\s*0\.1\),\s*0\s+2px\s+4px\s+-1px\s+rgba\(0,\s*0,\s*0,\s*0\.06\)/gi, replace: "box-shadow: var(--shadow-md)" },
  { regex: /box-shadow:\s*0\s+1px\s+3px\s+rgba\(0,\s*0,\s*0,\s*0\.1\)/gi, replace: "box-shadow: var(--shadow-sm)" }
];

function processDirectory(dir) {
  fs.readdirSync(dir).forEach(file => {
    const fullPath = path.join(dir, file);
    if (fs.statSync(fullPath).isDirectory()) {
      processDirectory(fullPath);
    } else if (fullPath.endsWith(".css") && !fullPath.endsWith("index.css")) {
      let content = fs.readFileSync(fullPath, "utf8");
      let originalContent = content;
      replacements.forEach(rule => {
        content = content.replace(rule.regex, rule.replace);
      });
      if (content !== originalContent) {
        fs.writeFileSync(fullPath, content);
        console.log(`Updated: ${fullPath}`);
      }
    }
  });
}

processDirectory(directoryPath);

