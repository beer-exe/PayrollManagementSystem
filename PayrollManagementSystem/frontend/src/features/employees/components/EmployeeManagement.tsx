import React, { useState, useEffect } from 'react';
import { EmpTable } from './EmpTable';
import { ColumnSetupDrawer } from './ColumnSetupDrawer';
import { EmployeeDetailPanel } from './EmployeeDetailPanel';
import { ChangeStatusModal } from './ChangeStatusModal';
import { CreateEmployeeStepper } from './CreateEmployeeStepper';
import { UpdateEmployeeModal } from './UpdateEmployeeModal';
import { UserProfileDetail } from '@/types/profile.types';
import { CreateEmployeeCommand } from '../types/employee.types';
import { useEmployees } from '../hooks/useEmployees';
import './EmployeeManagement.css';
import './EmployeeModals.css';

export const EmployeeManagement: React.FC = () => {
  const { 
    employees, totalRecords, loading, isExporting,
    fetchEmployees, exportExcel, changeStatus, createEmployee, updateEmployee 
  } = useEmployees();
  
  const [searchTerm, setSearchTerm] = useState('');
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize] = useState(15); 

  const [selectedEmp, setSelectedEmp] = useState<UserProfileDetail | null>(null);
  const [isPanelOpen, setIsPanelOpen] = useState(false);
  const [isDrawerOpen, setIsDrawerOpen] = useState(false);
  const [visibleColumns, setVisibleColumns] = useState<string[]>([]);
  
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const [employeeToChangeStatus, setEmployeeToChangeStatus] = useState<UserProfileDetail | null>(null);
  
  const [isUpdateModalOpen, setIsUpdateModalOpen] = useState(false);
  const [employeeToUpdate, setEmployeeToUpdate] = useState<UserProfileDetail | null>(null);

  useEffect(() => {
    const timeoutId = setTimeout(() => {
      fetchEmployees(searchTerm, pageNumber, pageSize);
    }, 400);
    return () => clearTimeout(timeoutId);
  }, [searchTerm, pageNumber, pageSize, fetchEmployees]);

  useEffect(() => {
    const saved = localStorage.getItem('empTableColumns');
    if (saved) {
      setVisibleColumns(JSON.parse(saved));
    } else {
      setVisibleColumns(['cccd', 'hoTen', 'tenChucVu', 'tenPhongBan', 'trangThai']);
    }
  }, []);

  const handleCreateSuccess = async (data: any) => {
    const command: CreateEmployeeCommand = {
      ...data,
      luongCoBan: 0
    };
    const success = await createEmployee(command);
    if (success) {
      setIsCreateModalOpen(false);
      setPageNumber(1); 
      fetchEmployees(searchTerm, 1, pageSize);
    }
    return success;
  };

  const handleEditClick = (emp: UserProfileDetail) => {
    setEmployeeToUpdate(emp);
    setIsUpdateModalOpen(true);
  };

  return (
    <div className="emp-wrapper">
      
      <div className="emp-header">
        <div className="emp-header-left">
          <h2 className="emp-title">👨‍💼 Hồ sơ Nhân sự</h2>
          <p className="emp-subtitle">Quản lý danh sách nhân sự toàn công ty</p>
        </div>
        <div className="emp-header-actions">
          <button 
            onClick={() => setIsCreateModalOpen(true)}
            className="emp-btn-primary"
          >
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{width: '1.2rem', height: '1.2rem'}}>
              <path strokeLinecap="round" strokeLinejoin="round" d="M12 4.5v15m7.5-7.5h-15" />
            </svg>
            Thêm Nhân Viên
          </button>
        </div>
      </div>

      <EmpTable 
        data={employees}
        visibleColumns={visibleColumns}
        isLoading={loading}
        isExporting={isExporting}
        searchTerm={searchTerm}
        onSearchChange={(v) => { setSearchTerm(v); setPageNumber(1); }}
        pageNumber={pageNumber}
        pageSize={pageSize}
        totalRecords={totalRecords}
        onPageChange={setPageNumber}
        onOpenSettings={() => setIsDrawerOpen(true)}
        onRowClick={(emp) => { setSelectedEmp(emp); setIsPanelOpen(true); }}
        onStatusClick={(emp) => setEmployeeToChangeStatus(emp)}
        onEditClick={handleEditClick}
        onExportExcel={() => exportExcel(searchTerm)}
      />

      <ColumnSetupDrawer 
        open={isDrawerOpen} 
        onClose={() => setIsDrawerOpen(false)}
        visibleColumns={visibleColumns}
        onChange={(cols) => { 
          setVisibleColumns(cols); 
          localStorage.setItem('empTableColumns', JSON.stringify(cols)); 
        }}
      />

      <EmployeeDetailPanel 
        employee={selectedEmp}
        isOpen={isPanelOpen} 
        onClose={() => setIsPanelOpen(false)} 
      />

      {isCreateModalOpen && (
        <div className="emp-modal-overlay">
          <div className="emp-modal-large" style={{ background: '#f9fafb' }}>
            <CreateEmployeeStepper 
              onSubmitSuccess={handleCreateSuccess}
              onCancel={() => setIsCreateModalOpen(false)}
            />
          </div>
        </div>
      )}

      {isUpdateModalOpen && employeeToUpdate && (
        <UpdateEmployeeModal
          isOpen={isUpdateModalOpen}
          onClose={() => {
            setIsUpdateModalOpen(false);
            setEmployeeToUpdate(null);
          }}
          employee={employeeToUpdate}
          onSubmitUpdate={async (cccd, data) => {
            const success = await updateEmployee(cccd, data);
            if (success) {
              fetchEmployees(searchTerm, pageNumber, pageSize);
            }
            return success;
          }}
        />
      )}

      {employeeToChangeStatus && (
        <ChangeStatusModal 
          isOpen={!!employeeToChangeStatus}
          onClose={() => setEmployeeToChangeStatus(null)}
          cccd={employeeToChangeStatus.cccd}
          currentStatus={employeeToChangeStatus.trangThai || 'DANG_LAM_VIEC'}
          onSubmitStatus={async (data) => {
            const success = await changeStatus(employeeToChangeStatus.cccd, data);
            if (success) fetchEmployees(searchTerm, pageNumber, pageSize);
            return success;
          }}
        />
      )}
    </div>
  );
};