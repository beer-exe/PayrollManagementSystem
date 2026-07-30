import React, { useState, useEffect } from 'react';
import { useUsers } from '../hooks/useUsers';
import { useDataTable } from '../../../hooks/useDataTable';
import { exportToExcel, exportToPdf, ExportColumn } from '../../../utils/exportUtils';
import { SortableHeader } from '../../../components/DataTable/SortableHeader';
import { ExportButtons } from '../../../components/DataTable/ExportButtons';
import { CreateUserModal } from './CreateUserModal';
import { UpdateRoleModal } from './UpdateRoleModal';
import { UserDto } from '../types/user.types';
import { Toast } from '../../../components/Toast/Toast';
import './UserManagement.css';

export const UserManagement: React.FC = () => {
  const { users, roles, isLoading, handleCreateUser, handleUpdateRole, handleToggleStatus, handleResetPassword, toast, setToast } = useUsers();

  const [isCreateOpen, setIsCreateOpen] = useState(false);
  const [roleModalUser, setRoleModalUser] = useState<UserDto | null>(null);
  const [activeDropdown, setActiveDropdown] = useState<string | null>(null);
  
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
  } = useDataTable<UserDto>({
    data: users,
    initialPageSize: 10,
    searchableFields: ['tenTaiKhoan', 'hoTen', 'email']
  });

  useEffect(() => {
    const handleClickOutside = (e: MouseEvent) => {
      if (activeDropdown && !(e.target as Element).closest('.usr-actions')) {
        setActiveDropdown(null);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, [activeDropdown]);

  const handleExportExcel = () => {
    const columns: ExportColumn<UserDto>[] = [
      { header: 'Tài khoản', key: 'tenTaiKhoan' },
      { header: 'Họ Tên', key: 'hoTen' },
      { header: 'Email', key: 'email' },
      { header: 'Vai trò', key: 'tenVaiTro' },
      { header: 'Trạng thái', key: 'tenTrangThai' },
    ];
    exportToExcel(allFilteredAndSortedData, columns, 'DanhSachTaiKhoan');
  };

  const handleExportPdf = () => {
    const columns: ExportColumn<UserDto>[] = [
      { header: 'Tài khoản', key: 'tenTaiKhoan' },
      { header: 'Họ Tên', key: 'hoTen' },
      { header: 'Email', key: 'email' },
      { header: 'Vai trò', key: 'tenVaiTro' },
      { header: 'Trạng thái', key: 'tenTrangThai' },
    ];
    exportToPdf(allFilteredAndSortedData, columns, 'DanhSachTaiKhoan', 'Danh sách Tài khoản Hệ thống');
  };

  const onResetPasswordClick = (user: UserDto) => {
    if (window.confirm(`Xác nhận đặt lại mật khẩu cho tài khoản ${user.tenTaiKhoan}?`)) {
      handleResetPassword(user.idTaiKhoan, { idTaiKhoan: user.idTaiKhoan, newPassword: 'NewPassword@123' });
      setActiveDropdown(null);
    }
  };

  const getRoleBadgeClass = (roleName: string) => {
    const lower = roleName.toLowerCase();
    if (lower.includes('admin') || lower.includes('quản trị')) return 'usr-badge-admin';
    if (lower.includes('hr') || lower.includes('nhân sự')) return 'usr-badge-hr';
    return 'usr-badge-user';
  };

  return (
    <div className="usr-container">
      <div className="usr-header">
        <div className="usr-header-title">
          <h2>🔐 Quản lý Tài khoản</h2>
          <p>Phân quyền và kiểm soát truy cập hệ thống</p>
        </div>
        <button className="usr-btn usr-btn-primary" onClick={() => setIsCreateOpen(true)}>
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '1.25rem', height: '1.25rem' }}>
            <path strokeLinecap="round" strokeLinejoin="round" d="M12 4.5v15m7.5-7.5h-15" />
          </svg>
          Thêm tài khoản
        </button>
      </div>

      <div className="usr-controls-wrapper">
        <div className="usr-filters" style={{ display: 'flex', justifyContent: 'space-between', width: '100%', flexWrap: 'wrap', gap: '1rem' }}>
          <div className="usr-input-wrapper" style={{ width: 'auto', minWidth: '300px' }}>
            <svg className="usr-input-icon" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '1.25rem', height: '1.25rem' }}>
              <path strokeLinecap="round" strokeLinejoin="round" d="M21 21l-5.197-5.197m0 0A7.5 7.5 0 105.196 5.196a7.5 7.5 0 0010.607 10.607z" />
            </svg>
            <input 
              type="text" 
              placeholder="Tìm theo username, họ tên, email..." 
              className="usr-input with-icon"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
            />
          </div>
          
          <ExportButtons onExportExcel={handleExportExcel} onExportPdf={handleExportPdf} />
        </div>

        <div className="usr-table-container custom-scrollbar">
          {isLoading ? (
            <div className="usr-loader">
              <div className="usr-spinner"></div>
            </div>
          ) : currentData.length > 0 ? (
            <table className="usr-table">
              <thead>
                <tr>
                  <SortableHeader 
                    label="Tài khoản" sortKey="tenTaiKhoan" 
                    currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} 
                  />
                  <SortableHeader 
                    label="Thông tin nhân viên" sortKey="hoTen" 
                    currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} 
                  />
                  <SortableHeader 
                    label="Vai trò" sortKey="tenVaiTro" 
                    currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} 
                  />
                  <SortableHeader 
                    label="Trạng thái" sortKey="tenTrangThai" 
                    currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} 
                    style={{ textAlign: 'center' }}
                  />
                  <th style={{ textAlign: 'right' }}>Thao tác</th>
                </tr>
              </thead>
              <tbody>
                {currentData.map(record => {
                  const isActive = record.trangThai === 'HOAT_DONG';

                  return (
                    <tr key={record.idTaiKhoan}>
                      <td style={{ fontWeight: 600, color: 'var(--text-primary)' }}>
                        {record.tenTaiKhoan}
                      </td>
                      <td>
                        <div style={{ display: 'flex', flexDirection: 'column' }}>
                          <span style={{ fontWeight: 600, color: 'var(--text-primary)' }}>{record.hoTen}</span>
                          <span style={{ fontSize: '0.75rem', color: 'var(--text-secondary)', marginTop: '0.25rem' }}>
                            {record.email || 'Chưa cập nhật email'}
                          </span>
                        </div>
                      </td>
                      <td>
                        <span className={`usr-badge ${getRoleBadgeClass(record.tenVaiTro)}`}>
                          {record.tenVaiTro}
                        </span>
                      </td>
                      <td style={{ textAlign: 'center' }}>
                        <div className="usr-switch-container" style={{ justifyContent: 'center' }}>
                          <label className="usr-switch">
                            <input 
                              type="checkbox" 
                              checked={isActive} 
                              onChange={() => handleToggleStatus(record.idTaiKhoan)} 
                            />
                            <span className="usr-slider"></span>
                          </label>
                          <span className={`usr-status-label ${isActive ? 'active' : 'inactive'}`} style={{ width: '70px', textAlign: 'left' }}>
                            {record.tenTrangThai}
                          </span>
                        </div>
                      </td>
                      <td>
                        <div className="usr-actions">
                          <button 
                            className="usr-btn-actions"
                            onClick={(e) => {
                              e.stopPropagation();
                              setActiveDropdown(activeDropdown === record.idTaiKhoan ? null : record.idTaiKhoan);
                            }}
                          >
                            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" style={{ width: '1.25rem', height: '1.25rem' }}>
                              <path strokeLinecap="round" strokeLinejoin="round" d="M12 6.75a.75.75 0 110-1.5.75.75 0 010 1.5zM12 12.75a.75.75 0 110-1.5.75.75 0 010 1.5zM12 18.75a.75.75 0 110-1.5.75.75 0 010 1.5z" />
                            </svg>
                          </button>
                          
                          {activeDropdown === record.idTaiKhoan && (
                            <div className="usr-actions-dropdown">
                              <button 
                                className="usr-dropdown-item" 
                                onClick={() => {
                                  setRoleModalUser(record);
                                  setActiveDropdown(null);
                                }}
                              >
                                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" style={{ width: '1rem', height: '1rem' }}>
                                  <path strokeLinecap="round" strokeLinejoin="round" d="M15.75 6a3.75 3.75 0 11-7.5 0 3.75 3.75 0 017.5 0zM4.501 20.118a7.5 7.5 0 0114.998 0A17.933 17.933 0 0112 21.75c-2.676 0-5.216-.584-7.499-1.632z" />
                                </svg>
                                Đổi quyền hạn
                              </button>
                              
                              <div className="usr-dropdown-divider"></div>
                              
                              <button 
                                className="usr-dropdown-item danger" 
                                onClick={() => onResetPasswordClick(record)}
                              >
                                <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" style={{ width: '1rem', height: '1rem' }}>
                                  <path strokeLinecap="round" strokeLinejoin="round" d="M15.75 5.25a3 3 0 013 3m3 0a6 6 0 01-7.029 5.912c-.563-.097-1.159.026-1.563.43L10.5 17.25H8.25v2.25H6v2.25H2.25v-2.818c0-.597.237-1.17.659-1.591l6.499-6.499c.404-.404.527-1 .43-1.563A6 6 0 1121.75 8.25z" />
                                </svg>
                                Đặt lại mật khẩu
                              </button>
                            </div>
                          )}
                        </div>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          ) : (
            <div className="usr-empty">
              <p>Không tìm thấy dữ liệu tài khoản nào.</p>
            </div>
          )}
        </div>

        {totalPages > 0 && (
          <div className="usr-pagination">
            <button 
              className="usr-btn usr-btn-secondary" 
              onClick={() => setCurrentPage(p => p - 1)} 
              disabled={currentPage === 1 || isLoading}
              style={{ padding: '0.35rem 0.75rem' }}
            >
              [Trước]
            </button>
            <div className="usr-pagination-info">
              Trang <span>{currentPage}</span> / <span>{totalPages}</span>
            </div>
            <button 
              className="usr-btn usr-btn-secondary" 
              onClick={() => setCurrentPage(p => p + 1)} 
              disabled={currentPage === totalPages || isLoading}
              style={{ padding: '0.35rem 0.75rem' }}
            >
              [Sau]
            </button>
          </div>
        )}
      </div>

      {/* Modals */}
      <CreateUserModal 
        isOpen={isCreateOpen} 
        onClose={() => setIsCreateOpen(false)} 
        onSubmit={handleCreateUser} 
        roles={roles}
      />
      <UpdateRoleModal 
        user={roleModalUser}
        isOpen={!!roleModalUser}
        onClose={() => setRoleModalUser(null)}
        onSubmit={handleUpdateRole}
        roles={roles}
      />

      {toast && (
        <Toast
          message={toast.message}
          type={toast.type}
          onClose={() => setToast(null)}
        />
      )}
    </div>
  );
};