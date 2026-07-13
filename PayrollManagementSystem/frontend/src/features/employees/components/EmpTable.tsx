import React from 'react';
import { UserProfileDetail } from '@/types/profile.types';

interface EmpTableProps {
  data: UserProfileDetail[];
  visibleColumns: string[];
  isLoading: boolean;
  isExporting?: boolean;
  searchTerm: string;
  onSearchChange: (value: string) => void;
  pageNumber: number;
  totalRecords: number;
  pageSize: number;
  onPageChange: (newPage: number) => void;
  onOpenSettings: () => void;
  onRowClick: (record: UserProfileDetail) => void;
  onStatusClick: (record: UserProfileDetail) => void;
  onEditClick: (record: UserProfileDetail) => void;
  onExportExcel: () => void;
}

export const EmpTable: React.FC<EmpTableProps> = ({ 
  data, visibleColumns, isLoading, isExporting, searchTerm, onSearchChange, 
  pageNumber, totalRecords, pageSize, 
  onPageChange, onOpenSettings, onRowClick, onStatusClick, onEditClick, onExportExcel 
}) => {
  const isVisible = (key: string) => visibleColumns.includes(key);
  const totalPages = Math.ceil(totalRecords / pageSize) || 1;

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
            onChange={(e) => onSearchChange(e.target.value)}
          />
        </div>
        
        <div className="emp-toolbar-actions">
          <button 
            onClick={onExportExcel} 
            disabled={isExporting}
            className="emp-btn-outline"
          >
            {isExporting ? (
              <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '1.2rem', height: '1.2rem', color: '#10b981', animation: 'spin 1s linear infinite' }}>
                <path strokeLinecap="round" strokeLinejoin="round" d="M16.023 9.348h4.992v-.001M2.985 19.644v-4.992m0 0h4.992m-4.993 0 3.181 3.183a8.25 8.25 0 0 0 13.803-3.7M4.031 9.865a8.25 8.25 0 0 1 13.803-3.7l3.181 3.182m0-4.991v4.99" />
              </svg>
            ) : (
              <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '1.2rem', height: '1.2rem', color: '#10b981' }}>
                <path strokeLinecap="round" strokeLinejoin="round" d="M19.5 14.25v-2.625a3.375 3.375 0 00-3.375-3.375h-1.5A1.125 1.125 0 0113.5 7.125v-1.5a3.375 3.375 0 00-3.375-3.375H8.25m.75 12l3 3m0 0l3-3m-3 3v-6m-1.5-9H5.625c-.621 0-1.125.504-1.125 1.125v17.25c0 .621.504 1.125 1.125 1.125h12.75c.621 0 1.125-.504 1.125-1.125V11.25a9 9 0 00-9-9z" />
              </svg>
            )}
            {isExporting ? 'Đang xuất...' : 'Xuất Excel'}
          </button>
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
              {isVisible('cccd') && <th>Mã NV (CCCD)</th>}
              {isVisible('hoTen') && <th>Họ tên</th>}
              {isVisible('tenChucVu') && <th>Chức vụ</th>}
              {isVisible('tenPhongBan') && <th>Phòng ban</th>}
              {isVisible('ngayVaoLam') && <th>Ngày vào làm</th>}
              {isVisible('trangThai') && <th>Trạng thái</th>}
              <th style={{ textAlign: 'right' }}>Hành động</th>
            </tr>
          </thead>
          <tbody>
            {data.length === 0 && !isLoading ? (
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
              data.map((row) => (
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
                    <button 
                      onClick={(e) => { e.stopPropagation(); onEditClick(row); }} 
                      className="emp-action-link edit"
                    >
                      Sửa
                    </button>
                    <button 
                      onClick={(e) => { e.stopPropagation(); onStatusClick(row); }} 
                      className="emp-action-link status"
                    >
                      Đổi TT
                    </button>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>

      <div className="emp-pagination-bar">
        <div className="emp-pagination-total">
          Tổng số <strong>{totalRecords}</strong> nhân viên
        </div>
        <div className="emp-pagination-controls">
          <button 
            disabled={pageNumber === 1 || isLoading} 
            onClick={() => onPageChange(pageNumber - 1)} 
            className="emp-btn-page"
          >
            Trước
          </button>
          <span className="emp-page-info">
            Trang <strong>{pageNumber}</strong> / {totalPages}
          </span>
          <button 
            disabled={pageNumber >= totalPages || isLoading} 
            onClick={() => onPageChange(pageNumber + 1)} 
            className="emp-btn-page"
          >
            Sau
          </button>
        </div>
      </div>

    </div>
  );
};