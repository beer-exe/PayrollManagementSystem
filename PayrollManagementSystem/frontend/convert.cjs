const fs = require('fs');
const base64 = fs.readFileSync('roboto.ttf', 'base64');
fs.writeFileSync('e:/LuanVan/Source/PayrollManagementSystem/frontend/src/assets/fonts/Roboto-Regular.ts', 'export const RobotoRegularBase64 = "' + base64 + '";');
