import React, { useState, useEffect } from 'react';
import { useSystemData } from '../hooks/useSystemData';
import { departmentApi } from '../api/departmentApi';
import type { EmployeeInDepartmentDto } from '../types/department.types';
import { Toast } from '@/components/Toast/Toast';

import { CreateDeptModal } from './Modals/CreateDeptModal';
import { TransferModal } from './Modals/TransferModal';
import { AdjustSalaryModal } from './Modals/AdjustSalaryModal';
import { ChangePositionModal } from './Modals/ChangePositionModal';

import { usePositions } from '@/features/positions/hooks/usePositions';
import { useDataTable } from '../../../hooks/useDataTable';
import { exportToExcel, exportToPdf, ExportColumn } from '../../../utils/exportUtils';
import { SortableHeader } from '../../../components/DataTable/SortableHeader';
import { ExportButtons } from '../../../components/DataTable/ExportButtons';
import './DepartmentManagement.css';

export const DepartmentManagement: React.FC = () => {
  const { departments, isLoading, refreshData, toast: sysToast, setToast: setSysToast } = useSystemData();
  const { positions, fetchPositions, toast: posToast, setToast: setPosToast } = usePositions();

  const [selectedDeptId, setSelectedDeptId] = useState<string | null>(null);
  const [deptEmployees, setDeptEmployees] = useState<EmployeeInDepartmentDto[]>([]);
  const [loadingEmp, setLoadingEmp] = useState(false);
  const [toast, setToast] = useState<{ message: string; type: 'success' | 'error' } | null>(null);

  const [isDeptModalOpen, setIsDeptModalOpen] = useState(false);
  const [isTransferModalOpen, setIsTransferModalOpen] = useState(false);
  const [isAdjustSalaryModalOpen, setIsAdjustSalaryModalOpen] = useState(false);
  const [isChangePositionModalOpen, setIsChangePositionModalOpen] = useState(false);

  const [selectedEmployee, setSelectedEmployee] = useState<EmployeeInDepartmentDto | null>(null);
  const [openDropdownId, setOpenDropdownId] = useState<string | null>(null);

  const {
    currentData,
    allFilteredAndSortedData,
    currentPage,
    totalPages,
    setCurrentPage,
    sortKey,
    sortDirection,
    handleSort,
    searchTerm,
    setSearchTerm
  } = useDataTable<EmployeeInDepartmentDto>({
    data: deptEmployees,
    initialPageSize: 10,
    searchableFields: ['cccd', 'hoTen', 'tenChucVu']
  });

  const handleExportExcel = () => {
    const columns: ExportColumn<EmployeeInDepartmentDto>[] = [
      { header: 'Mã NV', key: 'cccd' },
      { header: 'Họ Tên', key: 'hoTen' },
      { header: 'Chức Vụ', key: 'tenChucVu' },
      { header: 'Trạng Thái', key: 'tenTrangThai' },
    ];
    exportToExcel(allFilteredAndSortedData, columns, 'NhanSuPhongBan');
  };

  const handleExportPdf = () => {
    const columns: ExportColumn<EmployeeInDepartmentDto>[] = [
      { header: 'Mã NV', key: 'cccd' },
      { header: 'Họ Tên', key: 'hoTen' },
      { header: 'Chức Vụ', key: 'tenChucVu' },
      { header: 'Trạng Thái', key: 'tenTrangThai' },
    ];
    exportToPdf(allFilteredAndSortedData, columns, 'NhanSuPhongBan', 'Danh sách nhân sự phòng ban');
  };

  useEffect(() => {
    fetchPositions('', 'HOAT_DONG');
  }, [fetchPositions]);

  useEffect(() => {
    if (selectedDeptId) {
      fetchEmployees(selectedDeptId);
    } else {
      setDeptEmployees([]);
    }
  }, [selectedDeptId]);

  useEffect(() => {
    const handleClickOutside = (e: MouseEvent) => {
      if (!(e.target as Element).closest('.dept-td-actions')) {
        setOpenDropdownId(null);
      }
    };
    document.addEventListener('click', handleClickOutside);
    return () => document.removeEventListener('click', handleClickOutside);
  }, []);

  const fetchEmployees = async (idPb: string) => {
    setLoadingEmp(true);
    try {
      const res = await departmentApi.getEmployeesInDepartment(idPb);
      if (res.succeeded) setDeptEmployees(res.data);
    } catch (error) {
      console.error('Lỗi tải danh sách nhân viên', error);
      setToast({ message: 'Lỗi tải danh sách nhân viên', type: 'error' });
    } finally {
      setLoadingEmp(false);
    }
  };

  const handleOpenTransfer = (employee: EmployeeInDepartmentDto) => {
    setSelectedEmployee(employee);
    setIsTransferModalOpen(true);
  };

  const handleOpenAdjustSalary = (employee: EmployeeInDepartmentDto) => {
    setSelectedEmployee(employee);
    setIsAdjustSalaryModalOpen(true);
  };

  const handleOpenChangePosition = (employee: EmployeeInDepartmentDto) => {
    setSelectedEmployee(employee);
    setIsChangePositionModalOpen(true);
  };

  return (
    <div className="dept-container">
      {/* Header */}
      <div className="dept-header">
        <div className="dept-header-left">
          <h2 className="dept-title">🏢 Phòng ban & Vị trí</h2>
          <p className="dept-subtitle">Quản lý cơ cấu phòng ban và nhân sự</p>
        </div>
        <div className="dept-header-actions">
          <button
            onClick={() => setIsDeptModalOpen(true)}
            className="dept-btn-create"
            title="Thêm phòng ban mới"
          >
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '1rem', height: '1rem' }}>
              <path strokeLinecap="round" strokeLinejoin="round" d="M12 4.5v15m7.5-7.5h-15" />
            </svg>
            Phòng ban
          </button>
        </div>
      </div>

      <div className="dept-content">
        {/* Left Card: Department List */}
        <div className="dept-card dept-card-left">
          <div className="dept-card-header">
            Cơ cấu tổ chức
            <span className="dept-badge-count">{departments.length}</span>
          </div>
          <div className="dept-list-body">
            {isLoading ? (
              <div className="dept-spinner"></div>
            ) : departments.length === 0 ? (
              <div className="dept-empty">
                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" d="M2.25 21h19.5m-18-18v18m10.5-18v18m6-13.5V21M6.75 6.75h.75m-.75 3h.75m-.75 3h.75m3-6h.75m-.75 3h.75m-.75 3h.75M6.75 21v-3.375c0-.621.504-1.125 1.125-1.125h2.25c.621 0 1.125.504 1.125 1.125V21M3 3h12m-.75 4.5H21m-3.75 3.75h.008v.008h-.008v-.008zm0 3h.008v.008h-.008v-.008zm0 3h.008v.008h-.008v-.008z" />
                </svg>
                <p>Chưa có phòng ban</p>
              </div>
            ) : (
              departments.map((d) => (
                <button
                  key={d.idPb}
                  onClick={() => setSelectedDeptId(d.idPb)}
                  className={`dept-list-item ${selectedDeptId === d.idPb ? 'active' : ''}`}
                >
                  <div className="dept-list-item-content">
                    <div className="dept-item-title">{d.tenPb}</div>
                    <div className="dept-item-subtitle">{d.idPb}</div>
                  </div>
                  <svg
                    xmlns="http://www.w3.org/2000/svg"
                    viewBox="0 0 20 20"
                    fill="currentColor"
                    className={`dept-list-item-icon ${selectedDeptId === d.idPb ? 'active' : ''}`}
                  >
                    <path fillRule="evenodd" d="M7.21 14.77a.75.75 0 01.02-1.06L11.168 10 7.23 6.29a.75.75 0 111.04-1.08l4.5 4.25a.75.75 0 010 1.08l-4.5 4.25a.75.75 0 01-1.06-.02z" clipRule="evenodd" />
                  </svg>
                </button>
              ))
            )}
          </div>
        </div>

        {/* Right Card: Employee List */}
        <div className="dept-card dept-card-right">
          <div className="dept-card-header">
            <span style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
              <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" style={{ width: '1.25rem', height: '1.25rem', color: 'var(--text-secondary)' }}>
                <path strokeLinecap="round" strokeLinejoin="round" d="M15 19.128a9.38 9.38 0 002.625.372 9.337 9.337 0 004.121-.952 4.125 4.125 0 00-7.533-2.493M15 19.128v-.003c0-1.113-.285-2.16-.786-3.07M15 19.128v.106A12.318 12.318 0 018.624 21c-2.331 0-4.512-.645-6.374-1.766l-.001-.109a6.375 6.375 0 0111.964-3.07M12 6.375a3.375 3.375 0 11-6.75 0 3.375 3.375 0 016.75 0zm8.25 2.25a2.625 2.625 0 11-5.25 0 2.625 2.625 0 015.25 0z" />
              </svg>
              Danh sách nhân sự
              {selectedDeptId && (
                <span className="dept-selected-name">
                  / {departments.find((d) => d.idPb === selectedDeptId)?.tenPb}
                </span>
              )}
            </span>
          </div>

            <div style={{ display: 'flex', justifyContent: 'space-between', padding: '1rem', borderBottom: '1px solid var(--border-color)', gap: '1rem', flexWrap: 'wrap' }}>
              <div className="dept-search-box" style={{ flex: 1, minWidth: '250px' }}>
                <input
                  type="text"
                  placeholder="Tìm kiếm nhân sự (Tên, CCCD)..."
                  className="dept-search-input"
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  style={{ width: '100%', padding: '0.5rem 1rem', borderRadius: '4px', border: '1px solid var(--border-color)' }}
                  disabled={!selectedDeptId}
                />
              </div>
              <ExportButtons onExportExcel={handleExportExcel} onExportPdf={handleExportPdf} />
            </div>
            
          <div className="dept-table-wrapper">
            {!selectedDeptId ? (
              <div className="dept-empty">
                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" d="M15 15.97A4.004 4.004 0 0015.97 15m0 0v-4m0 4h4m-4 0l4 4m-8-12a4 4 0 11-8 0 4 4 0 018 0z" />
                </svg>
                <p>Vui lòng chọn phòng ban ở danh sách bên trái để xem chi tiết nhân sự</p>
              </div>
            ) : loadingEmp ? (
              <div className="dept-spinner"></div>
            ) : deptEmployees.length === 0 ? (
              <div className="dept-empty">
                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" d="M15.75 6a3.75 3.75 0 11-7.5 0 3.75 3.75 0 017.5 0zM4.501 20.118a7.5 7.5 0 0114.998 0A17.933 17.933 0 0112 21.75c-2.676 0-5.216-.584-7.499-1.632z" />
                </svg>
                <p>Phòng ban này hiện chưa có nhân sự</p>
              </div>
            ) : (
              <div style={{ display: 'flex', flexDirection: 'column', height: '100%' }}>
                <div style={{ flex: 1, overflowY: 'auto' }}>
                  <table className="dept-table" aria-label="Danh sách nhân sự">
                    <thead>
                      <tr>
                        <SortableHeader label="Mã NV" sortKey="cccd" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                        <SortableHeader label="Họ tên" sortKey="hoTen" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                        <SortableHeader label="Chức vụ" sortKey="tenChucVu" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                        <SortableHeader label="Trạng thái" sortKey="tenTrangThai" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                        <th style={{ textAlign: 'center' }}>Thao tác</th>
                      </tr>
                    </thead>
                    <tbody>
                      {currentData.map((emp) => (
                        <tr key={emp.cccd}>
                          <td className="dept-td-mono">{emp.cccd}</td>
                          <td className="dept-td-bold">{emp.hoTen}</td>
                          <td>{emp.tenChucVu}</td>
                          <td>
                            <span className={`dept-badge ${emp.trangThai === 'DANG_LAM_VIEC' ? 'active' : 'inactive'}`}>
                              <span className="dept-badge-dot"></span>
                              {emp.tenTrangThai || (emp.trangThai === 'DANG_LAM_VIEC' ? 'Đang làm việc' : 'Đã nghỉ')}
                            </span>
                          </td>
                          <td className="dept-td-actions">
                            {emp.trangThai === 'DANG_LAM_VIEC' && (
                              <div style={{ display: 'flex', justifyContent: 'center', position: 'relative' }}>
                                <button
                                  className="dept-btn-actions"
                                  onClick={(e) => {
                                    e.stopPropagation();
                                    setOpenDropdownId(openDropdownId === emp.cccd ? null : emp.cccd);
                                  }}
                                  aria-label="Thao tác"
                                >
                                  <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" style={{ width: '1.25rem', height: '1.25rem' }}>
                                    <path strokeLinecap="round" strokeLinejoin="round" d="M12 6.75a.75.75 0 1 1 0-1.5.75.75 0 0 1 0 1.5ZM12 12.75a.75.75 0 1 1 0-1.5.75.75 0 0 1 0 1.5ZM12 18.75a.75.75 0 1 1 0-1.5.75.75 0 0 1 0 1.5Z" />
                                  </svg>
                                </button>
                                {openDropdownId === emp.cccd && (
                                  <div className="dept-actions-dropdown">
                                    <button
                                      className="dept-dropdown-item"
                                      onClick={() => {
                                        handleOpenChangePosition(emp);
                                        setOpenDropdownId(null);
                                      }}
                                    >
                                      <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" style={{ width: '1rem', height: '1rem', color: 'var(--primary)' }}>
                                        <path strokeLinecap="round" strokeLinejoin="round" d="M15 9h3.75M15 12h3.75M15 15h3.75M4.5 19.5h15a2.25 2.25 0 002.25-2.25V6.75A2.25 2.25 0 0019.5 4.5h-15a2.25 2.25 0 00-2.25 2.25v10.5A2.25 2.25 0 004.5 19.5zm6-10.125a1.875 1.875 0 11-3.75 0 1.875 1.875 0 013.75 0zm1.294 6.336a6.721 6.721 0 01-3.17.789 6.721 6.721 0 01-3.168-.789 3.376 3.376 0 016.338 0z" />
                                      </svg>
                                      Thay đổi chức vụ
                                    </button>
                                    <button
                                      className="dept-dropdown-item"
                                      onClick={() => {
                                        handleOpenAdjustSalary(emp);
                                        setOpenDropdownId(null);
                                      }}
                                    >
                                      <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" style={{ width: '1rem', height: '1rem', color: 'var(--success-text)' }}>
                                        <path strokeLinecap="round" strokeLinejoin="round" d="M2.25 18L9 11.25l4.306 4.307a11.95 11.95 0 015.814-5.519l2.74-1.22m0 0l-5.94-2.28m5.94 2.28l-2.28 5.941" />
                                      </svg>
                                      Điều chỉnh bậc lương
                                    </button>
                                    <div className="dept-dropdown-divider"></div>
                                    <button
                                      className="dept-dropdown-item"
                                      onClick={() => {
                                        handleOpenTransfer(emp);
                                        setOpenDropdownId(null);
                                      }}
                                    >
                                      <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" style={{ width: '1rem', height: '1rem', color: 'var(--primary)' }}>
                                        <path strokeLinecap="round" strokeLinejoin="round" d="M7.5 21L3 16.5m0 0L7.5 12M3 16.5h13.5m0-13.5L21 7.5m0 0L16.5 12M21 7.5H7.5" />
                                      </svg>
                                      Điều chuyển phòng ban
                                    </button>
                                  </div>
                                )}
                              </div>
                            )}
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>

                {totalPages > 0 && (
                  <div className="dept-pagination">
                    <button 
                      className="dept-btn-page" 
                      onClick={() => setCurrentPage(p => Math.max(1, p - 1))}
                      disabled={currentPage === 1}
                    >
                      Trước
                    </button>
                    <span className="dept-page-info">
                      {currentPage} / {totalPages}
                    </span>
                    <button 
                      className="dept-btn-page"
                      onClick={() => setCurrentPage(p => Math.min(totalPages, p + 1))}
                      disabled={currentPage === totalPages}
                    >
                      Sau
                    </button>
                  </div>
                )}
              </div>
            )}
          </div>
        </div>
      </div>

      {/* CÁC MODALS */}
      {isDeptModalOpen && (
        <CreateDeptModal
          isOpen={isDeptModalOpen}
          onClose={() => setIsDeptModalOpen(false)}
          onSuccess={() => {
            refreshData();
            setToast({ message: 'Tạo phòng ban thành công!', type: 'success' });
          }}
        />
      )}

      {isTransferModalOpen && (
        <TransferModal
          isOpen={isTransferModalOpen}
          onClose={() => {
            setIsTransferModalOpen(false);
            setSelectedEmployee(null);
          }}
          onSuccess={() => {
            refreshData();
            if (selectedDeptId) fetchEmployees(selectedDeptId);
            setToast({ message: 'Điều chuyển nhân sự thành công!', type: 'success' });
          }}
          departments={departments}
          positions={positions}
          employee={selectedEmployee}
        />
      )}

      {isAdjustSalaryModalOpen && (
        <AdjustSalaryModal
          isOpen={isAdjustSalaryModalOpen}
          onClose={() => {
            setIsAdjustSalaryModalOpen(false);
            setSelectedEmployee(null);
          }}
          onSuccess={() => {
            refreshData();
            if (selectedDeptId) fetchEmployees(selectedDeptId);
            setToast({ message: 'Điều chỉnh lương thành công!', type: 'success' });
          }}
          employee={selectedEmployee}
          positions={positions}
        />
      )}

      {isChangePositionModalOpen && (
        <ChangePositionModal
          isOpen={isChangePositionModalOpen}
          onClose={() => {
            setIsChangePositionModalOpen(false);
            setSelectedEmployee(null);
          }}
          onSuccess={() => {
            refreshData();
            if (selectedDeptId) fetchEmployees(selectedDeptId);
            setToast({ message: 'Bổ nhiệm/miễn nhiệm thành công!', type: 'success' });
          }}
          employee={selectedEmployee}
          positions={positions.filter(p => p.idPhongBan === selectedDeptId)}
        />
      )}

      {toast && (
        <Toast
          message={toast.message}
          type={toast.type}
          onClose={() => setToast(null)}
        />
      )}
      
      {posToast && (
        <Toast
          message={posToast.message}
          type={posToast.type}
          onClose={() => setPosToast(null)}
        />
      )}

      {sysToast && (
        <Toast
          message={sysToast.message}
          type={sysToast.type}
          onClose={() => setSysToast(null)}
        />
      )}
    </div>
  );
};
