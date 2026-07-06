import React from 'react';
import { Drawer, Checkbox } from 'antd';
import './EmployeeModals.css';

interface Props {
  open: boolean;
  onClose: () => void;
  visibleColumns: string[];
  onChange: (columns: string[]) => void;
}

const availableColumns = [
  { key: 'cccd', label: 'Mã NV (CCCD)', required: true },
  { key: 'hoTen', label: 'Họ tên', required: false },
  { key: 'tenChucVu', label: 'Chức vụ', required: false },
  { key: 'tenPhongBan', label: 'Phòng ban', required: false },
  { key: 'ngayVaoLam', label: 'Ngày vào làm', required: false },
  { key: 'trangThai', label: 'Trạng thái', required: false },
];

export const ColumnSetupDrawer: React.FC<Props> = ({ open, onClose, visibleColumns, onChange }) => {
  
  const handleToggle = (key: string, checked: boolean) => {
    if (key === 'cccd') return; // Chặn toggle nếu là cột bắt buộc

    if (checked) {
      onChange([...visibleColumns, key]);
    } else {
      onChange(visibleColumns.filter((col) => col !== key));
    }
  };

  return (
    <Drawer
      title={<span className="text-gray-900 dark:text-white font-bold tracking-tight text-lg">Tùy chỉnh hiển thị</span>}
      placement="right"
      onClose={onClose}
      open={open}
      width={340}
      /* Custom Icon nút X (Đóng) */
      closeIcon={
        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" className="w-5 h-5 text-gray-500 hover:text-gray-800 transition-colors">
          <path strokeLinecap="round" strokeLinejoin="round" d="M6 18L18 6M6 6l12 12" />
        </svg>
      }
      footer={
        <div className="p-2">
          <button onClick={onClose} className="emp-btn-submit w-full py-2.5 text-base">
            Hoàn tất
          </button>
        </div>
      }
      styles={{
        body: { padding: '20px' },
        header: { borderBottom: '1px solid #f3f4f6' }
      }}
    >
      <div className="flex flex-col h-full">
        <p className="text-sm text-gray-500 dark:text-gray-400 mb-6 leading-relaxed">
          Lựa chọn các cột dữ liệu bạn muốn hiển thị trên bảng. Cột Mã NV là thông tin bắt buộc.
        </p>

        <div className="space-y-3">
          {availableColumns.map((col) => {
            const isChecked = visibleColumns.includes(col.key);
            
            return (
              <label
                key={col.key}
                className={`flex items-center justify-between p-3.5 rounded-xl border cursor-pointer transition-all ${
                  col.required
                    ? 'bg-gray-50 border-gray-200 dark:bg-gray-800/50 dark:border-gray-700'
                    : isChecked
                    ? 'bg-violet-50 border-violet-200 dark:bg-violet-900/20 dark:border-violet-800'
                    : 'bg-white border-gray-100 hover:border-violet-300 shadow-sm hover:shadow-md dark:bg-gray-800 dark:border-gray-700 dark:hover:border-violet-500'
                }`}
              >
                <div className="flex items-center gap-3">
                  <Checkbox
                    checked={col.required ? true : isChecked}
                    disabled={col.required}
                    onChange={(e) => handleToggle(col.key, e.target.checked)}
                  />
                  <span className={`text-sm font-semibold ${col.required ? 'text-gray-400 dark:text-gray-500' : 'text-gray-700 dark:text-gray-200'}`}>
                    {col.label}
                  </span>
                </div>

                {/* Nếu là cột bắt buộc (CCCD), hiển thị ổ khóa */}
                {col.required && (
                  <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" className="w-4 h-4 text-gray-400">
                    <path fillRule="evenodd" d="M10 1a4.5 4.5 0 00-4.5 4.5V9H5a2 2 0 00-2 2v6a2 2 0 002 2h10a2 2 0 002-2v-6a2 2 0 00-2-2h-.5V5.5A4.5 4.5 0 0010 1zm3 8V5.5a3 3 0 10-6 0V9h6z" clipRule="evenodd" />
                  </svg>
                )}
              </label>
            );
          })}
        </div>
      </div>
    </Drawer>
  );
};