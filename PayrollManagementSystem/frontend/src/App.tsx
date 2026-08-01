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
import { WorkScheduleManagement } from "@/features/workSchedule/components/WorkScheduleManagement";
import { WorkShiftManagement } from "@/features/workShifts/components/WorkShiftManagement";
import { ShiftAssignment } from "@/features/shiftAssignment/components/ShiftAssignment";
import { ChamCongManagement } from '@/features/chamCong/components/ChamCongManagement';
import { DonNghiManagement } from '@/features/donNghi/components/DonNghiManagement';
import { MyDonNghiPortal } from '@/features/donNghi/components/MyDonNghiPortal';
import PayrollManagement from '@/features/payroll/components/PayrollManagement';
import { KhoanKhauTruManagement } from '@/features/khoanKhauTru/components/KhoanKhauTruManagement';
import { ThueTncnManagement } from '@/features/thueTncn/components/ThueTncnManagement';


const ProtectedRoute = ({ allowedRoles, requireManager }: { allowedRoles: string[], requireManager?: boolean }) => {
  const { isAuthenticated, user } = useAuthStore();

  if (!isAuthenticated) return <Navigate to="/login" replace />;

  if (user?.role && !allowedRoles.includes(user.role)) {
    return <Navigate to="/profile" replace />;
  }

  if (requireManager && !user?.hasDirectReports) {
    return <Navigate to="/profile" replace />;
  }

  return <Outlet />;
};

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Navigate to="/profile" replace />} />

        <Route
          path="/login"
          element={
            <AuthLayout>
              <LoginForm />
            </AuthLayout>
          }
        />

        {/* Use a pathless route for MainLayout to wrap all protected routes */}
        <Route element={<MainLayout />}>
          
          {/* 1. Personal Group */}
          <Route path="/profile" element={<UserProfile />} />

          {/* 2. System Administration Group */}
          <Route element={<ProtectedRoute allowedRoles={["Admin"]} />}>
            <Route path="/admin/tai-khoan" element={<UserManagement />} />
          </Route>

          {/* 3. Human Resources Group */}
          <Route element={<ProtectedRoute allowedRoles={["HR"]} />}>
            <Route path="/hr/nhan-vien" element={<EmployeeManagement />} />
            <Route path="/hr/phong-ban" element={<DepartmentManagement />} />
            <Route path="/hr/ngach-luong" element={<NgachLuongManagement />} />
            <Route path="/hr/chuc-vu" element={<PositionManagement />} />
            <Route path="/hr/bang-luong" element={<PayrollManagement />} />
            <Route path="/hr/khoan-khau-tru" element={<KhoanKhauTruManagement />} />
            <Route path="/hr/thue-tncn" element={<ThueTncnManagement />} />

            
            <Route path="/time/ca-lam-viec" element={<WorkShiftManagement />} />
            <Route path="/time/phan-cong-ca" element={<ShiftAssignment />} />
            <Route path="/time/cham-cong" element={<ChamCongManagement />} />
            <Route path="/time/lich-lam-viec" element={<WorkScheduleManagement />} />
          </Route>

          {/* 4. Time & Attendance Group */}

          <Route element={<ProtectedRoute allowedRoles={["HR"]} />}>
            <Route path="/time/don-nghi" element={<DonNghiManagement />} />
          </Route>

          <Route element={<ProtectedRoute allowedRoles={["Admin", "HR", "Employee"]} />}>
            <Route path="/me/don-nghi" element={<MyDonNghiPortal />} />
          </Route>

          {/* Đánh giá năng lực P2 */}
          <Route element={<ProtectedRoute allowedRoles={["HR"]} />}>
            <Route path="/performance/khung-nang-luc" element={<KhungNangLucManagement />} />
            <Route path="/performance/cau-hinh" element={<MucQuyDoiManagement />} />
          </Route>

          <Route element={<ProtectedRoute allowedRoles={["HR"]} />}>
            <Route path="/performance/ky-danh-gia" element={<KyDanhGiaManagement />} />
          </Route>

          <Route element={<ProtectedRoute allowedRoles={["Admin", "HR", "Employee"]} />}>
            <Route path="/performance/tu-danh-gia" element={<TuDanhGia />} />
            <Route path="/performance/tu-danh-gia/:id" element={<TuDanhGiaForm />} />
          </Route>
          
          <Route element={<ProtectedRoute allowedRoles={["Admin", "HR", "Employee"]} requireManager={true} />}>
            <Route path="/performance/duyet-danh-gia" element={<DuyetDanhGia />} />
            <Route path="/performance/duyet-danh-gia/:id" element={<DuyetDanhGiaForm />} />
          </Route>
        </Route>

        <Route path="*" element={<Navigate to="/profile" replace />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
