import React, { useState } from 'react';
import { UserProfileDetail } from '@/types/profile.types';

interface Props {
  employee: UserProfileDetail | null;
  isOpen: boolean;
  onClose: () => void;
}

export const EmployeeDetailPanel: React.FC<Props> = ({ employee, isOpen, onClose }) => {
  const [tabIndex, setTabIndex] = useState(0);

  const getInitials = (name: string) => {
    if (!name) return 'U';
    const parts = name.trim().split(' ');
    if (parts.length === 1) return parts[0].charAt(0).toUpperCase();
    return (parts[0].charAt(0) + parts[parts.length - 1].charAt(0)).toUpperCase();
  };

  return (
    <div className={`slide-panel ${isOpen ? 'open' : 'closed'}`}>
      {/* Header (Sticky) */}
      <div className="p-6 border-b border-gray-100 dark:border-gray-700 sticky top-0 bg-white/90 dark:bg-gray-800/90 backdrop-blur-md z-10">
        <button onClick={onClose} className="btn-icon absolute top-4 right-4">
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-5 h-5"><path strokeLinecap="round" strokeLinejoin="round" d="M6 18 18 6M6 6l12 12" /></svg>
        </button>
        <h2 className="text-2xl font-bold text-gray-900 dark:text-white mb-2 pr-8 truncate">
          {employee?.hoTen || 'Đang tải...'}
        </h2>
        <div className="flex gap-2">
          <span className="px-2.5 py-0.5 rounded-md text-xs font-medium border border-gray-200 dark:border-gray-600 text-gray-600 dark:text-gray-300">
            Mã NV: {employee?.cccd}
          </span>
          <span className="px-2.5 py-0.5 rounded-md text-xs font-medium bg-violet-50 text-violet-700 dark:bg-violet-900/30 dark:text-violet-400">
            {employee?.tenChucVu || 'Chưa cập nhật'}
          </span>
        </div>
      </div>

      {/* Tabs */}
      <div className="flex border-b border-gray-100 dark:border-gray-700 px-6">
        <button 
          onClick={() => setTabIndex(0)} 
          className={`px-4 py-3 text-sm font-medium border-b-2 transition-colors ${tabIndex === 0 ? 'border-violet-600 text-violet-600 dark:border-violet-400 dark:text-violet-400' : 'border-transparent text-gray-500 hover:text-gray-700 dark:text-gray-400'}`}
        >
          Hồ sơ
        </button>
        <button 
          onClick={() => setTabIndex(1)} 
          className={`px-4 py-3 text-sm font-medium border-b-2 transition-colors ${tabIndex === 1 ? 'border-violet-600 text-violet-600 dark:border-violet-400 dark:text-violet-400' : 'border-transparent text-gray-500 hover:text-gray-700 dark:text-gray-400'}`}
        >
          Thân nhân
        </button>
      </div>

      {/* Scrollable Content */}
      <div className="flex-1 overflow-y-auto p-6 bg-gray-50 dark:bg-gray-900/50">
        {tabIndex === 0 && employee && (
          <div>
            <div className="flex justify-center mb-6">
              <div className="flex items-center justify-center w-24 h-24 rounded-full bg-violet-100 text-violet-700 dark:bg-violet-900/40 dark:text-violet-400 text-3xl font-bold shadow-sm">
                {getInitials(employee.hoTen)}
              </div>
            </div>
            
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-6">
              <div className="flex flex-col space-y-1">
                <span className="text-xs font-semibold uppercase text-gray-500 dark:text-gray-400 tracking-wider">Email</span>
                <span className="text-sm font-medium text-gray-900 dark:text-gray-100">{employee.email || '—'}</span>
              </div>
              <div className="flex flex-col space-y-1">
                <span className="text-xs font-semibold uppercase text-gray-500 dark:text-gray-400 tracking-wider">Số điện thoại</span>
                <span className="text-sm font-medium text-gray-900 dark:text-gray-100">{employee.sdt || '—'}</span>
              </div>
              <div className="flex flex-col space-y-1">
                <span className="text-xs font-semibold uppercase text-gray-500 dark:text-gray-400 tracking-wider">Ngày sinh</span>
                <span className="text-sm font-medium text-gray-900 dark:text-gray-100">{employee.ngaySinh || '—'}</span>
              </div>
              <div className="flex flex-col space-y-1">
                <span className="text-xs font-semibold uppercase text-gray-500 dark:text-gray-400 tracking-wider">Phòng ban</span>
                <span className="text-sm font-medium text-gray-900 dark:text-gray-100">{employee.tenPhongBan || '—'}</span>
              </div>
              <div className="col-span-1 sm:col-span-2 flex flex-col space-y-1">
                <span className="text-xs font-semibold uppercase text-gray-500 dark:text-gray-400 tracking-wider">Địa chỉ</span>
                <span className="text-sm font-medium text-gray-900 dark:text-gray-100">{employee.diaChi || '—'}</span>
              </div>
            </div>
          </div>
        )}

        {tabIndex === 1 && (
          <div className="flex flex-col space-y-4">
            {(employee?.thanNhans?.length ?? 0) > 0 ? (
              employee!.thanNhans.map((tn, index) => (
                <div key={index} className="bg-white dark:bg-gray-800 p-4 rounded-xl border border-gray-100 dark:border-gray-700 shadow-sm">
                  <h4 className="text-base font-bold text-gray-900 dark:text-white">
                    {tn.tenTn} ({tn.moiQuanHe})
                  </h4>
                    <div className="mt-2 flex flex-col space-y-1">
                      <span className="text-xs text-gray-500 dark:text-gray-400">Năm sinh</span>
                      <span className="text-sm font-medium text-gray-700 dark:text-gray-300">
                        {tn.ngaySinh || 'Chưa cập nhật'}
                      </span>
                    </div>
                </div>
              ))
            ) : (
              <p className="text-center text-gray-500 py-4">Không có thông tin thân nhân.</p>
            )}
          </div>
        )}
      </div>
    </div>
  );
};