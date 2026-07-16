const fs = require('fs');
const https = require('https');

https.get('https://raw.githubusercontent.com/google/fonts/main/ofl/roboto/Roboto-Regular.ttf', (res) => {
  const chunks = [];
  res.on('data', (c) => chunks.push(c));
  res.on('end', () => {
    const buffer = Buffer.concat(chunks);
    const base64 = buffer.toString('base64');
    fs.mkdirSync('e:/LuanVan/Source/PayrollManagementSystem/frontend/src/assets/fonts', { recursive: true });
    fs.writeFileSync('e:/LuanVan/Source/PayrollManagementSystem/frontend/src/assets/fonts/Roboto-Regular.ts', 'export const RobotoRegularBase64 = "' + base64 + '";');
    console.log('Done');
  });
});
