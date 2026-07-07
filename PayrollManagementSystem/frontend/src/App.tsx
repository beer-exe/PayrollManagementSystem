import {
  BrowserRouter,
  Routes,
  Route,
  Navigate,
  Outlet,
} from "react-router-dom";
import { useAuthStore } from "@/store/useAuthStore";
import { AuthLayout } from "@/layouts/AuthLayout";
import { MainLayout } from "@/layouts/MainLayout";
import { LoginForm } from "@/features/auth/components/LoginForm";
import { UserProfile } from "@/features/profile/components/UserProfile";
import { EmployeeManagement } from "@/features/employees/components/EmployeeManagement";
import { UserManagement } from "@/features/users/components/UserManagement";
import { DepartmentManagement } from "@/features/departments/components/DepartmentManagement";
import { PositionManagement } from "@/features/positions/components/PositionManagement";
import { NgachLuongManagement } from "@/features/jobGrades/components/NgachLuongManagement";
import { MucQuyDoiManagement } from "@/features/competencyP2/components/MucQuyDoiManagement";
import { KyDanhGiaManagement } from "@/features/competencyP2/components/KyDanhGiaManagement";
import { KhungNangLucManagement } from "@/features/competencyP2/components/KhungNangLucManagement";
import { TuDanhGia } from "@/features/competencyP2/components/TuDanhGia";
import { TuDanhGiaForm } from "@/features/competencyP2/components/TuDanhGiaForm";
import { DuyetDanhGia } from "@/features/competencyP2/components/DuyetDanhGia";
import { DuyetDanhGiaForm } from "@/features/competencyP2/components/DuyetDanhGiaForm";

const ProtectedRoute = ({ allowedRoles }: { allowedRoles: string[] }) => {
  const { isAuthenticated, user } = useAuthStore();

  if (!isAuthenticated) return <Navigate to="/login" replace />;

  if (user?.role && !allowedRoles.includes(user.role)) {
    return <Navigate to="/dashboard" replace />;
  }

  return <Outlet />;
};

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Navigate to="/dashboard" replace />} />

        <Route
          path="/login"
          element={
            <AuthLayout>
              <LoginForm />
            </AuthLayout>
          }
        />

        <Route path="/dashboard" element={<MainLayout />}>
          <Route index element={<Navigate to="ho-so" replace />} />

          <Route path="ho-so" element={<UserProfile />} />

          <Route element={<ProtectedRoute allowedRoles={["Admin"]} />}>
            <Route path="tai-khoan" element={<UserManagement />} />
          </Route>

          <Route element={<ProtectedRoute allowedRoles={["HR"]} />}>
            <Route path="nhan-vien" element={<EmployeeManagement />} />
            <Route path="phong-ban" element={<DepartmentManagement />} />
            <Route path="ngach-luong" element={<NgachLuongManagement />} />
            <Route path="chuc-vu" element={<PositionManagement />} />
          </Route>

          {/* Đánh giá năng lực P2 */}
          <Route element={<ProtectedRoute allowedRoles={["HR"]} />}>
            <Route path="danh-gia/khung-nang-luc" element={<KhungNangLucManagement />} />
            <Route path="danh-gia/cau-hinh" element={<MucQuyDoiManagement />} />
          </Route>

          <Route element={<ProtectedRoute allowedRoles={["Admin", "HR"]} />}>
            <Route path="danh-gia/ky-danh-gia" element={<KyDanhGiaManagement />} />
          </Route>

          <Route element={<ProtectedRoute allowedRoles={["Admin", "HR", "Employee"]} />}>
            <Route path="danh-gia/tu-danh-gia" element={<TuDanhGia />} />
            <Route path="danh-gia/tu-danh-gia/:id" element={<TuDanhGiaForm />} />
            <Route path="danh-gia/duyet-danh-gia" element={<DuyetDanhGia />} />
            <Route path="danh-gia/duyet-danh-gia/:id" element={<DuyetDanhGiaForm />} />
          </Route>
        </Route>

        <Route path="*" element={<Navigate to="/dashboard" replace />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
