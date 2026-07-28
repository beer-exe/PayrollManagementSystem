import React, { useState, useEffect, useRef } from 'react';
import { UserProfileDetail } from '@/types/profile.types';
import { useDataTable } from '../../../hooks/useDataTable';
import { exportToExcel, exportToPdf, ExportColumn } from '../../../utils/exportUtils';
import { SortableHeader } from '../../../components/DataTable/SortableHeader';
import { ExportButtons } from '../../../components/DataTable/ExportButtons';

interface EmpTableProps {
  data: UserProfileDetail[];
  visibleColumns: string[];
  isLoading: boolean;
  onOpenSettings: () => void;
  onRowClick: (record: UserProfileDetail) => void;
  onStatusClick: (record: UserProfileDetail) => void;
  onEditClick: (record: UserProfileDetail) => void;
  onAssignDeptClick: (record: UserProfileDetail) => void;
}

export const EmpTable: React.FC<EmpTableProps> = ({ 
  data, visibleColumns, isLoading,
  onOpenSettings, onRowClick, onStatusClick, onEditClick, onAssignDeptClick 
}) => {
  const isVisible = (key: string) => visibleColumns.includes(key);

  const [activeDropdown, setActiveDropdown] = useState<string | null>(null);
  const dropdownRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
        setActiveDropdown(null);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

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
  } = useDataTable<UserProfileDetail>({
    data: data,
    initialPageSize: 10,
    searchableFields: ['cccd', 'hoTen', 'email']
  });

  const handleExportExcel = () => {
    const columns: ExportColumn<UserProfileDetail>[] = [
      { header: 'Mã NV', key: 'cccd' },
      { header: 'Họ Tên', key: 'hoTen' },
      { header: 'Chức vụ', key: 'tenChucVu' },
      { header: 'Phòng ban', key: 'tenPhongBan' },
      { header: 'Ngày vào làm', key: 'ngayVaoLam' },
      { header: 'Trạng thái', key: 'tenTrangThai' },
    ];
    exportToExcel(allFilteredAndSortedData, columns, 'DanhSachNhanVien');
  };

  const handleExportPdf = () => {
    const columns: ExportColumn<UserProfileDetail>[] = [
      { header: 'Mã NV', key: 'cccd' },
      { header: 'Họ Tên', key: 'hoTen' },
      { header: 'Chức vụ', key: 'tenChucVu' },
      { header: 'Phòng ban', key: 'tenPhongBan' },
      { header: 'Ngày vào làm', key: 'ngayVaoLam' },
      { header: 'Trạng thái', key: 'tenTrangThai' },
    ];
    exportToPdf(allFilteredAndSortedData, columns, 'DanhSachNhanVien', 'Danh sách Nhân viên');
  };

  const getInitials = (name: string) => {
    if (!name) return 'U';
    const parts = name.trim().split(' ');
    if (parts.length === 1) return parts[0].charAt(0).toUpperCase();
    return (parts[0].charAt(0) + parts[parts.length - 1].charAt(0)).toUpperCase();
  };

  return (
    <div className="emp-card">
      <div className="emp-toolbar">
        <div className="emp-search-box">
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '1.2rem', height: '1.2rem', color: '#9ca3af' }}>
            <path strokeLinecap="round" strokeLinejoin="round" d="M21 21l-5.197-5.197m0 0A7.5 7.5 0 105.196 5.196a7.5 7.5 0 0010.607 10.607z" />
          </svg>
          <input 
            type="text" 
            placeholder="Tìm kiếm theo CCCD, họ tên..." 
            className="emp-search-input" 
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
          />
        </div>
        
        <div className="emp-toolbar-actions">
          <ExportButtons onExportExcel={handleExportExcel} onExportPdf={handleExportPdf} />
          <button onClick={onOpenSettings} className="emp-btn-icon" title="Tùy chỉnh hiển thị cột">
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" style={{ width: '1.25rem', height: '1.25rem' }}>
              <path strokeLinecap="round" strokeLinejoin="round" d="M9 4.5v15m6-15v15m-10.5-6h15m-15-6h15m-3-4.5h3.75c.621 0 1.125.504 1.125 1.125v15.75c0 .621-.504 1.125-1.125 1.125H3.75c-.621 0-1.125-.504-1.125-1.125V5.625c0-.621.504-1.125 1.125-1.125H6.75Z" />
            </svg>
          </button>
        </div>
      </div>

      <div className="emp-table-wrapper">
        {isLoading && (
          <div className="emp-spinner-overlay">
            <div className="emp-spinner"></div>
          </div>
        )}
        
        <table className="emp-table" aria-label="Danh sách nhân viên">
          <thead>
            <tr>
              {isVisible('cccd') && <SortableHeader label="Mã NV (CCCD)" sortKey="cccd" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />}
              {isVisible('hoTen') && <SortableHeader label="Họ tên" sortKey="hoTen" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />}
              {isVisible('tenChucVu') && <SortableHeader label="Chức vụ" sortKey="tenChucVu" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />}
              {isVisible('tenPhongBan') && <SortableHeader label="Phòng ban" sortKey="tenPhongBan" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />}
              {isVisible('ngayVaoLam') && <SortableHeader label="Ngày vào làm" sortKey="ngayVaoLam" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />}
              {isVisible('trangThai') && <SortableHeader label="Trạng thái" sortKey="tenTrangThai" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />}
              <th style={{ textAlign: 'right' }}>Hành động</th>
            </tr>
          </thead>
          <tbody>
            {currentData.length === 0 && !isLoading ? (
              <tr>
                <td colSpan={7} style={{ padding: 0 }}>
                  <div className="emp-empty">
                    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
                      <path strokeLinecap="round" strokeLinejoin="round" d="M15 15.97A4.004 4.004 0 0015.97 15m0 0v-4m0 4h4m-4 0l4 4m-8-12a4 4 0 11-8 0 4 4 0 018 0z" />
                    </svg>
                    <p>Không tìm thấy nhân viên nào</p>
                  </div>
                </td>
              </tr>
            ) : (
              currentData.map((row) => (
                <tr key={row.cccd} onClick={() => onRowClick(row)} className="emp-tr">
                  {isVisible('cccd') && <td className="emp-td-mono">{row.cccd}</td>}
                  {isVisible('hoTen') && (
                    <td>
                      <div className="emp-info">
                        <div className="emp-avatar">{getInitials(row.hoTen)}</div>
                        <span className="emp-name">{row.hoTen}</span>
                      </div>
                    </td>
                  )}
                  {isVisible('tenChucVu') && <td style={{ fontWeight: 500 }}>{row.tenChucVu || '—'}</td>}
                  {isVisible('tenPhongBan') && <td>{row.tenPhongBan || '—'}</td>}
                  {isVisible('ngayVaoLam') && <td>{row.ngayVaoLam || '—'}</td>}
                  {isVisible('trangThai') && (
                    <td>
                      <span className={`emp-badge ${row.trangThai === 'DANG_LAM_VIEC' ? 'active' : 'inactive'}`}>
                        <span className="emp-badge-dot"></span>
                        {row.tenTrangThai || (row.trangThai === 'DANG_LAM_VIEC' ? 'Đang làm việc' : 'Đã nghỉ')}
                      </span>
                    </td>
                  )}
                  <td style={{ textAlign: 'right' }}>
                    <div className="emp-dropdown-container" ref={activeDropdown === row.cccd ? dropdownRef : null}>
                      <button 
                        className="emp-btn-more"
                        onClick={(e) => { 
                          e.stopPropagation(); 
                          setActiveDropdown(activeDropdown === row.cccd ? null : row.cccd);
                        }}
                      >
                        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '1.25rem', height: '1.25rem' }}>
                          <path strokeLinecap="round" strokeLinejoin="round" d="M6.75 12a.75.75 0 1 1-1.5 0 .75.75 0 0 1 1.5 0ZM12.75 12a.75.75 0 1 1-1.5 0 .75.75 0 0 1 1.5 0ZM18.75 12a.75.75 0 1 1-1.5 0 .75.75 0 0 1 1.5 0Z" />
                        </svg>
                      </button>

                      {activeDropdown === row.cccd && (
                        <div className="emp-dropdown-menu">
                          {(!row.idPb || row.idPb === '') && row.trangThai === 'DANG_LAM_VIEC' && (
                            <button 
                              onClick={(e) => { e.stopPropagation(); setActiveDropdown(null); onAssignDeptClick(row); }} 
                              className="emp-dropdown-item assign"
                            >
                              Thêm vào phòng ban
                            </button>
                          )}
                          <button 
                            onClick={(e) => { e.stopPropagation(); setActiveDropdown(null); onEditClick(row); }} 
                            className="emp-dropdown-item edit"
                          >
                            Sửa thông tin
                          </button>
                          <button 
                            onClick={(e) => { e.stopPropagation(); setActiveDropdown(null); onStatusClick(row); }} 
                            className="emp-dropdown-item status"
                          >
                            Đổi trạng thái
                          </button>
                        </div>
                      )}
                    </div>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>

      {totalPages > 0 && (
        <div className="emp-pagination-bar">
          <div className="emp-pagination-total">
            Tổng số <strong>{allFilteredAndSortedData.length}</strong> nhân viên
          </div>
          <div className="emp-pagination-controls">
            <button 
              disabled={currentPage === 1 || isLoading} 
              onClick={() => setCurrentPage(currentPage - 1)} 
              className="emp-btn-page"
            >
              Trước
            </button>
            <span className="emp-page-info">
              Trang <strong>{currentPage}</strong> / {totalPages}
            </span>
            <button 
              disabled={currentPage >= totalPages || isLoading} 
              onClick={() => setCurrentPage(currentPage + 1)} 
              className="emp-btn-page"
            >
              Sau
            </button>
          </div>
        </div>
      )}

    </div>
  );
};