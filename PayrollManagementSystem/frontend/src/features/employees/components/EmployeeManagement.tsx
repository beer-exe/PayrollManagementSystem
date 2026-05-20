import React, { useState, useEffect } from 'react';
import { EmpTable } from './EmpTable';
import { ColumnSetupDrawer } from './ColumnSetupDrawer';
import { EmployeeDetailPanel } from './EmployeeDetailPanel';
import { UserProfileDetail } from '@/types/profile.types';
import { employeeApi } from '../api/employeeApi';
import './EmployeeManagement.css';

export const EmployeeManagement: React.FC = () => {
  // States quản lý dữ liệu API
  const [employees, setEmployees] = useState<UserProfileDetail[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [totalRecords, setTotalRecords] = useState(0);
  
  // States quản lý tìm kiếm & phân trang
  const [searchTerm, setSearchTerm] = useState('');
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize] = useState(15); // Có thể đổi thành state nếu cho user chọn kích thước trang

  // States quản lý UI (Mở panel, tùy chỉnh cột)
  const [selectedEmp, setSelectedEmp] = useState<UserProfileDetail | null>(null);
  const [isPanelOpen, setIsPanelOpen] = useState(false);
  const [isDrawerOpen, setIsDrawerOpen] = useState(false);
  const [visibleColumns, setVisibleColumns] = useState<string[]>([]);

  // 1. Fetch dữ liệu từ API
  useEffect(() => {
    const fetchEmployees = async () => {
      try {
        setIsLoading(true);
        const response = await employeeApi.getEmployees({
          SearchTerm: searchTerm,
          PageNumber: pageNumber,
          PageSize: pageSize
        });
        
        if (response.succeeded) {
          setEmployees(response.data);
          setTotalRecords(response.totalRecords);
        }
      } catch (error) {
        console.error("Lỗi khi tải dữ liệu nhân viên:", error);
      } finally {
        setIsLoading(false);
      }
    };

    // Áp dụng Debounce cho tính năng tìm kiếm (Tránh gọi API liên tục khi đang gõ)
    const timeoutId = setTimeout(() => {
      fetchEmployees();
    }, 400);

    return () => clearTimeout(timeoutId);
  }, [searchTerm, pageNumber, pageSize]);

  // 2. Load cấu hình cột từ LocalStorage
  useEffect(() => {
    const saved = localStorage.getItem('empTableColumns');
    if (saved) {
      setVisibleColumns(JSON.parse(saved));
    } else {
      setVisibleColumns(['cccd', 'hoTen', 'tenChucVu', 'tenPhongBan', 'trangThai']);
    }
  }, []);

  const handleColumnChange = (newCols: string[]) => {
    setVisibleColumns(newCols);
    localStorage.setItem('empTableColumns', JSON.stringify(newCols));
  };

  const handleRowClick = (emp: UserProfileDetail) => {
    setSelectedEmp(emp);
    setIsPanelOpen(true);
  };

  const handleSearchChange = (value: string) => {
    setSearchTerm(value);
    setPageNumber(1); // Reset về trang 1 khi tìm kiếm từ khóa mới
  };

  return (
    <div className="emp-wrapper p-4 sm:p-6">
      <EmpTable 
        data={employees} // <--- Đưa dữ liệu thật vào đây
        visibleColumns={visibleColumns}
        isLoading={isLoading}
        searchTerm={searchTerm}
        onSearchChange={handleSearchChange}
        pageNumber={pageNumber}
        pageSize={pageSize}
        totalRecords={totalRecords}
        onPageChange={(page) => setPageNumber(page)}
        onOpenSettings={() => setIsDrawerOpen(true)}
        onRowClick={handleRowClick}
      />

      <ColumnSetupDrawer 
        open={isDrawerOpen} 
        onClose={() => setIsDrawerOpen(false)}
        visibleColumns={visibleColumns}
        onChange={handleColumnChange}
      />

      <EmployeeDetailPanel 
        employee={selectedEmp}
        isOpen={isPanelOpen} 
        onClose={() => setIsPanelOpen(false)} 
      />
    </div>
  );
};