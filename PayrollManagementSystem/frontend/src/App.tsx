import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthLayout } from '@/layouts/AuthLayout';
import { LoginForm } from '@/features/auth/components/LoginForm';
import { MainLayout } from '@/layouts/MainLayout';
import { UserProfile } from '@/features/profile/components/UserProfile';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        {/* LƯỒNG ĐĂNG NHẬP (AUTH FLOW) */}
        <Route 
          path="/login" 
          element = {
            <AuthLayout>
              <LoginForm />
            </AuthLayout>
          } 
        />

        {/* LƯỒNG GIAO DIỆN CHÍNH (DASHBOARD FLOW) */}
        {/* MainLayout đóng vai trò làm khung bọc (Layout Wrapper) */}
        <Route path="/dashboard" element={<MainLayout />}>
          {/* Route mặc định khi vào /dashboard */}
          <Route index element={<div className="p-4 bg-white rounded-lg shadow-sm"><h2>Chào mừng bạn đến với trang Tổng Quan Hệ Thống</h2></div>} />
          
          {/* Các Domain Modules con sẽ được render vào vị trí của <Outlet /> trong MainLayout */}
          <Route path="nhan-vien" element={<div className="p-4 bg-white rounded-lg shadow-sm"><h2>TQuản lý nhân viên (Đang phát triển)</h2></div>} />
          <Route path="phong-ban" element={<div className="p-4 bg-white rounded-lg shadow-sm"><h2>Quản lý phòng ban (Đang phát triển)</h2></div>} />
          <Route path="chuc-vu" element={<div className="p-4 bg-white rounded-lg shadow-sm"><h2>Quan lý chức vụ (Đang phát triển)</h2></div>} />
          <Route path="ho-so" element={<UserProfile />} />
        </Route>

        {/* Điểu hướng mặc định: Nếu gõ sai URL hoặc vào trang chủ '/', tự động đưa về /login */}
        <Route path="*" element={<Navigate to="/login" replace />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;