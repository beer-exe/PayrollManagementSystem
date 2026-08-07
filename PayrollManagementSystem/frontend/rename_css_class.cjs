const fs = require('fs');
const path = require('path');

const directoryPath = 'e:/LuanVan/Source/PayrollManagementSystem/frontend/src/features/systemLogs/components';

const filesToUpdate = [
    'SystemLogViewer.tsx',
    'LogDetailModal.tsx',
    'RealtimePanel.tsx'
];

filesToUpdate.forEach(file => {
    const fullPath = path.join(directoryPath, file);
    if (fs.existsSync(fullPath)) {
        let content = fs.readFileSync(fullPath, 'utf8');
        
        // Replace 'sl-' with 'syslog-'
        // Only replace where it's used as a class name or in string interpolation:
        // Examples: className="sl-page", 'sl-content ', "sl-realtime-entry"
        content = content.replace(/className=(["'`])(.*?)sl-(.*?)(["'`])/g, (match, p1, p2, p3, p4) => {
            return `className=${p1}${p2}syslog-${p3}${p4}`;
        });

        // Some might be dynamic: className={`sl-content ${...}`}
        content = content.replace(/sl-/g, 'syslog-');

        fs.writeFileSync(fullPath, content, 'utf8');
        console.log(`Updated ${file}`);
    } else {
        console.log(`File not found: ${file}`);
    }
});
