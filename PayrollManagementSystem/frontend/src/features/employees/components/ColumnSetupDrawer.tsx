import React from 'react';

interface Props {
  open: boolean;
  onClose: () => void;
  visibleColumns: string[];
  onChange: (columns: string[]) => void;
}

const availableColumns = [
  { key: 'cccd', label: 'Mã NV (CCCD)' },
  { key: 'hoTen', label: 'Họ tên' },
  { key: 'tenChucVu', label: 'Chức vụ' },
  { key: 'tenPhongBan', label: 'Phòng ban' },
  { key: 'ngayVaoLam', label: 'Ngày vào làm' },
  { key: 'trangThai', label: 'Trạng thái' },
];

export const ColumnSetupDrawer: React.FC<Props> = ({ open, onClose, visibleColumns, onChange }) => {
  const handleToggle = (key: string) => {
    const currentIndex = visibleColumns.indexOf(key);
    const newColumns = [...visibleColumns];

    if (currentIndex === -1) {
      newColumns.push(key);
    } else {
      newColumns.splice(currentIndex, 1);
    }
    
    if (key === 'cccd' && currentIndex !== -1) return;
    onChange(newColumns);
  };

  return (
    <>
      {/* Overlay */}
      <div className={`drawer-overlay ${open ? 'open' : 'closed'}`} onClick={onClose} />

      {/* Drawer */}
      <div className={`drawer-panel ${open ? 'open' : 'closed'}`}>
        <div className="flex items-center justify-between p-4 border-b border-gray-100 dark:border-gray-700">
          <h3 className="text-lg font-semibold text-gray-900 dark:text-white">Tùy chỉnh cột</h3>
          <button onClick={onClose} className="btn-icon">
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-5 h-5"><path strokeLinecap="round" strokeLinejoin="round" d="M6 18 18 6M6 6l12 12" /></svg>
          </button>
        </div>
        
        <div className="p-4 flex-1 overflow-y-auto">
          <p className="text-sm text-gray-500 dark:text-gray-400 mb-4">
            Chọn các cột bạn muốn hiển thị trên bảng dữ liệu.
          </p>
          <div className="flex flex-col space-y-3">
            {availableColumns.map((col) => (
              <label key={col.key} className="flex items-center cursor-pointer">
                <input 
                  type="checkbox" 
                  checked={visibleColumns.includes(col.key)}
                  onChange={() => handleToggle(col.key)}
                  disabled={col.key === 'cccd'}
                  className="w-4 h-4 text-violet-600 border-gray-300 rounded focus:ring-violet-500 disabled:opacity-50"
                />
                <span className="ml-3 text-sm font-medium text-gray-700 dark:text-gray-200">{col.label}</span>
              </label>
            ))}
          </div>
        </div>
        
        <div className="p-4 border-t border-gray-100 dark:border-gray-700">
          <button onClick={onClose} className="w-full py-2 px-4 bg-violet-600 hover:bg-violet-700 text-white font-medium rounded-md transition-colors focus:outline-none focus:ring-2 focus:ring-violet-500 focus:ring-offset-2">
            Hoàn tất
          </button>
        </div>
      </div>
    </>
  );
};