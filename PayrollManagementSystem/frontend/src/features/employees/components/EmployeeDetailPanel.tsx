import React from 'react';
import { Drawer, Tabs, Empty } from 'antd';
import { UserProfileDetail } from '@/types/profile.types';

interface Props {
  employee: UserProfileDetail | null;
  isOpen: boolean;
  onClose: () => void;
}

export const EmployeeDetailPanel: React.FC<Props> = ({ employee, isOpen, onClose }) => {
  const getInitials = (name: string) => {
    if (!name) return 'U';
    const parts = name.trim().split(' ');
    if (parts.length === 1) return parts[0].charAt(0).toUpperCase();
    return (parts[0].charAt(0) + parts[parts.length - 1].charAt(0)).toUpperCase();
  };

  const formatDate = (dateString: string | null) => {
    if (!dateString) return '—';
    const date = new Date(dateString);
    return new Intl.DateTimeFormat('vi-VN').format(date);
  };

  const drawerTitle = employee ? (
    <div className="flex items-center gap-4 py-1">
      <div className="flex items-center justify-center w-14 h-14 rounded-full bg-violet-100 text-violet-700 dark:bg-violet-900/40 dark:text-violet-400 text-xl font-bold shadow-sm shrink-0 border border-violet-200 dark:border-violet-800">
        {getInitials(employee.hoTen)}
      </div>
      <div className="flex flex-col min-w-0">
        <h2 className="text-lg sm:text-xl font-bold text-gray-900 dark:text-white truncate tracking-tight">
          {employee.hoTen}
        </h2>
        <div className="flex items-center gap-2 mt-1 flex-wrap">
          <span className="text-xs font-mono bg-gray-100 text-gray-600 px-2 py-0.5 rounded dark:bg-gray-800 dark:text-gray-300 border border-gray-200 dark:border-gray-700">
            {employee.cccd}
          </span>
          <span className="text-xs font-medium text-violet-700 bg-violet-50 border border-violet-100 px-2 py-0.5 rounded dark:text-violet-400 dark:bg-violet-900/30 dark:border-violet-800/50">
            {employee.tenChucVu || 'Chưa cập nhật'}
          </span>
        </div>
      </div>
    </div>
  ) : 'Đang tải thông tin...';

  const tabItems = [
    {
      key: '1',
      label: <span className="px-2 font-medium">Hồ sơ cá nhân</span>,
      children: employee ? (
        <div className="px-5 pb-8 space-y-6 animate-[fadeIn_0.3s_ease-out]">
          
          {/* Nhóm Thông tin liên hệ */}
          <div>
            <h3 className="text-xs font-bold text-gray-400 dark:text-gray-500 uppercase tracking-wider mb-3 px-1">Liên hệ</h3>
            <div className="bg-white dark:bg-gray-800 rounded-xl border border-gray-100 dark:border-gray-700 overflow-hidden shadow-sm">
              <div className="flex flex-col divide-y divide-gray-50 dark:divide-gray-700/50">
                <div className="p-3.5 flex justify-between items-center hover:bg-gray-50 dark:hover:bg-gray-800/80 transition-colors">
                  <span className="text-sm text-gray-500 flex items-center gap-2">
                    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-4 h-4"><path strokeLinecap="round" strokeLinejoin="round" d="M21.75 6.75v10.5a2.25 2.25 0 01-2.25 2.25h-15a2.25 2.25 0 01-2.25-2.25V6.75m19.5 0A2.25 2.25 0 0019.5 4.5h-15a2.25 2.25 0 00-2.25 2.25m19.5 0v.243a2.25 2.25 0 01-1.07 1.916l-7.5 4.615a2.25 2.25 0 01-2.36 0L3.32 8.91a2.25 2.25 0 01-1.07-1.916V6.75" /></svg>
                    Email
                  </span>
                  <span className="text-sm font-medium text-gray-900 dark:text-white">{employee.email || '—'}</span>
                </div>
                <div className="p-3.5 flex justify-between items-center hover:bg-gray-50 dark:hover:bg-gray-800/80 transition-colors">
                  <span className="text-sm text-gray-500 flex items-center gap-2">
                    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-4 h-4"><path strokeLinecap="round" strokeLinejoin="round" d="M2.25 6.75c0 8.284 6.716 15 15 15h2.25a2.25 2.25 0 002.25-2.25v-1.372c0-.516-.351-.966-.852-1.091l-4.423-1.106c-.44-.11-.902.055-1.173.417l-.97 1.293c-2.896-1.596-5.48-4.18-7.076-7.076l1.293-.97c.362-.271.527-.734.417-1.173L6.963 3.102a1.125 1.125 0 00-1.091-.852H4.5A2.25 2.25 0 002.25 4.5v2.25z" /></svg>
                    Số điện thoại
                  </span>
                  <span className="text-sm font-medium text-gray-900 dark:text-white">{employee.sdt || '—'}</span>
                </div>
                <div className="p-3.5 flex justify-between items-start hover:bg-gray-50 dark:hover:bg-gray-800/80 transition-colors">
                  <span className="text-sm text-gray-500 flex items-center gap-2 shrink-0">
                    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-4 h-4"><path strokeLinecap="round" strokeLinejoin="round" d="M15 10.5a3 3 0 11-6 0 3 3 0 016 0z" /><path strokeLinecap="round" strokeLinejoin="round" d="M19.5 10.5c0 7.142-7.5 11.25-7.5 11.25S4.5 17.642 4.5 10.5a7.5 7.5 0 1115 0z" /></svg>
                    Địa chỉ
                  </span>
                  <span className="text-sm font-medium text-gray-900 dark:text-white text-right ml-4 leading-relaxed">{employee.diaChi || '—'}</span>
                </div>
              </div>
            </div>
          </div>

          {/* Nhóm Thông tin cơ bản */}
          <div>
            <h3 className="text-xs font-bold text-gray-400 dark:text-gray-500 uppercase tracking-wider mb-3 px-1">Lý lịch</h3>
            <div className="bg-white dark:bg-gray-800 rounded-xl border border-gray-100 dark:border-gray-700 overflow-hidden shadow-sm">
              <div className="flex flex-col divide-y divide-gray-50 dark:divide-gray-700/50">
                <div className="p-3.5 flex justify-between items-center hover:bg-gray-50 dark:hover:bg-gray-800/80 transition-colors">
                  <span className="text-sm text-gray-500">Ngày sinh</span>
                  <span className="text-sm font-medium text-gray-900 dark:text-white">{formatDate(employee.ngaySinh)}</span>
                </div>
                <div className="p-3.5 flex justify-between items-center hover:bg-gray-50 dark:hover:bg-gray-800/80 transition-colors">
                  <span className="text-sm text-gray-500">Giới tính</span>
                  <span className="text-sm font-medium text-gray-900 dark:text-white">
                    {employee.gioiTinh === true ? 'Nam' : employee.gioiTinh === false ? 'Nữ' : '—'}
                  </span>
                </div>
                <div className="p-3.5 flex justify-between items-center hover:bg-gray-50 dark:hover:bg-gray-800/80 transition-colors">
                  <span className="text-sm text-gray-500">Dân tộc</span>
                  <span className="text-sm font-medium text-gray-900 dark:text-white">{employee.danToc || '—'}</span>
                </div>
              </div>
            </div>
          </div>

          {/* Nhóm Chuyên môn & Bảo hiểm */}
          <div>
            <h3 className="text-xs font-bold text-gray-400 dark:text-gray-500 uppercase tracking-wider mb-3 px-1">Công tác & BHXH</h3>
            <div className="bg-white dark:bg-gray-800 rounded-xl border border-gray-100 dark:border-gray-700 overflow-hidden shadow-sm">
              <div className="flex flex-col divide-y divide-gray-50 dark:divide-gray-700/50">
                <div className="p-3.5 flex justify-between items-center hover:bg-gray-50 dark:hover:bg-gray-800/80 transition-colors">
                  <span className="text-sm text-gray-500">Phòng ban</span>
                  <span className="text-sm font-medium text-gray-900 dark:text-white">{employee.tenPhongBan || '—'}</span>
                </div>
                <div className="p-3.5 flex justify-between items-center hover:bg-gray-50 dark:hover:bg-gray-800/80 transition-colors">
                  <span className="text-sm text-gray-500">Chuyên ngành</span>
                  <span className="text-sm font-medium text-gray-900 dark:text-white">{employee.chuyenNganh || '—'}</span>
                </div>
                <div className="p-3.5 flex justify-between items-center hover:bg-gray-50 dark:hover:bg-gray-800/80 transition-colors">
                  <span className="text-sm text-gray-500">Số BHXH</span>
                  <span className="text-sm font-medium text-gray-900 dark:text-white font-mono">{employee.soBhxh || '—'}</span>
                </div>
              </div>
            </div>
          </div>

          {/* Nhóm Tài chính & Thuế */}
          <div>
            <h3 className="text-xs font-bold text-gray-400 dark:text-gray-500 uppercase tracking-wider mb-3 px-1">Tài chính & Thuế</h3>
            <div className="bg-white dark:bg-gray-800 rounded-xl border border-gray-100 dark:border-gray-700 overflow-hidden shadow-sm">
              <div className="flex flex-col divide-y divide-gray-50 dark:divide-gray-700/50">
                <div className="p-3.5 flex justify-between items-center hover:bg-gray-50 dark:hover:bg-gray-800/80 transition-colors">
                  <span className="text-sm text-gray-500">Số tài khoản</span>
                  <span className="text-sm font-medium text-gray-900 dark:text-white font-mono">{employee.soTaiKhoan || '—'}</span>
                </div>
                <div className="p-3.5 flex justify-between items-center hover:bg-gray-50 dark:hover:bg-gray-800/80 transition-colors">
                  <span className="text-sm text-gray-500">Ngân hàng</span>
                  <span className="text-sm font-medium text-gray-900 dark:text-white">{employee.tenNganHang || '—'}</span>
                </div>
                <div className="p-3.5 flex justify-between items-center hover:bg-gray-50 dark:hover:bg-gray-800/80 transition-colors">
                  <span className="text-sm text-gray-500">Mã số thuế</span>
                  <span className="text-sm font-medium text-gray-900 dark:text-white font-mono">{employee.maSoThue || '—'}</span>
                </div>
              </div>
            </div>
          </div>

        </div>
      ) : null
    },
    {
      key: '2',
      label: <span className="px-2 font-medium">Thân nhân</span>,
      children: employee ? (
        <div className="px-5 pb-8 animate-[fadeIn_0.3s_ease-out]">
          {(employee.thanNhans?.length ?? 0) > 0 ? (
            <div className="space-y-4">
              {employee.thanNhans.map((tn, index) => (
                <div key={index} className="bg-white dark:bg-gray-800 p-4 rounded-xl border border-gray-100 dark:border-gray-700 shadow-sm flex flex-col hover:border-violet-300 dark:hover:border-violet-500 transition-colors">
                  <div className="flex justify-between items-start mb-2">
                    <h4 className="text-base font-bold text-gray-900 dark:text-white tracking-tight">
                      {tn.tenTn}
                    </h4>
                    <span className="px-2.5 py-1 bg-violet-50 text-violet-700 dark:bg-violet-900/30 dark:text-violet-400 text-xs font-semibold rounded-full border border-violet-100 dark:border-violet-800/50">
                      {tn.moiQuanHe || 'Không rõ'}
                    </span>
                  </div>
                  <div className="flex justify-between items-center pt-3 border-t border-gray-50 dark:border-gray-700/50 mt-1">
                    <span className="text-xs text-gray-500 flex items-center gap-1.5">
                      <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-4 h-4"><path strokeLinecap="round" strokeLinejoin="round" d="M6.75 3v2.25M17.25 3v2.25M3 18.75V7.5a2.25 2.25 0 012.25-2.25h13.5A2.25 2.25 0 0121 7.5v11.25m-18 0A2.25 2.25 0 005.25 21h13.5A2.25 2.25 0 0021 18.75m-18 0v-7.5A2.25 2.25 0 015.25 9h13.5A2.25 2.25 0 0121 11.25v7.5" /></svg>
                      Năm sinh
                    </span>
                    <span className="text-sm font-medium text-gray-800 dark:text-gray-200">
                      {formatDate(tn.ngaySinh) || '—'}
                    </span>
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <div className="mt-8">
              <Empty 
                description={<span className="text-gray-500 font-medium">Chưa có thông tin thân nhân</span>} 
                image={Empty.PRESENTED_IMAGE_SIMPLE} 
              />
            </div>
          )}
        </div>
      ) : null
    },
    {
      key: '3',
      label: <span className="px-2 font-medium">Lương & HĐ</span>,
      children: employee ? (
        <div className="px-5 pb-8 space-y-6 animate-[fadeIn_0.3s_ease-out]">
          
          <div>
            <h3 className="text-xs font-bold text-gray-400 dark:text-gray-500 uppercase tracking-wider mb-3 px-1">Lương 3P</h3>
            <div className="bg-white dark:bg-gray-800 rounded-xl border border-gray-100 dark:border-gray-700 overflow-hidden shadow-sm">
              <div className="flex flex-col divide-y divide-gray-50 dark:divide-gray-700/50">
                <div className="p-3.5 flex justify-between items-center hover:bg-gray-50 dark:hover:bg-gray-800/80 transition-colors">
                  <span className="text-sm text-gray-500">Lương P1 (Vị trí)</span>
                  <span className="text-sm font-medium text-violet-700 dark:text-violet-400">
                    {employee.luongP1 ? new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(employee.luongP1) : 'Chưa cập nhật'}
                  </span>
                </div>
                <div className="p-3.5 flex justify-between items-center hover:bg-gray-50 dark:hover:bg-gray-800/80 transition-colors">
                  <span className="text-sm text-gray-500">Hệ số P2 (Năng lực)</span>
                  <span className="text-sm font-medium text-amber-600 dark:text-amber-400">
                    {employee.heSoP2 ? `${employee.heSoP2}` : '1.00'}
                  </span>
                </div>
              </div>
            </div>
          </div>

          <div>
            <h3 className="text-xs font-bold text-gray-400 dark:text-gray-500 uppercase tracking-wider mb-3 px-1">Hợp đồng hiện tại</h3>
            <div className="bg-white dark:bg-gray-800 rounded-xl border border-gray-100 dark:border-gray-700 overflow-hidden shadow-sm">
              <div className="flex flex-col divide-y divide-gray-50 dark:divide-gray-700/50">
                <div className="p-3.5 flex justify-between items-center hover:bg-gray-50 dark:hover:bg-gray-800/80 transition-colors">
                  <span className="text-sm text-gray-500">Số HĐ</span>
                  <span className="text-sm font-medium text-gray-900 dark:text-white font-mono">{employee.soHopDong || '—'}</span>
                </div>
                <div className="p-3.5 flex justify-between items-center hover:bg-gray-50 dark:hover:bg-gray-800/80 transition-colors">
                  <span className="text-sm text-gray-500">Loại HĐ</span>
                  <span className="text-sm font-medium text-gray-900 dark:text-white">{employee.loaiHopDong || '—'}</span>
                </div>
                <div className="p-3.5 flex justify-between items-center hover:bg-gray-50 dark:hover:bg-gray-800/80 transition-colors">
                  <span className="text-sm text-gray-500">Ngày bắt đầu</span>
                  <span className="text-sm font-medium text-gray-900 dark:text-white">{formatDate(employee.ngayBatDauHopDong ?? null)}</span>
                </div>
              </div>
            </div>
          </div>

        </div>
      ) : null
    }
  ];

  return (
    <Drawer
      title={drawerTitle}
      placement="right"
      onClose={onClose}
      open={isOpen}
      width={typeof window !== 'undefined' && window.innerWidth < 576 ? '100%' : 450}
      closeIcon={
        <div className="p-1.5 bg-gray-50 dark:bg-gray-800 hover:bg-gray-100 dark:hover:bg-gray-700 rounded-full transition-colors border border-gray-200 dark:border-gray-700">
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" className="w-5 h-5 text-gray-500">
            <path strokeLinecap="round" strokeLinejoin="round" d="M6 18L18 6M6 6l12 12" />
          </svg>
        </div>
      }
      styles={{
        body: { padding: 0, backgroundColor: '#f9fafb' },
        header: { padding: '16px 20px', borderBottom: '1px solid #f3f4f6' }
      }}
      className="dark:bg-gray-900"
    >
      <Tabs 
        defaultActiveKey="1" 
        items={tabItems} 
        className="emp-detail-tabs"
        tabBarStyle={{ padding: '0 20px', marginBottom: '16px', backgroundColor: '#fff' }}
      />
    </Drawer>
  );
};