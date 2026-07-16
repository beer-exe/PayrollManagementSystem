import React from 'react';
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

  if (!open) return null;

  return (
    <div className="emp-drawer-overlay" onClick={onClose}>
      <div className="emp-drawer" onClick={e => e.stopPropagation()} style={{ maxWidth: '350px' }}>
        <div className="emp-drawer-header">
          <h2 style={{ fontSize: '1.15rem', fontWeight: 700, margin: 0, color: 'var(--text-primary)' }}>Tùy chỉnh hiển thị</h2>
          <button className="emp-drawer-close" onClick={onClose}>
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '1.25rem', height: '1.25rem' }}>
              <path strokeLinecap="round" strokeLinejoin="round" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>
        
        <div className="emp-drawer-body custom-scrollbar" style={{ padding: '1.25rem' }}>
          <p style={{ fontSize: '0.85rem', color: 'var(--text-secondary)', marginBottom: '1.5rem', lineHeight: 1.5 }}>
            Lựa chọn các cột dữ liệu bạn muốn hiển thị trên bảng. Cột Mã NV là thông tin bắt buộc.
          </p>

          <div style={{ display: 'flex', flexDirection: 'column', gap: '0.75rem' }}>
            {availableColumns.map((col) => {
              const isChecked = visibleColumns.includes(col.key);
              
              let bg = 'var(--bg-surface)';
              let border = 'var(--border-color)';
              if (col.required) { bg = 'var(--bg-main)'; border = 'var(--border-color)'; }
              else if (isChecked) { bg = 'var(--primary-light)'; border = 'var(--primary)'; }

              return (
                <label
                  key={col.key}
                  style={{
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'space-between',
                    padding: '0.85rem 1rem',
                    borderRadius: '12px',
                    border: `1px solid ${border}`,
                    background: bg,
                    cursor: col.required ? 'not-allowed' : 'pointer',
                    transition: 'all 0.2s',
                    boxShadow: !col.required && !isChecked ? '0 1px 2px rgba(0,0,0,0.02)' : 'none'
                  }}
                  className={!col.required && !isChecked ? "hover-border-violet" : ""}
                >
                  <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem' }}>
                    <input
                      type="checkbox"
                      checked={col.required ? true : isChecked}
                      disabled={col.required}
                      onChange={(e) => handleToggle(col.key, e.target.checked)}
                      style={{ width: '1rem', height: '1rem', cursor: col.required ? 'not-allowed' : 'pointer', accentColor: '#7c3aed' }}
                    />
                    <span style={{ fontSize: '0.85rem', fontWeight: 600, color: col.required ? 'var(--text-muted)' : 'var(--text-primary)' }}>
                      {col.label}
                    </span>
                  </div>

                  {col.required && (
                    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" style={{ width: '1rem', height: '1rem', color: 'var(--text-muted)' }}>
                      <path fillRule="evenodd" d="M10 1a4.5 4.5 0 00-4.5 4.5V9H5a2 2 0 00-2 2v6a2 2 0 002 2h10a2 2 0 002-2v-6a2 2 0 00-2-2h-.5V5.5A4.5 4.5 0 0010 1zm3 8V5.5a3 3 0 10-6 0V9h6z" clipRule="evenodd" />
                    </svg>
                  )}
                </label>
              );
            })}
          </div>
        </div>

        <div style={{ padding: '1rem', borderTop: '1px solid var(--border-color)', background: 'var(--bg-surface)' }}>
          <button onClick={onClose} className="emp-btn-submit" style={{ width: '100%', padding: '0.75rem', justifyContent: 'center' }}>
            Hoàn tất
          </button>
        </div>
      </div>
    </div>
  );
};