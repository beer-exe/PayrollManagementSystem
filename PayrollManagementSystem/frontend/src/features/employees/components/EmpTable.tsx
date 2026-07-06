import React from 'react';
import { Empty } from 'antd';
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
    <div className="emp-card flex flex-col h-full w-full min-w-0 overflow-hidden bg-white dark:bg-gray-800 rounded-xl shadow-sm border border-gray-100 dark:border-gray-700">
      
      <div className="emp-toolbar z-10 flex-shrink-0 p-4 border-b border-gray-100 dark:border-gray-700 flex flex-col sm:flex-row gap-4 justify-between items-center bg-white dark:bg-gray-800">
        <div className="emp-search-box relative w-full sm:max-w-md">
          <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" className="w-5 h-5 text-gray-400">
              <path strokeLinecap="round" strokeLinejoin="round" d="M21 21l-5.197-5.197m0 0A7.5 7.5 0 105.196 5.196a7.5 7.5 0 0010.607 10.607z" />
            </svg>
          </div>
          <input 
            type="text" 
            placeholder="Tìm kiếm theo CCCD, họ tên..." 
            className="emp-search-input w-full pl-10 pr-4 py-2 border border-gray-200 dark:border-gray-600 rounded-lg focus:ring-2 focus:ring-violet-500 outline-none bg-gray-50 dark:bg-gray-700 dark:text-white" 
            value={searchTerm}
            onChange={(e) => onSearchChange(e.target.value)}
          />
        </div>
        
        <div className="flex gap-2 w-full sm:w-auto">
          <button 
            onClick={onExportExcel} 
            disabled={isExporting}
            className="emp-btn-outline w-full sm:w-auto flex items-center justify-center gap-2 px-4 py-2 border border-gray-200 dark:border-gray-600 rounded-lg text-gray-700 dark:text-gray-200 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {isExporting ? (
              <div className="w-5 h-5 border-2 border-emerald-600 border-t-transparent rounded-full animate-spin"></div>
            ) : (
              <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" className="w-5 h-5 text-emerald-600"><path strokeLinecap="round" strokeLinejoin="round" d="M19.5 14.25v-2.625a3.375 3.375 0 00-3.375-3.375h-1.5A1.125 1.125 0 0113.5 7.125v-1.5a3.375 3.375 0 00-3.375-3.375H8.25m.75 12l3 3m0 0l3-3m-3 3v-6m-1.5-9H5.625c-.621 0-1.125.504-1.125 1.125v17.25c0 .621.504 1.125 1.125 1.125h12.75c.621 0 1.125-.504 1.125-1.125V11.25a9 9 0 00-9-9z" /></svg>
            )}
            {isExporting ? 'Đang xuất...' : 'Xuất Excel'}
          </button>
          <button onClick={onOpenSettings} className="emp-btn-icon flex items-center justify-center p-2 border border-gray-200 dark:border-gray-600 rounded-lg text-gray-500 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors" title="Tùy chỉnh hiển thị cột">
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-5 h-5"><path strokeLinecap="round" strokeLinejoin="round" d="M9 4.5v15m6-15v15m-10.5-6h15m-15-6h15m-3-4.5h3.75c.621 0 1.125.504 1.125 1.125v15.75c0 .621-.504 1.125-1.125 1.125H3.75c-.621 0-1.125-.504-1.125-1.125V5.625c0-.621.504-1.125 1.125-1.125H6.75Z" /></svg>
          </button>
        </div>
      </div>

      <div className="grid grid-cols-1 w-full flex-1 min-h-0 relative">
        <div className="w-full h-full overflow-auto relative bg-white dark:bg-gray-800">
          
          {isLoading && (
            <div className="absolute inset-0 z-20 flex items-center justify-center bg-white/60 dark:bg-gray-800/60 backdrop-blur-[1px]">
              <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-violet-600"></div>
            </div>
          )}
          
          <table className="emp-table w-full min-w-max border-collapse text-sm text-left">
            <thead className="text-xs text-gray-500 uppercase bg-gray-50 dark:bg-gray-700/50 dark:text-gray-400 sticky top-0 z-10 shadow-sm">
              <tr>
                {isVisible('cccd') && <th className="px-6 py-4 font-semibold whitespace-nowrap">Mã NV (CCCD)</th>}
                {isVisible('hoTen') && <th className="px-6 py-4 font-semibold whitespace-nowrap">Họ tên</th>}
                {isVisible('tenChucVu') && <th className="px-6 py-4 font-semibold whitespace-nowrap">Chức vụ</th>}
                {isVisible('tenPhongBan') && <th className="px-6 py-4 font-semibold whitespace-nowrap">Phòng ban</th>}
                {isVisible('ngayVaoLam') && <th className="px-6 py-4 font-semibold whitespace-nowrap">Ngày vào làm</th>}
                {isVisible('trangThai') && <th className="px-6 py-4 font-semibold whitespace-nowrap">Trạng thái</th>}
                <th className="px-6 py-4 font-semibold text-right whitespace-nowrap">Hành động</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100 dark:divide-gray-700/50">
              {data.length === 0 && !isLoading ? (
                <tr>
                  <td colSpan={7} className="py-16 text-center">
                    <Empty description={<span className="text-gray-500">Không tìm thấy nhân viên nào</span>} image={Empty.PRESENTED_IMAGE_SIMPLE} />
                  </td>
                </tr>
              ) : (
                data.map((row) => (
                  <tr key={row.cccd} onClick={() => onRowClick(row)} className="emp-tr group hover:bg-gray-50 dark:hover:bg-gray-700/50 cursor-pointer transition-colors">
                    {isVisible('cccd') && <td className="px-6 py-4 font-mono text-gray-500 dark:text-gray-400">{row.cccd}</td>}
                    {isVisible('hoTen') && (
                      <td className="px-6 py-4">
                        <div className="flex items-center gap-3">
                          <div className="w-8 h-8 rounded-full bg-violet-100 dark:bg-violet-900/50 text-violet-600 dark:text-violet-400 flex items-center justify-center font-bold text-xs flex-shrink-0">
                            {getInitials(row.hoTen)}
                          </div>
                          <span className="font-semibold text-gray-900 dark:text-white whitespace-nowrap">{row.hoTen}</span>
                        </div>
                      </td>
                    )}
                    {isVisible('tenChucVu') && <td className="px-6 py-4 font-medium text-gray-700 dark:text-gray-300 whitespace-nowrap">{row.tenChucVu || '—'}</td>}
                    {isVisible('tenPhongBan') && <td className="px-6 py-4 text-gray-600 dark:text-gray-400 whitespace-nowrap">{row.tenPhongBan || '—'}</td>}
                    {isVisible('ngayVaoLam') && <td className="px-6 py-4 text-gray-600 dark:text-gray-400 whitespace-nowrap">{row.ngayVaoLam || '—'}</td>}
                    {isVisible('trangThai') && (
                      <td className="px-6 py-4 whitespace-nowrap">
                        <span className={`inline-flex items-center px-2.5 py-1 rounded-full text-xs font-medium border ${
                          row.trangThai === 'DANG_LAM_VIEC' 
                            ? 'bg-emerald-50 text-emerald-700 border-emerald-200 dark:bg-emerald-500/10 dark:text-emerald-400 dark:border-emerald-500/20' 
                            : 'bg-rose-50 text-rose-700 border-rose-200 dark:bg-rose-500/10 dark:text-rose-400 dark:border-rose-500/20'
                        }`}>
                          <span className={`w-1.5 h-1.5 rounded-full mr-1.5 ${row.trangThai === 'DANG_LAM_VIEC' ? 'bg-emerald-500' : 'bg-rose-500'}`}></span>
                          {row.trangThai === 'DANG_LAM_VIEC' ? 'Đang làm việc' : 'Đã nghỉ'}
                        </span>
                      </td>
                    )}
                    <td className="px-6 py-4 text-right whitespace-nowrap">
                      <button 
                        onClick={(e) => { e.stopPropagation(); onEditClick(row); }} 
                        className="text-violet-600 hover:text-violet-800 dark:text-violet-400 dark:hover:text-violet-300 font-medium text-sm mr-4 transition-colors"
                      >
                        Sửa
                      </button>
                      <button 
                        onClick={(e) => { e.stopPropagation(); onStatusClick(row); }} 
                        className="text-amber-600 hover:text-amber-800 dark:text-amber-500 dark:hover:text-amber-400 font-medium text-sm transition-colors"
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
      </div>

      <div className="px-5 py-3.5 border-t border-gray-100 dark:border-gray-700 flex items-center justify-between bg-gray-50 dark:bg-gray-800/50 flex-shrink-0 z-10">
        <span className="text-sm text-gray-500 dark:text-gray-400 hidden sm:inline-block">
          Tổng số <span className="font-bold text-gray-900 dark:text-white">{totalRecords}</span> nhân viên
        </span>
        <div className="flex gap-2 w-full sm:w-auto justify-between sm:justify-end items-center">
          <button 
            disabled={pageNumber === 1 || isLoading} 
            onClick={() => onPageChange(pageNumber - 1)} 
            className="px-3 py-1.5 text-sm font-medium bg-white dark:bg-gray-800 rounded-md border border-gray-200 dark:border-gray-700 hover:bg-gray-50 dark:hover:bg-gray-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors text-gray-700 dark:text-gray-300 shadow-sm"
          >
            Trước
          </button>
          <span className="px-3 py-1.5 text-sm font-medium text-gray-700 dark:text-gray-300">
            Trang <span className="font-semibold text-violet-600 dark:text-violet-400">{pageNumber}</span> / {totalPages}
          </span>
          <button 
            disabled={pageNumber >= totalPages || isLoading} 
            onClick={() => onPageChange(pageNumber + 1)} 
            className="px-3 py-1.5 text-sm font-medium bg-white dark:bg-gray-800 rounded-md border border-gray-200 dark:border-gray-700 hover:bg-gray-50 dark:hover:bg-gray-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors text-gray-700 dark:text-gray-300 shadow-sm"
          >
            Sau
          </button>
        </div>
      </div>

    </div>
  );
};