import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthLayout } from '@/layouts/AuthLayout';
import { LoginForm } from '@/features/auth/components/LoginForm';
import { MainLayout } from '@/layouts/MainLayout';
import { UserProfile } from '@/features/profile/components/UserProfile';
import { EmployeeManagement } from '@/features/employees/components/EmployeeManagement';
import { UserManagement } from '@/features/users/components/UserManagement';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route 
          path="/login" 
          element = {
            <AuthLayout>
              <LoginForm />
            </AuthLayout>
          } 
        />

        <Route path="/dashboard" element={<MainLayout />}>
          <Route index element={<div className="p-4 bg-white rounded-lg shadow-sm"><h2>Chào mừng bạn đến với trang Tổng Quan Hệ Thống</h2></div>} />
          
          <Route path="nhan-vien" element={<EmployeeManagement />} />
          <Route path="phong-ban" element={<div className="p-4 bg-white rounded-lg shadow-sm"><h2>Quản lý phòng ban (Đang phát triển)</h2></div>} />
          <Route path="chuc-vu" element={<div className="p-4 bg-white rounded-lg shadow-sm"><h2>Quan lý chức vụ (Đang phát triển)</h2></div>} />
          <Route path="ho-so" element={<UserProfile />} />
          <Route path="tai-khoan" element={<UserManagement />} />
        </Route>

        <Route path="*" element={<Navigate to="/login" replace />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;