import React from 'react';
import { UserProfileDetail } from '@/types/profile.types';

interface EmpTableProps {
  data: UserProfileDetail[];
  visibleColumns: string[];
  isLoading: boolean;
  searchTerm: string;
  onSearchChange: (value: string) => void;
  pageNumber: number;
  totalRecords: number;
  pageSize: number;
  onPageChange: (newPage: number) => void;
  onOpenSettings: () => void;
  onRowClick: (record: UserProfileDetail) => void;
}

export const EmpTable: React.FC<EmpTableProps> = ({ 
  data, visibleColumns, isLoading, searchTerm, onSearchChange, 
  pageNumber, totalRecords, pageSize, onPageChange, onOpenSettings, onRowClick 
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
    <div className="emp-card flex flex-col h-full">
      {/* Toolbar */}
      <div className="emp-toolbar flex-shrink-0">
        <div className="emp-search-box">
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-5 h-5 text-gray-400">
            <path strokeLinecap="round" strokeLinejoin="round" d="m21 21-5.197-5.197m0 0A7.5 7.5 0 1 0 5.196 5.196a7.5 7.5 0 0 0 10.607 10.607Z" />
          </svg>
          <input 
            type="text" 
            placeholder="Tìm kiếm theo mã, tên..." 
            className="emp-search-input" 
            value={searchTerm}
            onChange={(e) => onSearchChange(e.target.value)}
          />
        </div>
        
        <button onClick={onOpenSettings} className="btn-icon" title="Tùy chỉnh cột">
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-5 h-5"><path strokeLinecap="round" strokeLinejoin="round" d="M9 4.5v15m6-15v15m-10.5-6h15m-15-6h15m-3-4.5h3.75c.621 0 1.125.504 1.125 1.125v15.75c0 .621-.504 1.125-1.125 1.125H3.75c-.621 0-1.125-.504-1.125-1.125V5.625c0-.621.504-1.125 1.125-1.125H6.75Z" /></svg>
        </button>
      </div>

      {/* Table Content */}
      <div className="emp-table-container flex-1 relative">
        {isLoading && (
          <div className="absolute inset-0 bg-white/60 dark:bg-gray-800/60 backdrop-blur-[1px] z-20 flex items-center justify-center">
            <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-violet-600"></div>
          </div>
        )}
        
        <table className="emp-table">
          <thead>
            <tr>
              {isVisible('cccd') && <th className="emp-th w-36">Mã NV (CCCD)</th>}
              {isVisible('hoTen') && <th className="emp-th w-64">Họ tên</th>}
              {isVisible('tenChucVu') && <th className="emp-th">Chức vụ</th>}
              {isVisible('tenPhongBan') && <th className="emp-th">Phòng ban</th>}
              {isVisible('ngayVaoLam') && <th className="emp-th">Ngày vào làm</th>}
              {isVisible('trangThai') && <th className="emp-th">Trạng thái</th>}
            </tr>
          </thead>
          <tbody>
            {data.length === 0 && !isLoading ? (
              <tr>
                <td colSpan={6} className="text-center py-8 text-gray-500">Không tìm thấy dữ liệu nhân viên.</td>
              </tr>
            ) : (
              data.map((row) => (
                <tr key={row.cccd} onClick={() => onRowClick(row)} className="emp-tr">
                  {isVisible('cccd') && <td className="emp-td font-mono">{row.cccd}</td>}
                  {isVisible('hoTen') && (
                    <td className="emp-td">
                      <div className="flex items-center gap-3">
                        <div className="emp-avatar">{getInitials(row.hoTen)}</div>
                        <span className="font-medium text-gray-900 dark:text-white">{row.hoTen}</span>
                      </div>
                    </td>
                  )}
                  {isVisible('tenChucVu') && <td className="emp-td">{row.tenChucVu || '—'}</td>}
                  {isVisible('tenPhongBan') && <td className="emp-td">{row.tenPhongBan || '—'}</td>}
                  {isVisible('ngayVaoLam') && <td className="emp-td">{row.ngayVaoLam || '—'}</td>}
                  {isVisible('trangThai') && (
                    <td className="emp-td">
                      <span className={`status-badge ${row.trangThai === 'DANG_LAM_VIEC' ? 'status-active' : 'status-inactive'}`}>
                        {row.trangThai === 'DANG_LAM_VIEC' ? 'Đang làm việc' : 'Đã nghỉ'}
                      </span>
                    </td>
                  )}
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>

      {/* Pagination Footer */}
      <div className="px-4 py-3 border-t border-gray-100 dark:border-gray-700 flex items-center justify-between bg-gray-50 dark:bg-gray-800/50 flex-shrink-0">
        <span className="text-sm text-gray-500 dark:text-gray-400">
          Tổng số <span className="font-medium text-gray-900 dark:text-white">{totalRecords}</span> nhân viên
        </span>
        <div className="flex gap-2">
          <button 
            disabled={pageNumber === 1 || isLoading}
            onClick={() => onPageChange(pageNumber - 1)}
            className="px-3 py-1.5 text-sm font-medium rounded-md border border-gray-200 dark:border-gray-600 hover:bg-gray-100 dark:hover:bg-gray-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
          >
            Trước
          </button>
          <span className="px-3 py-1.5 text-sm font-medium">Trang {pageNumber} / {totalPages}</span>
          <button 
            disabled={pageNumber >= totalPages || isLoading}
            onClick={() => onPageChange(pageNumber + 1)}
            className="px-3 py-1.5 text-sm font-medium rounded-md border border-gray-200 dark:border-gray-600 hover:bg-gray-100 dark:hover:bg-gray-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
          >
            Sau
          </button>
        </div>
      </div>
    </div>
  );
};